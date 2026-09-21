using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PlaylistControl.Application.Behaviors;

namespace PlaylistControl.Application
{
    /// <summary>
    /// Extension methods for registering Application layer services
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers MediatR handlers, FluentValidation validators, and the validation pipeline behavior for the Application layer.
        /// </summary>
        /// <param name="services">The service collection to add registrations to</param>
        /// <returns>The same service collection, for chaining</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}