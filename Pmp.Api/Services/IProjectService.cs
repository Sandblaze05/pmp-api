using Pmp.Api.Contracts;

namespace Pmp.Api.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProjectResponse?> GetByIdAsync(string projectId, CancellationToken cancellationToken);

    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken);
    
    Task<ProjectResponse?> UpdateAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken);
    
    Task<ProjectResponse?> UpdateStatusAsync(string projectId, UpdateProjectStatusRequest request, CancellationToken cancellationToken);
}