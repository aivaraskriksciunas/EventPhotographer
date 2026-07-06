using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.PipelineBehaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.UseCases.Common.Decorators;

internal class FilterDecorator<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
    where TResponse : class
{
    private readonly ICommandHandler<TCommand, TResponse> _decoratedHandler;
    private readonly IServiceProvider _serviceProvider;

    public FilterDecorator(
        ICommandHandler<TCommand, TResponse> decoratedHandler,
        IServiceProvider serviceProvider)
    {
        _decoratedHandler = decoratedHandler;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var filters = _serviceProvider.GetServices<ICommandFilter>();

        foreach (var filter in filters)
        {
            var result = await filter.ApplyFilterAsync<TCommand, TResponse>(command, cancellationToken);

            if (result != null) return result;
        }

        return await _decoratedHandler.HandleAsync(command, cancellationToken);
    }
}
