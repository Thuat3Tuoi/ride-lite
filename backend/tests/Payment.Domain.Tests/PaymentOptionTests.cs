using Payment.Domain.Entities;
using Xunit;

namespace Payment.Domain.Tests;

public class PaymentOptionTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePaymentOption()
    {
        var option = PaymentOption.Create(
            methodId: 1L,
            providerId: 2L,
            code: "  momo_wallet  ",
            name: "  Ví MoMo  ",
            active: true);

        Assert.Equal(1L, option.MethodId);
        Assert.Equal(2L, option.ProviderId);
        Assert.Equal("MOMO_WALLET", option.Code);
        Assert.Equal("Ví MoMo", option.Name);
        Assert.True(option.Active);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidMethodId_ShouldThrow(long invalidMethodId)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(invalidMethodId, 2L, "MOMO", "Ví MoMo"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidProviderId_ShouldThrow(long invalidProviderId)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(1L, invalidProviderId, "MOMO", "Ví MoMo"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrow(string invalidCode)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(1L, 2L, invalidCode, "Ví MoMo"));
    }

    [Fact]
    public void Create_WithExcessiveCode_ShouldThrow()
    {
        var longCode = new string('A', 33);

        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(1L, 2L, longCode, "Ví MoMo"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(1L, 2L, "MOMO", invalidName));
    }

    [Fact]
    public void Create_WithExcessiveName_ShouldThrow()
    {
        var longName = new string('a', 256);

        Assert.Throws<ArgumentException>(() =>
            PaymentOption.Create(1L, 2L, "MOMO", longName));
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        var option = PaymentOption.Create(1L, 2L, "MOMO", "Ví MoMo", active: true);

        option.Deactivate();

        Assert.False(option.Active);
    }

    [Fact]
    public void Activate_ShouldSetActiveToTrue()
    {
        var option = PaymentOption.Create(1L, 2L, "MOMO", "Ví MoMo", active: false);

        option.Activate();

        Assert.True(option.Active);
    }
}

