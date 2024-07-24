using FluentValidation;
using MediatR;
using System.Text;

namespace Banking.Application
{
    public class PipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            var validationContext = new ValidationContext<TRequest>(request);
            var validationResult = _validators.Select(v => v.Validate(validationContext)).SelectMany(result => result.Errors)
                .Where(failure => failure is not null);

            if (validationResult.Any())
            {
                var stringBuilder = new StringBuilder();
                validationResult.ToList().ForEach(x => stringBuilder.AppendLine(x.ErrorMessage));
                var exception = new ValidationException(stringBuilder.ToString());

                throw exception;
            }


            return next();
        }
    }
}