using Pmp.Api.Models;

namespace Pmp.Api.Contracts;

public sealed record CreateProjectRequest(
    string CustomerName,
    string ProjectName,
    DateTimeOffset ProjectStartDate,
    DateTimeOffset ProjectEndDate,
    DateTimeOffset? RevisedEndDate,
    DateTimeOffset? RenewalDate,
    DateTimeOffset ProjectIssueDate,
    decimal EstimatedHours,
    decimal? ActualHours,
    string OnshoreManagerName,
    string OffshoreManagerName,
    string AccountManagerName
);

public sealed record UpdateProjectRequest(
    string CustomerName,
    string ProjectName,
    DateTimeOffset ProjectStartDate,
    DateTimeOffset ProjectEndDate,
    DateTimeOffset? RevisedEndDate,
    DateTimeOffset? RenewalDate,
    DateTimeOffset ProjectIssueDate,
    decimal EstimatedHours,
    decimal? ActualHours,
    string OnshoreManagerName,
    string OffshoreManagerName,
    string AccountManagerName
);

public sealed record UpdateProjectStatusRequest(
    ProjectStatus Status
);

public sealed record ProjectResponse(
    string ProjectId,
    string CustomerName,
    string ProjectName,
    DateTimeOffset ProjectStartDate,
    DateTimeOffset ProjectEndDate,
    DateTimeOffset? RevisedEndDate,
    DateTimeOffset? RenewalDate,
    DateTimeOffset ProjectIssueDate,
    decimal EstimatedHours,
    decimal? ActualHours,
    string OnshoreManagerName,
    string OffshoreManagerName,
    string AccountManagerName,
    ProjectStatus Status
);