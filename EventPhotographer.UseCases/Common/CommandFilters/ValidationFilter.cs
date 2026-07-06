using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.PipelineBehaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.UseCases.Common.CommandFilters;

internal class ValidationFilter(
    IServiceProvider serviceProvider)
    : ICommandFilter
{
    public async Task<Result<TResult>?> ApplyFilterAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResult>
    {
        var validator = serviceProvider.GetService<IValidator<TCommand>>();
        if (validator == null)
        {
            return null;
        }

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult);
        }

        return null;
    }
}
