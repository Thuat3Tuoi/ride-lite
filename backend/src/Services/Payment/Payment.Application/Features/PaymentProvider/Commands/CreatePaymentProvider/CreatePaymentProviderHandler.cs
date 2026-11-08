using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;

public sealed class CreatePaymentProviderHandler
{
    private readonly IPaymentProviderRepository _paymentProviderRepository;

    public CreatePaymentProviderHandler(IPaymentProviderRepository paymentProviderRepository)
    {
        _paymentProviderRepository = paymentProviderRepository;
    }

    public async Task<CreatePaymentProviderResult> HandleAsync(
        CreatePaymentProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var provider = Domain.Entities.PaymentProvider.Create(
            command.Code,
            command.Name,
            command.HandlerType,
            command.Active
        );

        // 2. Kiểm tra mã nhà cung cấp trùng
        var exists = await _paymentProviderRepository.ExistsByCodeAsync(
            provider.Code,
            cancellationToken
        );

        if (exists)
        {
            throw new AppException(PaymentErrors.PaymentProviderAlreadyExists);
        }

        // 3. Lưu PaymentProvider
        await _paymentProviderRepository.AddAsync(
            provider,
            cancellationToken
        );

        await _paymentProviderRepository.SaveChangesAsync(
            cancellationToken
        );

        // 4. Trả kết quả
        return new CreatePaymentProviderResult(
            provider.Id,
            provider.Code,
            provider.Name,
            provider.HandlerType,
            provider.Active
        );
    }
}
