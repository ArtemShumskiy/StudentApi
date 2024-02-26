using System;
using System.IO;
using System.Reflection;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using StudentManagement.Api.Validators;
using StudentManagement.Api;
using StudentManagement.Domain;
using StudentManagement.Domain.DbContexts;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Student Management API",
        Description = "An ASP.NET Core Web API for managing students"
    });
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddOptions<StudentManagementOptions>()
    .Bind(builder.Configuration);

builder.Services.AddDomain();
builder.Services.AddDbContext<StudentManagementContext>(
    (IServiceProvider sp, DbContextOptionsBuilder c) =>
    {
        IOptionsMonitor<StudentManagementOptions> options = sp.GetRequiredService<IOptionsMonitor<StudentManagementOptions>>();
        _ = c.UseNpgsql(options.CurrentValue.StudentManagementConnectionString);
    });


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AssignCourseRequestValidator>();
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { };