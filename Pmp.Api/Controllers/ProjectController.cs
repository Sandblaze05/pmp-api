using Microsoft.AspNetCore.Mvc;
using Pmp.Api.Contracts;
using Pmp.Api.Services;

namespace Pmp.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var projects = await service.GetAllAsync(cancellationToken);

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectResponse>> GetById(
        string projectId,
        CancellationToken cancellationToken)
    {
        var project = await service.GetByIdAsync(projectId, cancellationToken);

        return project is null
            ? NotFound()
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
            ? NotFound()
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
            ? NotFound()
            : Ok(project);
    }
}