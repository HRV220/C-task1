using Library.Application.Abstractions.Services;
using Library.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IRequestService, RequestService>();

        return services;
    }
}
