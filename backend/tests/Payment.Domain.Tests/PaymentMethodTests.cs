using Payment.Domain.Entities;
using Xunit;

namespace Payment.Domain.Tests;

public class PaymentMethodTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePaymentMethod()
    {
        var method = PaymentMethod.Create("  e_wallet  ", "  Ví điện tử  ", active: true);

        Assert.Equal("E_WALLET", method.Code);
        Assert.Equal("Ví điện tử", method.Name);
        Assert.True(method.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrow(string invalidCode)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentMethod.Create(invalidCode, "Ví điện tử"));
    }

    [Fact]
    public void Create_WithExcessiveCode_ShouldThrow()
    {
        var longCode = new string('A', 33);

        Assert.Throws<ArgumentException>(() =>
            PaymentMethod.Create(longCode, "Ví điện tử"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentMethod.Create("WALLET", invalidName));
    }

    [Fact]
    public void Create_WithExcessiveName_ShouldThrow()
    {
        var longName = new string('a', 256);

        Assert.Throws<ArgumentException>(() =>
            PaymentMethod.Create("WALLET", longName));
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        var method = PaymentMethod.Create("WALLET", "Ví điện tử", active: true);

        method.Deactivate();

        Assert.False(method.Active);
    }

    [Fact]
    public void Activate_ShouldSetActiveToTrue()
    {
        var method = PaymentMethod.Create("WALLET", "Ví điện tử", active: false);

        method.Activate();

        Assert.True(method.Active);
    }
}

