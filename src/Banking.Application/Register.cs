using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Banking.Application
{
    public static class Register
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddValidatorsFromAssembly(assembly);
            services.AddMediatR(config =>
            {
                config.Lifetime = ServiceLifetime.Scoped;
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(PipelineBehavior<,>));
            });

            return services;
        }
    }
}
