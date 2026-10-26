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
        var _page = Math.Max(page, 1);
        var _pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Projects
            .AsNoTracking()
            .Where(project => status == null || project.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .OrderBy(project => project.ProjectName)
            .ThenBy(project => project.ProjectId)
            .Skip((_page - 1) * pageSize)
            .Take(_pageSize + 1)
            .ToListAsync(cancellationToken);

        return new ProjectListResponse(
            projects.Take(_pageSize).Select(ToResponse).ToList(),
            projects.Count > _pageSize,
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
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var counter = db.ProjectsIdCounters.Where(c => c.Id == 1);

        await counter.ExecuteUpdateAsync(
            setters => setters.SetProperty(
                c => c.LastNumber,
                c => c.LastNumber + 1
            ),
            cancellationToken
        );

        var nextNumber = await counter.Select(c => c.LastNumber).SingleAsync(cancellationToken);

        var project = new Project
        {
            ProjectId = $"SVAM_PROJECT_{nextNumber:D3}",
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
        await transaction.CommitAsync(cancellationToken);

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

    public async Task<ProjectListResponse> SearchAsync(int page, int pageSize, ProjectSearchFilters filters, CancellationToken cancellationToken)
    {
        IQueryable<Project> query = db.Projects;

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var term = filters.SearchTerm.Trim();

            query = query.Where(p => 
                p.ProjectId.Contains(term) ||
                p.ProjectName.Contains(term) ||
                p.CustomerName.Contains(term) ||
                p.OnshoreManagerName.Contains(term) ||
                p.OffshoreManagerName.Contains(term) ||
                p.AccountManagerName.Contains(term)
            );
        }

        if (filters.Status is not null)
        {
            query = query.Where(p => p.Status == filters.Status);
        }

        if (filters.StartDateFrom is not null)
        {
            var startDate = filters.StartDateFrom.Value;
            query = query.Where(p => p.ProjectStartDate >= startDate);
        }

        if (filters.StartDateTo is not null)
        {
            var endDate = filters.StartDateTo.Value;
            query = query.Where(p => p.ProjectStartDate <= endDate);
        }

        if (!string.IsNullOrWhiteSpace(filters.CustomerName))
        {
            var customer = filters.CustomerName.Trim();
            query = query.Where(p => p.CustomerName.Contains(customer));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var _page = Math.Max(page, 1);
        var _pageSize = Math.Clamp(pageSize, 1, 100);

        var projects = await query
            .OrderBy(p => p.ProjectName)
            .Skip((_page - 1) * _pageSize)
            .Take(_pageSize + 1)
            .ToListAsync(cancellationToken);
        
        var hasMore = projects.Count > _pageSize;

        var items = projects
            .Take(_pageSize)
            .Select(ToResponse)
            .ToList();

        return new ProjectListResponse(items, hasMore, totalCount);
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

    // private async Task<string> GenerateProjectIdAsync(CancellationToken cancellationToken)
    // {
    //     var ids = await db.Projects
    //         .Where(project => project.ProjectId.StartsWith("SVAM_PROJECT_"))
    //         .Select(project => project.ProjectId)
    //         .ToListAsync(cancellationToken);

    //     var nextNumber = ids
    //         .Select(id => int.TryParse(id["SVAM_PROJECT_".Length..], out var number) ? number : 0)
    //         .DefaultIfEmpty()
    //         .Max() + 1;

    //     return $"SVAM_PROJECT_{nextNumber:D3}";
    // }

    private static ProjectWizardDetails EmptyWizardDetails() =>
        new("", ProjectSector.Default, "", "", "", "", "", "", "", "", "", null, "", "", "", "");

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
