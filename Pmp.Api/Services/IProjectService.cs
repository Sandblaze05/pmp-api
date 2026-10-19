using Pmp.Api.Contracts;
using Pmp.Api.Models;

namespace Pmp.Api.Services;

public interface IProjectService
{
    Task<ProjectListResponse> GetAllAsync(
        int page,
        int pageSize,
        ProjectStatus? status,
        CancellationToken cancellationToken);

    Task<ProjectResponse?> GetByIdAsync(string projectId, CancellationToken cancellationToken);

    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken);
    
    Task<ProjectResponse?> UpdateAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken);
    
    Task<ProjectResponse?> UpdateStatusAsync(string projectId, UpdateProjectStatusRequest request, CancellationToken cancellationToken);
}