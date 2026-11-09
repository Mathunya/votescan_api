namespace Web_Api.Services;

// PR seat estimate from captured slips, following Schedule 1 of the Local Government:
// Municipal Structures Act 117 of 1998 (as amended):
//   quota  = floor(party ward+PR votes / (council seats - independent-won wards -
//            wards won by parties with no PR list)) + 1
//   seats  = floor(party ward+PR votes / quota), remaining seats to the largest remainders
//   PR     = total seats - ward seats won
// Quota and allocation run on each party's ward+PR votes combined, NOT PR-ballot votes
// alone. A previous version of this file used PR-only votes, reasoning that ward seats
// are decided separately so folding ward votes in would double-count them; that
// reasoning was wrong — the Act's own quota pools ward and PR votes together before
// allocating, it's the SEATS (ward vs PR) that are kept separate, not the votes that
// size the quota. Verified against the IEC's own "Seat Calculation Detail" reports for
// all 19 Free State municipalities in the 2026 Local Government Elections: every one
// states a party's "Total Valid Votes" as ward+PR combined (e.g. Moqhaka: ANC listed at
// 26,506 = 13,200 ward + 13,306 PR, not 13,306 alone), and every municipality's quota
// reproduces exactly from that combined total — e.g. Moqhaka Q=1,841 from A=73,610
// (ward+PR for all PR-listed parties) over B=40 seats, not from the PR-only total of
// ~37,128. An earlier version before that divided by (seats - independents + 1) — a
// generic Droop quota, copied from this project's own planning doc rather than the Act —
// which gave a materially different (and also wrong) quota.
//
// Overhang (a party that already won more wards than its vote share would give it keeps
// those wards, takes no PR seat, and is removed from the pool so quota/remainder math
// reruns for the rest) is this project's own addition — the IEC deck doesn't describe
// that case, so it's unverified against an authoritative source, kept only because
// without it a party could structurally finish with negative PR seats.
public static class SeatEstimator
{
    // PrVotes separate from the ward/PR total: by law every registered party's PR ballot
    // covers every ward, so a "party" that won a ward but has zero votes anywhere on the
    // PR ballot is an independent candidate standing under their own name, not a party
    // with zero PR support. Independents never entered the PR-seat pool at all here —
    // this kept them from taking a PR seat in error, but it also meant their ward votes
    // (and the seat their ward uses up) stayed in the quota math, inflating everyone
    // else's share. They're now pulled out before quota/pool math starts, same as a
    // CouncilSeats.IndependentWards entry.
    public record PartyInput(string Key, int TotalVotes, int PrVotes, int WardsWon);
    public record PartyResult(string Key, int Seats, int WardSeats, int PrSeats);

    public record Estimate(int Quota, IReadOnlyList<PartyResult> Parties);

    // councilSeats: total seats on the council. independentWards: wards won by
    // independent candidates that CouncilSeats already knows about by hand (see
    // results_summary.sql) — on top of whichever independents this call detects itself
    // from zero PR votes.
    public static Estimate Estimate_(int councilSeats, int independentWards, IEnumerable<PartyInput> parties)
    {
        var all = parties.ToList();
        var detectedIndependents = all.Where(p => p.PrVotes == 0 && p.WardsWon > 0).ToList();
        var pool = all.Where(p => p.PrVotes > 0 || p.WardsWon == 0).ToList();

        var fixedSeats = new Dictionary<string, PartyResult>();
        foreach (var independent in detectedIndependents)
        {
            fixedSeats[independent.Key] = new PartyResult(independent.Key, independent.WardsWon, independent.WardsWon, 0);
        }
        var seatsToShare = Math.Max(0, councilSeats
            - Math.Max(0, independentWards)
            - detectedIndependents.Sum(p => p.WardsWon));
        var quota = 0;

        // At most one party can be removed as an overhang per pass, so this converges
        // in at most pool.Count passes; the explicit bound just guards against a bug
        // in the removal logic turning this into an infinite loop.
        for (var guard = 0; guard <= pool.Count; guard++)
        {
            var totalVotes = pool.Sum(p => p.TotalVotes);
            // Schedule 1: quota = floor(total party ward+PR votes / (council seats -
            // independent/no-PR-list wards)) + 1 — the +1 applies to the quotient, not
            // the denominator.
            quota = totalVotes > 0 && seatsToShare > 0
                ? (totalVotes / seatsToShare) + 1
                : 0;

            var allocations = pool.Select(p => new
            {
                p.Key,
                p.TotalVotes,
                p.WardsWon,
                BaseSeats = quota > 0 ? p.TotalVotes / quota : 0,
                Remainder = quota > 0 ? p.TotalVotes % quota : 0,
            }).ToList();

            var seatsAllocated = allocations.Sum(a => a.BaseSeats);
            var remaining = Math.Max(0, seatsToShare - seatsAllocated);
            var byRemainder = allocations
                .OrderByDescending(a => a.Remainder)
                .ThenByDescending(a => a.TotalVotes)
                .ToList();
            var bonus = byRemainder.Take(remaining).Select(a => a.Key).ToHashSet();

            // final seats, remainder included — the overhang check below must run
            // against this, not against BaseSeats, since a party's own remainder seat
            // can take it to (or past) its ward count
            var finalSeats = allocations.ToDictionary(a => a.Key, a => a.BaseSeats + (bonus.Contains(a.Key) ? 1 : 0));

            var overhang = allocations.FirstOrDefault(a => a.WardsWon > finalSeats[a.Key]);
            if (overhang != null && seatsToShare > 0)
            {
                // keeps its wards, gets no PR seats, and drops out of the pool that
                // shares the remaining seats; the quota is recalculated for everyone
                // else, same as steps 2-5 repeating per the plan
                fixedSeats[overhang.Key] = new PartyResult(overhang.Key, overhang.WardsWon, overhang.WardsWon, 0);
                pool = pool.Where(p => p.Key != overhang.Key).ToList();
                seatsToShare = Math.Max(0, seatsToShare - overhang.WardsWon);
                continue;
            }

            foreach (var a in allocations)
            {
                // WardSeats/PrSeats are placeholders — the pass below derives both from
                // Seats and each party's WardsWon for every entry, overhangs included.
                fixedSeats[a.Key] = new PartyResult(a.Key, finalSeats[a.Key], 0, 0);
            }
            break;
        }

        // wardSeats/prSeats split: a party's wards are always safe; whatever seats it
        // has beyond its wards are PR top-up.
        var results = fixedSeats.Values
            .Select(r => r with { WardSeats = Math.Min(r.Seats, all.First(p => p.Key == r.Key).WardsWon) })
            .Select(r => r with { PrSeats = Math.Max(0, r.Seats - r.WardSeats) })
            .OrderByDescending(r => r.Seats)
            .ToList();
        return new Estimate(quota, results);
    }
}
