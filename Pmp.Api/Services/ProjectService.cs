using Microsoft.EntityFrameworkCore;
using Pmp.Api.Contracts;
using Pmp.Api.Data;
using Pmp.Api.Models;
using System.Text.Json;

namespace Pmp.Api.Services;

public sealed class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<ProjectListResponse> GetAllAsync(
        int page,
        int pageSize,
        ProjectStatus? status,
        CancellationToken cancellationToken)
    {
        var query = db.Projects
            .AsNoTracking()
            .Where(project => status == null || project.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .OrderBy(project => project.ProjectName)
            .ThenBy(project => project.ProjectId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        return new ProjectListResponse(
            projects.Take(pageSize).Select(ToResponse).ToList(),
            projects.Count > pageSize,
            totalCount);
    }

    public async Task<ProjectResponse?> GetByIdAsync(string projectId, CancellationToken cancellationToken)
    {
        return await db.Projects
                       .AsNoTracking()
                       .Where(project => project.ProjectId == projectId)
                       .Select(project => ToResponse(project))
                       .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            ProjectId = await GenerateProjectIdAsync(cancellationToken),
            CustomerName = request.CustomerName,
            ProjectName = request.ProjectName,
            ProjectStartDate = request.ProjectStartDate,
            ProjectEndDate = request.ProjectEndDate,
            RevisedEndDate = request.RevisedEndDate,
            RenewalDate = request.RenewalDate,
            ProjectIssueDate = request.ProjectIssueDate,
            EstimatedHours = request.EstimatedHours,
            ActualHours = request.ActualHours,
            OnshoreManagerName = request.OnshoreManagerName,
            OffshoreManagerName = request.OffshoreManagerName,
            AccountManagerName = request.AccountManagerName,
            WizardDetailsJson = JsonSerializer.Serialize(request.WizardDetails),
            TasksJson = JsonSerializer.Serialize(request.Tasks),
            EffortsJson = JsonSerializer.Serialize(request.Efforts),
            Status = ProjectStatus.PendingApproval
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.SingleOrDefaultAsync(project => project.ProjectId == projectId, cancellationToken);

        if (project is null)
        {
            return null;
        }

        project.CustomerName = request.CustomerName;
        project.ProjectName = request.ProjectName;
        project.ProjectStartDate = request.ProjectStartDate;
        project.ProjectEndDate = request.ProjectEndDate;
        project.RevisedEndDate = request.RevisedEndDate;
        project.RenewalDate = request.RenewalDate;
        project.ProjectIssueDate = request.ProjectIssueDate;
        project.EstimatedHours = request.EstimatedHours;
        project.ActualHours = request.ActualHours;
        project.OnshoreManagerName = request.OnshoreManagerName;
        project.OffshoreManagerName = request.OffshoreManagerName;
        project.AccountManagerName = request.AccountManagerName;
        project.WizardDetailsJson = JsonSerializer.Serialize(request.WizardDetails);
        project.TasksJson = JsonSerializer.Serialize(request.Tasks);
        project.EffortsJson = JsonSerializer.Serialize(request.Efforts);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateStatusAsync(string projectId, UpdateProjectStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Status))
        {
            throw new ArgumentException("Invalid project status");
        }
        
        var project = await db.Projects.SingleOrDefaultAsync(project => project.ProjectId == projectId, cancellationToken);

        if (project is null)
        {
            return null;
        }

        project.Status = request.Status;

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(project);
    }
    
    private static ProjectResponse ToResponse(Project project)
    {
        return new ProjectResponse(
            project.ProjectId,
            project.CustomerName,
            project.ProjectName,
            project.ProjectStartDate,
            project.ProjectEndDate,
            project.RevisedEndDate,
            project.RenewalDate,
            project.ProjectIssueDate,
            project.EstimatedHours,
            project.ActualHours,
            project.OnshoreManagerName,
            project.OffshoreManagerName,
            project.AccountManagerName,
            project.Status,
            Deserialize(project.WizardDetailsJson, EmptyWizardDetails()),
            Deserialize(project.TasksJson, (IReadOnlyList<ProjectTask>)[]),
            Deserialize(project.EffortsJson, (IReadOnlyList<ProjectEffort>)[])
        );
    }

    private async Task<string> GenerateProjectIdAsync(CancellationToken cancellationToken)
    {
        var ids = await db.Projects
            .Where(project => project.ProjectId.StartsWith("SVAM_PROJECT_"))
            .Select(project => project.ProjectId)
            .ToListAsync(cancellationToken);

        var nextNumber = ids
            .Select(id => int.TryParse(id["SVAM_PROJECT_".Length..], out var number) ? number : 0)
            .DefaultIfEmpty()
            .Max() + 1;

        return $"SVAM_PROJECT_{nextNumber:D3}";
    }

    private static ProjectWizardDetails EmptyWizardDetails() =>
        new("", "", "", "", "", "", "", "", "", "", "", null, "", "", "", "");

    private static T Deserialize<T>(string json, T fallback)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return fallback;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json) ?? fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }
}
