namespace Web_Api.Services;

// PR seat estimate from captured slips, following Schedule 1 of the Local Government:
// Municipal Structures Act 117 of 1998 (as amended) — largest-remainder allocation,
// with an overhang check: a party that already won more wards than its vote share
// would give it (after the remainder seats are handed out, not before — a party right
// on the edge can be pushed either side of its ward count by its own remainder seat)
// keeps those wards and takes no PR seats, and is removed from the pool before the
// rest is shared out (steps 2-6 of the Results Slip Workflow and Seats plan,
// 4 Nov 2026). This is an estimate from the slips counted so far; it does not apply the
// IEC's own rounding/remainder rules and has not been checked against an IEC publication.
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
            quota = totalVotes > 0 && seatsToShare > 0
                ? (int)(totalVotes / (seatsToShare + 1)) + 1
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
