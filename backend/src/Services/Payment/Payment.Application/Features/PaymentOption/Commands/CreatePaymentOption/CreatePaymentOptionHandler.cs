using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;
using Payment.Domain.Entities;

namespace Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;

public sealed class CreatePaymentOptionHandler
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public CreatePaymentOptionHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<CreatePaymentOptionResult> HandleAsync(
        CreatePaymentOptionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var option = Domain.Entities.PaymentOption.Create(
            command.MethodId,
            command.ProviderId,
            command.Code,
            command.Name,
            command.Active
        );

        // 2. Kiểm tra mã phương thức option trùng
        var exists = await _paymentOptionRepository.ExistsByCodeAsync(
            option.Code,
            cancellationToken
        );

        if (exists)
        {
            throw new PaymentOptionAlreadyExistsException(option.Code);
        }

        // 3. Lưu PaymentOption
        await _paymentOptionRepository.AddAsync(
            option,
            cancellationToken
        );

        await _paymentOptionRepository.SaveChangesAsync(
            cancellationToken
        );

        // 4. Trả kết quả
        return new CreatePaymentOptionResult(
            option.Id,
            option.MethodId,
            option.ProviderId,
            option.Code,
            option.Name,
            option.Active
        );
    }
}

