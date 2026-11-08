using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;

namespace Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;

public sealed class CreatePaymentLedgerEntryHandler
{
    private readonly ILedgerEntryRepository _ledgerEntryRepository;

    public CreatePaymentLedgerEntryHandler(ILedgerEntryRepository ledgerEntryRepository)
    {
        _ledgerEntryRepository = ledgerEntryRepository;
    }

    public async Task<CreatePaymentLedgerEntryResult> HandleAsync(
        CreatePaymentLedgerEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var entry = Domain.Entities.LedgerEntry.CreatePaymentEntry(
            command.PaymentId,
            command.Amount,
            command.DebitAccount,
            command.CreditAccount
        );

        // 2. Lưu LedgerEntry
        await _ledgerEntryRepository.AddAsync(
            entry,
            cancellationToken
        );

        await _ledgerEntryRepository.SaveChangesAsync(
            cancellationToken
        );

        // 3. Trả kết quả
        return new CreatePaymentLedgerEntryResult(
            entry.Id,
            entry.PaymentId,
            entry.EntryType.ToString(),
            entry.EntryRef,
            entry.DebitAccount,
            entry.CreditAccount,
            entry.Amount,
            entry.CreatedAt
        );
    }
}

