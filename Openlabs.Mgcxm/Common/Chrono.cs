// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common;

/// <summary>
/// Represents a ratio of two integers.
/// </summary>
public class Ratio
{
    /// <summary>
    /// Creates a new instance of the <see cref="Ratio"/> class with the specified numerator and denominator.
    /// </summary>
    /// <param name="num">The numerator of the ratio.</param>
    /// <param name="den">The denominator of the ratio.</param>
    /// <returns>A new instance of the <see cref="Ratio"/> class.</returns>
    public static Ratio Create(int num, int den) => new Ratio { _num = num, _den = den };

    /// <summary>
    /// Gets the result of the ratio as an integer value (integer division of numerator by denominator).
    /// </summary>
    /// <returns>The result of the ratio as an integer value.</returns>
    public int GetValue() => Numerator / Denominator;

    /// <summary>
    /// Gets the numerator of the ratio.
    /// </summary>
    public int Numerator => _num;

    /// <summary>
    /// Gets the denominator of the ratio.
    /// </summary>
    public int Denominator => _den;

    private int _num;
    private int _den;
}

/// <summary>
/// Represents a value in time.
/// </summary>
public class Duration
{
    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in hours.
    /// </summary>
    /// <param name="period">The value of the duration in hours.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in hours.</returns>
    public static Duration Hours(int period) => Create(DurationType.Hours, period);

    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in minutes.
    /// </summary>
    /// <param name="period">The value of the duration in minutes.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in minutes.</returns>
    public static Duration Minutes(int period) => Create(DurationType.Minutes, period);

    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in seconds.
    /// </summary>
    /// <param name="period">The value of the duration in seconds.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in seconds.</returns>
    public static Duration Seconds(int period) => Create(DurationType.Seconds, period);

    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in milliseconds.
    /// </summary>
    /// <param name="period">The value of the duration in milliseconds.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in milliseconds.</returns>
    public static Duration Milliseconds(int period) => Create(DurationType.Milliseconds, period);

    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in microseconds.
    /// </summary>
    /// <param name="period">The value of the duration in microseconds.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in microseconds.</returns>
    public static Duration Microseconds(int period) => Create(DurationType.Microseconds, period);

    /// <summary>
    /// Creates a new instance of <see cref="Duration"/> with the specified duration in nanoseconds.
    /// </summary>
    /// <param name="period">The value of the duration in nanoseconds.</param>
    /// <returns>A new instance of <see cref="Duration"/> with the specified duration in nanoseconds.</returns>
    public static Duration Nanoseconds(int period) => Create(DurationType.Nanoseconds, period);

    /// <summary>
    /// Creates a new instance of the <see cref="Duration"/> class with the specified duration type and period.
    /// </summary>
    /// <param name="type">The type of duration (e.g., hours, minutes, seconds, etc.).</param>
    /// <param name="period">The value of the duration period.</param>
    /// <returns>A new instance of the <see cref="Duration"/> class.</returns>
    public static Duration Create(DurationType type, int period)
    {
        var dur = new Duration();
        dur._durationType = type;
        dur._durationPeriod = period;

        return dur;
    }

    /// <summary>
    /// Gets the actual value of the duration in hours.
    /// </summary>
    /// <returns>The value of the duration in hours.</returns>
    public long GetHours() => (long)(GetSeconds() / HoursRatio.Denominator);

    /// <summary>
    /// Gets the actual value of the duration in minutes.
    /// </summary>
    /// <returns>The value of the duration in minutes.</returns>
    public long GetMinutes() => (long)(GetSeconds() / MinutesRatio.Denominator);

    /// <summary>
    /// Gets the actual value of the duration in seconds.
    /// </summary>
    /// <returns>The value of the duration in seconds.</returns>
    public float GetSeconds()
    {
        float curPeriod = _durationPeriod;
        switch (_durationType)
        {
            case DurationType.Hours: return curPeriod * HoursRatio.Numerator;
            case DurationType.Minutes: return curPeriod * MinutesRatio.Numerator;
            case DurationType.Seconds: return curPeriod * SecondsRatio.Numerator;
            case DurationType.Milliseconds: return curPeriod / MillisecondsRatio.Denominator;
            case DurationType.Microseconds: return curPeriod / MicrosecondsRatio.Denominator;
            case DurationType.Nanoseconds: return curPeriod / NanosecondsRatio.Denominator;
        }

        throw new InvalidOperationException("Cannot return a period unit that hasn't been changed.");
    }

    /// <summary>
    /// Gets the actual value of the duration in milliseconds.
    /// </summary>
    /// <returns>The value of the duration in milliseconds.</returns>
    public long GetMilliseconds() => (long)(GetSeconds() * MillisecondsRatio.Denominator);

    /// <summary>
    /// Gets the actual value of the duration in microseconds.
    /// </summary>
    /// <returns>The value of the duration in microseconds.</returns>
    public long GetMicroseconds() => (long)(GetSeconds() * MicrosecondsRatio.Denominator);

    /// <summary>
    /// Gets the actual value of the duration in nanoseconds.
    /// </summary>
    /// <returns>The value of the duration in nanoseconds.</returns>
    public long GetNanoseconds() => (long)(GetSeconds() * NanosecondsRatio.Denominator);

    /// <summary>
    /// Gets the actual value of the duration in TimeSpan ticks.
    /// </summary>
    /// <returns>The value of the duration in TimeSpan ticks.</returns>
    public long GetTicks() => TimeSpan.Ticks;

    /// <summary>
    /// Gets the duration as a <see cref="TimeSpan"/> instance.
    /// </summary>
    public TimeSpan TimeSpan => TimeSpan.FromSeconds(GetSeconds());

    private int _durationPeriod;
    private DurationType _durationType;

    // positive multiplier
    private static readonly Ratio HoursRatio = Ratio.Create(3600, 1);
    private static readonly Ratio MinutesRatio = Ratio.Create(60, 1);
    private static readonly Ratio SecondsRatio = Ratio.Create(1, 1);

    // positive divider
    private static readonly Ratio MillisecondsRatio = Ratio.Create(1, 1000);
    private static readonly Ratio MicrosecondsRatio = Ratio.Create(1, 1000000);
    private static readonly Ratio NanosecondsRatio = Ratio.Create(1, 1000000000);
}

/// <summary>
/// Represents the type of duration.
/// </summary>
public enum DurationType
{
    /// <summary>
    /// Duration in hours (3600 seconds).
    /// </summary>
    Hours,

    /// <summary>
    /// Duration in minutes (60 seconds).
    /// </summary>
    Minutes,

    /// <summary>
    /// Duration in seconds.
    /// </summary>
    Seconds,

    /// <summary>
    /// Duration in milliseconds (1/1000 seconds).
    /// </summary>
    Milliseconds,

    /// <summary>
    /// Duration in microseconds (1/1000000 seconds).
    /// </summary>
    Microseconds,

    /// <summary>
    /// Duration in nanoseconds (1/1000000000 seconds).
    /// </summary>
    Nanoseconds
}