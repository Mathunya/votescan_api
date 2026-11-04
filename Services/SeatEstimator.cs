namespace Web_Api.Services;

// PR seat estimate from captured slips, following Schedule 1 of the Local Government:
// Municipal Structures Act 117 of 1998 (as amended) — largest-remainder allocation,
// with an overhang check: a party that already won more wards than its vote share
// would give it keeps those wards and takes no PR seats, and is removed from the pool
// before the rest is shared out (steps 2-6 of the Results Slip Workflow and Seats plan,
// 4 Nov 2026). This is an estimate from the slips counted so far; it does not apply the
// IEC's own rounding/remainder rules and has not been checked against an IEC publication.
public static class SeatEstimator
{
    public record PartyInput(string Key, int TotalVotes, int WardsWon);
    public record PartyResult(string Key, int Seats, int WardSeats, int PrSeats);

    public record Estimate(int Quota, IReadOnlyList<PartyResult> Parties);

    // councilSeats: total seats on the council. independentWards: wards won by
    // independent candidates, entered by hand in CouncilSeats (see results_summary.sql) —
    // not detected from the slips.
    public static Estimate Estimate_(int councilSeats, int independentWards, IEnumerable<PartyInput> parties)
    {
        var pool = parties.Where(p => p.TotalVotes > 0 || p.WardsWon > 0).ToList();
        var fixedSeats = new Dictionary<string, PartyResult>();
        var seatsToShare = Math.Max(0, councilSeats - Math.Max(0, independentWards));
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

            var overhang = allocations.FirstOrDefault(a => a.WardsWon > a.BaseSeats);
            if (overhang != null && seatsToShare > 0)
            {
                // keeps its wards, gets no PR seats, and drops out of the pool that
                // shares the remaining seats
                fixedSeats[overhang.Key] = new PartyResult(overhang.Key, overhang.WardsWon, overhang.WardsWon, 0);
                pool = pool.Where(p => p.Key != overhang.Key).ToList();
                seatsToShare = Math.Max(0, seatsToShare - overhang.WardsWon);
                continue;
            }

            var seatsAllocated = allocations.Sum(a => a.BaseSeats);
            var remaining = Math.Max(0, seatsToShare - seatsAllocated);
            var byRemainder = allocations
                .OrderByDescending(a => a.Remainder)
                .ThenByDescending(a => a.TotalVotes)
                .ToList();
            var bonus = byRemainder.Take(remaining).Select(a => a.Key).ToHashSet();

            // WardSeats/PrSeats are placeholders here — the pass below derives both
            // from Seats and each party's WardsWon for every entry, overhangs included.
            foreach (var a in allocations)
            {
                var seats = a.BaseSeats + (bonus.Contains(a.Key) ? 1 : 0);
                fixedSeats[a.Key] = new PartyResult(a.Key, seats, 0, 0);
            }
            break;
        }

        // wardSeats/prSeats split: a party's wards are always safe; whatever seats it
        // has beyond its wards are PR top-up.
        var results = fixedSeats.Values
            .Select(r => r with { WardSeats = Math.Min(r.Seats, parties.First(p => p.Key == r.Key).WardsWon) })
            .Select(r => r with { PrSeats = Math.Max(0, r.Seats - r.WardSeats) })
            .OrderByDescending(r => r.Seats)
            .ToList();
        return new Estimate(quota, results);
    }
}
