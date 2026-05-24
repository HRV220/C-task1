using Library.Application;
using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Services;
using Library.Infrastructure;
using Library.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Library.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersServices()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IUserService));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IBookService));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IRequestService));
    }

    [Fact]
    public void AddInfrastructure_RegistersRepositoriesAndDbContext()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure("Host=localhost;Database=library;Username=postgres;Password=postgres");

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IUserRepository));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IBookRepository));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IRequestRepository));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(LibraryDbContext));
    }
}
