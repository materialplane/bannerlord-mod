namespace RealmsOfDust.Core.Models;

using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Represents a standardized sovereign domain comprising one Capital (Town),
/// one Stronghold (Castle), and their assigned bound Villages.
/// </summary>
public sealed class RealmDomain
{
    /// <summary>
    /// Gets the primary Capital (Town) of the realm.
    /// </summary>
    public Settlement Capital { get; }

    /// <summary>
    /// Gets the Stronghold (Castle) paired with the capital.
    /// </summary>
    public Settlement Stronghold { get; }

    /// <summary>
    /// Gets the list of bound villages assigned to this domain.
    /// </summary>
    public IReadOnlyList<Settlement> BoundVillages { get; }

    public RealmDomain(
        Settlement capital,
        Settlement stronghold,
        IReadOnlyList<Settlement> boundVillages)
    {
        Capital = capital ?? throw new System.ArgumentNullException(nameof(capital));
        Stronghold = stronghold ?? throw new System.ArgumentNullException(nameof(stronghold));
        BoundVillages = boundVillages ?? throw new System.ArgumentNullException(nameof(boundVillages));
    }
}
