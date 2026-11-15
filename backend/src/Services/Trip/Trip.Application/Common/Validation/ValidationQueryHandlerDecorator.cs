using FluentValidation;
using Trip.Application.Abstractions.Messaging;

namespace Trip.Application.Common.Validation;

public sealed class ValidationQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _inner;
    private readonly IValidator<TQuery>? _validator;

    public ValidationQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> inner,
        IValidator<TQuery>? validator = null)
    {
        _inner = inner;
        _validator = validator;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (_validator is not null)
        {
            await _validator.EnsureValidAsync(query, cancellationToken);
        }

        return await _inner.HandleAsync(query, cancellationToken);
    }
}

