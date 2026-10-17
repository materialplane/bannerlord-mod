namespace RealmsOfDust.Core.Interfaces;

using System.Collections.Generic;
using RealmsOfDust.Core.Models;

/// <summary>
/// Service contract for selecting and initializing the player's custom starting domain.
/// </summary>
public interface IPlayerRealmInitializer
{
    /// <summary>
    /// Selects a randomized domain for the player matching their chosen culture.
    /// </summary>
    /// <param name="availableDomains">All allocated domains on the map.</param>
    /// <returns>The assigned RealmDomain for the player.</returns>
    RealmDomain SelectPlayerDomain(IReadOnlyList<RealmDomain> availableDomains);

    /// <summary>
    /// Elevates the player clan to kingdom status and grants starting assets.
    /// </summary>
    /// <param name="playerDomain">The domain assigned to the player.</param>
    void InitializePlayerRealm(RealmDomain playerDomain);
}
