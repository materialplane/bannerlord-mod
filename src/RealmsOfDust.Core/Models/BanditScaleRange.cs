namespace RealmsOfDust.Core.Models;

/// <summary>
/// Defines the minimum and maximum headcount bounds for bandit party generation.
/// </summary>
public readonly struct BanditScaleRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BanditScaleRange"/> struct.
    /// </summary>
    /// <param name="minCount">Minimum troop count.</param>
    /// <param name="maxCount">Maximum troop count.</param>
    public BanditScaleRange(int minCount, int maxCount)
    {
        MinCount = minCount;
        MaxCount = maxCount;
    }

    /// <summary>
    /// Gets the minimum troop headcount.
    /// </summary>
    public int MinCount { get; }

    /// <summary>
    /// Gets the maximum troop headcount.
    /// </summary>
    public int MaxCount { get; }
}
