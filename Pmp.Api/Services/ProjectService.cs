using Microsoft.EntityFrameworkCore;
using Pmp.Api.Contracts;
using Pmp.Api.Data;
using Pmp.Api.Models;

namespace Pmp.Api.Services;

public sealed class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Projects
                       .AsNoTracking()
                       .OrderBy(project => project.ProjectName)
                       .Select(project => ToResponse(project))
                       .ToListAsync(cancellationToken);
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
            ProjectId = Guid.NewGuid().ToString(),
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
            project.Status
        );
    }
}
