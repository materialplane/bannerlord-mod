namespace RealmsOfDust.Engine;

using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

/// <summary>
/// Suppresses and bypasses the vanilla campaign intro video, routing directly to character creation.
/// </summary>
public sealed class IntroVideoBypassService : IGameStateManagerListener
{
    private static IntroVideoBypassService? _instance;
    private bool _isRegistered;
    private VideoPlaybackState? _pendingBypass;

    /// <summary>
    /// Gets the singleton instance of the intro video bypass service.
    /// </summary>
    public static IntroVideoBypassService Instance => _instance ??= new IntroVideoBypassService();

    /// <summary>
    /// Ensures this service is registered as a listener on the active GameStateManager.
    /// </summary>
    public void EnsureRegistered()
    {
        if (_isRegistered || GameStateManager.Current == null)
        {
            return;
        }

        GameStateManager.Current.RegisterListener(this);
        _isRegistered = true;
    }

    /// <summary>
    /// Per-frame update hook to immediately catch and finalize any lingering intro video state.
    /// </summary>
    public void Tick()
    {
        EnsureRegistered();

        if (_pendingBypass != null)
        {
            VideoPlaybackState pending = _pendingBypass;
            _pendingBypass = null;
            pending.OnVideoFinished();
            return;
        }

        if (GameStateManager.Current?.ActiveState is VideoPlaybackState activeState &&
            IsCampaignIntro(activeState))
        {
            activeState.SetStartingParameters(string.Empty, string.Empty, string.Empty, 30f);
            activeState.OnVideoFinished();
        }
    }

    /// <inheritdoc />
    public void OnPushState(GameState gameState, bool isTopProcess)
    {
        if (gameState is VideoPlaybackState videoState && IsCampaignIntro(videoState))
        {
            // Wipe media paths immediately to prevent decoding or audio streaming.
            videoState.SetStartingParameters(string.Empty, string.Empty, string.Empty, 30f);
            _pendingBypass = videoState;
            videoState.OnVideoFinished();
        }
    }

    /// <inheritdoc />
    public void OnCreateState(GameState gameState)
    {
    }

    /// <inheritdoc />
    public void OnPopState(GameState gameState)
    {
        if (_pendingBypass == gameState)
        {
            _pendingBypass = null;
        }
    }

    /// <inheritdoc />
    public void OnCleanStates()
    {
        _pendingBypass = null;
    }

    /// <inheritdoc />
    public void OnSavedGameLoadFinished()
    {
    }

    private static bool IsCampaignIntro(VideoPlaybackState state)
    {
        return !string.IsNullOrEmpty(state.VideoPath) &&
               state.VideoPath.IndexOf("campaign_intro", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
