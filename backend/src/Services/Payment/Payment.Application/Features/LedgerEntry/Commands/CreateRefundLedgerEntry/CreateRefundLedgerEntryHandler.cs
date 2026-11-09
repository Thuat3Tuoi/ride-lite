using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;

namespace Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;

public sealed class CreateRefundLedgerEntryHandler
{
    private readonly ILedgerEntryRepository _ledgerEntryRepository;

    public CreateRefundLedgerEntryHandler(ILedgerEntryRepository ledgerEntryRepository)
    {
        _ledgerEntryRepository = ledgerEntryRepository;
    }

    public async Task<CreateRefundLedgerEntryResult> HandleAsync(
        CreateRefundLedgerEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var entry = Domain.Entities.LedgerEntry.CreateRefundEntry(
            command.PaymentId,
            command.RefundId,
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
        return new CreateRefundLedgerEntryResult(
            entry.Id,
            entry.PaymentId,
            entry.RefundId!.Value,
            entry.EntryType.ToString(),
            entry.EntryRef,
            entry.DebitAccount,
            entry.CreditAccount,
            entry.Amount,
            entry.CreatedAt
        );
    }
}

