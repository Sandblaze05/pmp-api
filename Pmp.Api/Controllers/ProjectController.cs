using Microsoft.AspNetCore.Mvc;
using Pmp.Api.Contracts;
using Pmp.Api.Models;
using Pmp.Api.Services;

namespace Pmp.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProjectListResponse>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid page",
                detail: "The page must be at least 1.");
        }

        return Ok(await service.GetAllAsync(page, 10, status, cancellationToken));
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectResponse>> GetById(
        string projectId,
        CancellationToken cancellationToken)
    {
        var project = await service.GetByIdAsync(projectId, cancellationToken);

        return project is null
            ? Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found",
                detail: $"No project exists with ID '{projectId}'.")
            : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await service.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId = project.ProjectId },
            project);
    }

    [HttpPut("{projectId}")]
    public async Task<ActionResult<ProjectResponse>> Update(
        string projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await service.UpdateAsync(projectId, request, cancellationToken);

        return project is null
            ? Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found",
                detail: $"No project exists with ID '{projectId}'.")
            : Ok(project);
    }

    [HttpPatch("{projectId}")]
    public async Task<ActionResult<ProjectResponse>> UpdateStatus(
        string projectId,
        UpdateProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        var project = await service.UpdateStatusAsync(projectId, request, cancellationToken);

        return project is null
            ? Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found",
                detail: $"No project exists with ID '{projectId}'.")
            : Ok(project);
    }
}