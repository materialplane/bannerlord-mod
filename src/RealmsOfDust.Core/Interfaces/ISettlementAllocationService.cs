namespace RealmsOfDust.Core.Interfaces;

using RealmsOfDust.Core.Models;

/// <summary>
/// Service contract for discovering and partitioning map settlements into
/// balanced sovereign domains and frontier strongholds.
/// </summary>
public interface ISettlementAllocationService
{
    /// <summary>
    /// Analyzes all active settlements and generates the balanced map allocation.
    /// </summary>
    /// <returns>A structured MapAllocationResult containing all domains and frontier holds.</returns>
    MapAllocationResult AllocateMap();
}
