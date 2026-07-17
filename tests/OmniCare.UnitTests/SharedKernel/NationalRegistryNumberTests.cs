using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.SharedKernel;

public class NationalRegistryNumberTests
{
    [Fact]
    public void Create_accepts_valid_number_born_before_2000()
    {
        // 850107123 mod 97 = 93 → clé 04
        var niss = NationalRegistryNumber.Create("85010712304");
        Assert.Equal("85010712304", niss.Value);
    }

    [Fact]
    public void Create_accepts_valid_number_born_from_2000()
    {
        // (2000000000 + 040229045) mod 97 = 12 → clé 85
        var niss = NationalRegistryNumber.Create("04022904585");
        Assert.Equal("04022904585", niss.Value);
    }

    [Fact]
    public void Create_accepts_formatted_input()
    {
        var niss = NationalRegistryNumber.Create("85.01.07-123.04");
        Assert.Equal("85010712304", niss.Value);
    }

    [Theory]
    [InlineData("85010712305")] // clé incorrecte
    [InlineData("1234567890")]  // 10 chiffres
    [InlineData("")]
    [InlineData("aaaaaaaaaaa")]
    public void Create_rejects_invalid_numbers(string input)
    {
        Assert.Throws<InvalidNationalRegistryNumberException>(
            () => NationalRegistryNumber.Create(input));
    }

    [Fact]
    public void Masked_hides_middle_digits()
    {
        var niss = NationalRegistryNumber.Create("85010712304");
        Assert.Equal("85*******04", niss.Masked);
        Assert.DoesNotContain("0107123", niss.ToString());
    }
}
