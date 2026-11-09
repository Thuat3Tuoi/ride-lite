using Payment.Domain.Entities;
using Payment.Domain.Enumerators;
using Xunit;

namespace Payment.Domain.Tests;

public class PaymentProviderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePaymentProvider()
    {
        var provider = PaymentProvider.Create(
            code: "  vnpay  ",
            name: "  VNPay Gateway  ",
            handlerType: PaymentHandlerType.Vnpay,
            active: true);

        Assert.Equal("VNPAY", provider.Code);
        Assert.Equal("VNPay Gateway", provider.Name);
        Assert.Equal(PaymentHandlerType.Vnpay, provider.HandlerType);
        Assert.True(provider.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrow(string invalidCode)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentProvider.Create(invalidCode, "VNPay Gateway", PaymentHandlerType.Vnpay));
    }

    [Fact]
    public void Create_WithExcessiveCode_ShouldThrow()
    {
        var longCode = new string('A', 33);

        Assert.Throws<ArgumentException>(() =>
            PaymentProvider.Create(longCode, "VNPay Gateway", PaymentHandlerType.Vnpay));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
            PaymentProvider.Create("VNPAY", invalidName, PaymentHandlerType.Vnpay));
    }

    [Fact]
    public void Create_WithExcessiveName_ShouldThrow()
    {
        var longName = new string('a', 256);

        Assert.Throws<ArgumentException>(() =>
            PaymentProvider.Create("VNPAY", longName, PaymentHandlerType.Vnpay));
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        var provider = PaymentProvider.Create("VNPAY", "VNPay Gateway", PaymentHandlerType.Vnpay, active: true);

        provider.Deactivate();

        Assert.False(provider.Active);
    }

    [Fact]
    public void Activate_ShouldSetActiveToTrue()
    {
        var provider = PaymentProvider.Create("VNPAY", "VNPay Gateway", PaymentHandlerType.Vnpay, active: false);

        provider.Activate();

        Assert.True(provider.Active);
    }
}

