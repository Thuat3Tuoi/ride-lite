using FluentValidation;
using Payment.Application.Abstractions.Messaging;

namespace Payment.Application.Common.Validation;

public sealed class ValidationCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly IValidator<TCommand>? _validator;

    public ValidationCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> inner,
        IValidator<TCommand>? validator = null)
    {
        _inner = inner;
        _validator = validator;
    }

    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (_validator is not null)
        {
            await _validator.EnsureValidAsync(command, cancellationToken);
        }

        return await _inner.HandleAsync(command, cancellationToken);
    }
}

