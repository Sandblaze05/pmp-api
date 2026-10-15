using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Pmp.Api.Data;
using Pmp.Api.Contracts;
using Pmp.Api.Models;
using Pmp.Api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("PmpDatabase")));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapGet("/", () =>
{
    return "hello";
});

app.MapGet("/api/projects", async (IProjectService service, CancellationToken cancellationToken) =>
{
    var projects = await service.GetAllAsync(cancellationToken);
    return Results.Ok(projects);
});

app.MapGet("/api/projects/{projectId}", async (string projectId, IProjectService service, CancellationToken cancellationToken) =>
{
    var project = await service.GetByIdAsync(projectId, cancellationToken);

    return project is null
        ? Results.NotFound()
        : Results.Ok(project);
});

app.MapPost("/api/projects", async (CreateProjectRequest request, IProjectService service, CancellationToken cancellationToken) =>
{
    var project = await service.CreateAsync(request, cancellationToken);

    return Results.Created(
        $"/api/projects/{project.ProjectId}",
        project
    );
});

app.MapPut("/api/projects/{projectId}", async (string projectId, UpdateProjectRequest request, IProjectService service, CancellationToken cancellationToken) =>
{
    var project = await service.UpdateAsync(projectId, request, cancellationToken);

    return project is null
        ? Results.NotFound()
        : Results.Ok(project);
});

app.MapPatch("/api/projects/{projectId}", async (string projectId, UpdateProjectStatusRequest request, IProjectService service, CancellationToken cancellationToken) =>
{
    var project = await service.UpdateStatusAsync(projectId, request, cancellationToken);

    return project is null
        ? Results.NotFound()
        : Results.Ok(project);
});

app.Run();