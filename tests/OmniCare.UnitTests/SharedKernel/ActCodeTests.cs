using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.SharedKernel;

public class ActCodeTests
{
    [Fact]
    public void InamiCode_accepts_six_digits()
    {
        var code = InamiCode.Create("560011");
        Assert.Equal("560011", code.Value);
        Assert.IsAssignableFrom<ActCode>(code);
    }

    [Fact]
    public void InamiCode_trims_input()
    {
        Assert.Equal("560011", InamiCode.Create(" 560011 ").Value);
    }

    [Theory]
    [InlineData("56001")]
    [InlineData("5600112")]
    [InlineData("56A011")]
    [InlineData("")]
    public void InamiCode_rejects_invalid_format(string input)
    {
        Assert.Throws<InvalidActCodeException>(() => InamiCode.Create(input));
    }

    [Fact]
    public void HealthProfession_resolves_known_codes_case_insensitively()
    {
        Assert.Equal(HealthProfession.Physiotherapy, HealthProfession.FromCode("physio"));
    }

    [Fact]
    public void HealthProfession_rejects_unknown_code()
    {
        Assert.Throws<DomainException>(() => HealthProfession.FromCode("ASTROLOGER"));
    }
}
