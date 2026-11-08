using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;
using Payment.Domain.Entities;

namespace Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;

public sealed class CreatePaymentMethodHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public CreatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<CreatePaymentMethodResult> HandleAsync(
        CreatePaymentMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var paymentMethod = Domain.Entities.PaymentMethod.Create(
            command.Code,
            command.Name,
            command.Active
        );

        // 2. Kiểm tra mã phương thức trùng
        var exists = await _paymentMethodRepository.ExistsByCodeAsync(
            paymentMethod.Code,
            cancellationToken
        );

        if (exists)
        {
            throw new PaymentMethodAlreadyExistsException(paymentMethod.Code);
        }

        // 3. Lưu PaymentMethod
        await _paymentMethodRepository.AddAsync(
            paymentMethod,
            cancellationToken
        );

        await _paymentMethodRepository.SaveChangesAsync(
            cancellationToken
        );

        // 4. Trả kết quả
        return new CreatePaymentMethodResult(
            paymentMethod.Id,
            paymentMethod.Code,
            paymentMethod.Name,
            paymentMethod.Active
        );
    }
}

