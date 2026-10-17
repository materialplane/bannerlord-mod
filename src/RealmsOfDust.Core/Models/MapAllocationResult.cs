namespace RealmsOfDust.Core.Models;

using System.Collections.Generic;

/// <summary>
/// Encapsulates the complete result of partitioning map settlements into
/// sovereign domains and unaligned frontier strongholds.
/// </summary>
public sealed class MapAllocationResult
{
    /// <summary>
    /// Gets all balanced sovereign domains generated on the active map.
    /// </summary>
    public IReadOnlyList<RealmDomain> Domains { get; }

    /// <summary>
    /// Gets all unaligned frontier strongholds generated on the active map.
    /// </summary>
    public IReadOnlyList<FrontierHold> FrontierHolds { get; }

    public MapAllocationResult(
        IReadOnlyList<RealmDomain> domains,
        IReadOnlyList<FrontierHold> frontierHolds)
    {
        Domains = domains ?? throw new System.ArgumentNullException(nameof(domains));
        FrontierHolds = frontierHolds ?? throw new System.ArgumentNullException(nameof(frontierHolds));
    }
}
