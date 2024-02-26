using Microsoft.Extensions.DependencyInjection;

using AutoMapper;

using StudnetManagement.Domain.Commands;
using StudentManagement.Domain.Helpers;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.Profiles;

namespace StudentManagement.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            _ = services.AddScoped<IRepository, Repository>();
            _ = services.AddScoped<IIdentifierGenerator, IdentifierGenerator>();
            _ = services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            _ = services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CreateCourseCommand).Assembly));
            _ = services.AddSingleton(_ =>
            {
                MapperConfiguration mc = new(cfg => cfg.AddProfile<StudentManagementProfile>());
                return mc.CreateMapper();
            });

            return services;
        }
    }
}