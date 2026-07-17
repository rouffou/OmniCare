using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.Agenda;

public class TimeSlotTests
{
    private static readonly DateTimeOffset T0 = new(2026, 7, 20, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_rejects_end_before_start()
    {
        Assert.Throws<DomainException>(() => TimeSlot.Create(T0, T0));
        Assert.Throws<DomainException>(() => TimeSlot.Create(T0, T0.AddMinutes(-30)));
    }

    [Fact]
    public void Create_rejects_slots_longer_than_8_hours()
    {
        Assert.Throws<DomainException>(() => TimeSlot.Create(T0, T0.AddHours(9)));
    }

    [Theory]
    [InlineData(0, 30, 15, 45, true)]   // chevauchement partiel
    [InlineData(0, 30, 0, 30, true)]    // identique
    [InlineData(0, 30, 30, 60, false)]  // adjacent
    [InlineData(0, 30, 60, 90, false)]  // disjoint
    public void Overlaps_detects_intersections(int s1, int e1, int s2, int e2, bool expected)
    {
        var a = TimeSlot.Create(T0.AddMinutes(s1), T0.AddMinutes(e1));
        var b = TimeSlot.Create(T0.AddMinutes(s2), T0.AddMinutes(e2));
        Assert.Equal(expected, a.Overlaps(b));
        Assert.Equal(expected, b.Overlaps(a));
    }

    [Fact]
    public void AppointmentType_validates_name_and_duration()
    {
        Assert.Throws<DomainException>(() =>
            AppointmentType.Define("", HealthProfession.Physiotherapy, TimeSpan.FromMinutes(30)));
        Assert.Throws<DomainException>(() =>
            AppointmentType.Define("Séance", HealthProfession.Physiotherapy, TimeSpan.Zero));

        var type = AppointmentType.Define("Séance individuelle", HealthProfession.Physiotherapy, TimeSpan.FromMinutes(30));
        Assert.True(type.IsActive);
        type.Deactivate();
        Assert.False(type.IsActive);
    }
}
