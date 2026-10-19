using Pmp.Api.Models;

namespace Pmp.Api.Contracts;

public sealed record ProjectWizardDetails(
    string CustomerAddress,
    string ProjectSector,
    string ClientContactName,
    string ClientEmail,
    string ClientPmName,
    string ClientPmEmail,
    string ProjectDescription,
    string Scope,
    string ManagedBy,
    string Domain,
    string Type,
    decimal? TotalValue,
    string Methodology,
    string Technology,
    string Lead,
    string Stakeholders
);

public sealed record ProjectTask(
    string Name,
    string Description
);

public sealed record ProjectEffort(
    string Person,
    string TaskName,
    decimal EstimatedHours
);

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
    string AccountManagerName,
    ProjectWizardDetails WizardDetails,
    IReadOnlyList<ProjectTask> Tasks,
    IReadOnlyList<ProjectEffort> Efforts
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
    string AccountManagerName,
    ProjectWizardDetails WizardDetails,
    IReadOnlyList<ProjectTask> Tasks,
    IReadOnlyList<ProjectEffort> Efforts
);

public sealed record UpdateProjectStatusRequest(
    ProjectStatus Status
);

public sealed record ProjectListResponse(
    IReadOnlyList<ProjectResponse> Items,
    bool HasNextPage,
    int TotalCount
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
    ProjectStatus Status,
    ProjectWizardDetails WizardDetails,
    IReadOnlyList<ProjectTask> Tasks,
    IReadOnlyList<ProjectEffort> Efforts
);