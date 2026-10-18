namespace RealmsOfDust.Core.Interfaces;

/// <summary>
/// Service contract for pre-activating and populating hideouts on Day 1 campaign setup.
/// </summary>
public interface IHideoutActivationService
{
    /// <summary>
    /// Activates and seeds defender parties across map hideouts so bandit spawning begins immediately.
    /// </summary>
    void ActivateAllHideouts();
}
