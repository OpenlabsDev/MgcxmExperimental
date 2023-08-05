// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common;

public class Ratio
{
    public static Ratio Create(int num, int den) => new Ratio { _num = num, _den = den };

    public int GetValue() => Numerator / Denominator;

    public int Numerator => _num;
    public int Denominator => _den;

    private int _num;
    private int _den;
}

public class Duration
{
    public static Duration Create(DurationType type, int period)
    {
        var dur = new Duration();
        dur._durationType = type;
        dur._durationPeriod = period;

        return dur;
    }

    /// <summary> Gets the actual value in hours. </summary>
    public long GetHours() => (long)(GetSeconds() / HoursRatio.Denominator);

    /// <summary> Gets the actual value in minutes. </summary>
    public long GetMinutes() => (long)(GetSeconds() / MinutesRatio.Denominator);

    /// <summary> Gets the actual value in seconds. </summary>
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

    /// <summary> Gets the actual value in milliseconds. </summary>
    public long GetMilliseconds() => (long)(GetSeconds() * MillisecondsRatio.Denominator);

    /// <summary> Gets the actual value in microseconds. </summary>
    public long GetMicroseconds() => (long)(GetSeconds() * MicrosecondsRatio.Denominator);

    /// <summary> Gets the actual value in nanoseconds. </summary>
    public long GetNanoseconds() => (long)(GetSeconds() * NanosecondsRatio.Denominator);

    /// <summary> Gets the actual value in TimeSpan ticks. </summary>
    public long GetTicks() => TimeSpan.Ticks;

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

public enum DurationType
{
    /// <summary> 3600/1 </summary>
    Hours,
    /// <summary> 60/1 </summary>
    Minutes,
    /// <summary> 1/1 </summary>
    Seconds,
    /// <summary> 1/1000 </summary>
    Milliseconds,
    /// <summary> 1/1000000 </summary>
    Microseconds,
    /// <summary> 1/1000000000 </summary>
    Nanoseconds
}