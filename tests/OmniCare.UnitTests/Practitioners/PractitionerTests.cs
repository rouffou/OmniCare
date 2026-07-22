using OmniCare.Modules.Practitioners.Domain.Entities;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.Practitioners;

public class CabinetTests
{
    [Fact]
    public void Register_creates_active_cabinet()
    {
        var cabinet = Cabinet.Register(
            "Cabinet de kinésithérapie du Parc",
            BceNumber.Create("0123.456.789"),
            Address.Create("Rue de la Paix 12", "1000", "Bruxelles"));

        Assert.True(cabinet.IsActive);
        Assert.Equal("Cabinet de kinésithérapie du Parc", cabinet.Name);
        Assert.Equal("BE0123.456.789", cabinet.BceNumber.Formatted);
    }

    [Fact]
    public void Register_requires_name()
    {
        Assert.Throws<DomainException>(() => Cabinet.Register(
            "  ",
            BceNumber.Create("0123456789"),
            Address.Create("Rue de la Paix 12", "1000", "Bruxelles")));
    }

    [Fact]
    public void Deactivated_cabinet_refuses_address_update()
    {
        var cabinet = Cabinet.Register(
            "Cabinet", BceNumber.Create("0123456789"), Address.Create("Rue A", "1000", "Bruxelles"));
        cabinet.Deactivate();

        Assert.Throws<DomainException>(() =>
            cabinet.UpdateAddress(Address.Create("Rue B", "1000", "Bruxelles")));
    }
}

public class PractitionerTests
{
    [Fact]
    public void Register_creates_active_practitioner_linked_to_cabinet()
    {
        var cabinetId = Guid.NewGuid();
        var practitioner = Practitioner.Register(
            PersonName.Create("Marie", "Dupont"),
            HealthProfession.Physiotherapy,
            PractitionerInamiNumber.Create("12345678901"),
            cabinetId);

        Assert.Equal(PractitionerStatus.Active, practitioner.Status);
        Assert.Equal(cabinetId, practitioner.CabinetId);
        Assert.Equal("Marie Dupont", practitioner.Name.FullName);
    }

    [Fact]
    public void Register_rejects_empty_cabinet()
    {
        Assert.Throws<DomainException>(() => Practitioner.Register(
            PersonName.Create("Marie", "Dupont"),
            HealthProfession.Physiotherapy,
            PractitionerInamiNumber.Create("12345678901"),
            Guid.Empty));
    }

    [Fact]
    public void Deactivate_then_activate_round_trips_status()
    {
        var practitioner = Practitioner.Register(
            PersonName.Create("Marie", "Dupont"),
            HealthProfession.Physiotherapy,
            PractitionerInamiNumber.Create("12345678901"),
            Guid.NewGuid());

        practitioner.Deactivate();
        Assert.Equal(PractitionerStatus.Inactive, practitioner.Status);

        practitioner.Activate();
        Assert.Equal(PractitionerStatus.Active, practitioner.Status);
    }
}

public class BceNumberTests
{
    [Theory]
    [InlineData("0123.456.789")]
    [InlineData("0123456789")]
    [InlineData("BE 0123 456 789")]
    public void Create_accepts_common_formats(string input) =>
        Assert.Equal("0123456789", BceNumber.Create(input).Value);

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    [InlineData("012345678901")]
    public void Create_rejects_invalid_format(string input) =>
        Assert.Throws<DomainException>(() => BceNumber.Create(input));
}

public class PractitionerInamiNumberTests
{
    [Fact]
    public void Create_strips_separators()
    {
        Assert.Equal("12345678901", PractitionerInamiNumber.Create("1-2345678-901").Value);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    public void Create_rejects_invalid_format(string input) =>
        Assert.Throws<DomainException>(() => PractitionerInamiNumber.Create(input));
}
