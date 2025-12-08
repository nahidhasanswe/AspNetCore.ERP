using ERP.Core.ValueObjects;

namespace ERP.Domain.ValueObjects;

public class TimeRange : ValueObject
{
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    private TimeRange() { } // EF Core

    public TimeRange(TimeSpan startTime, TimeSpan endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        StartTime = startTime;
        EndTime = endTime;
    }

    public bool Overlaps(TimeRange other)
    {
        return StartTime < other.EndTime && other.StartTime < EndTime;
    }

    public TimeSpan Duration => EndTime - StartTime;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }
}