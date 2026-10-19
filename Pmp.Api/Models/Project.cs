namespace Pmp.Api.Models;

public enum ProjectStatus
{
    InProgress = 0,
    PendingApproval = 1,
    Completed = 3
}

public sealed class Project
{
    public required string ProjectId { get; set; }

    public required string CustomerName { get; set; }
    public required string ProjectName { get; set; }

    public DateTimeOffset ProjectStartDate { get; set; }
    public DateTimeOffset ProjectEndDate { get; set; }
    public DateTimeOffset? RevisedEndDate { get; set; }
    public DateTimeOffset? RenewalDate { get; set; }
    public DateTimeOffset ProjectIssueDate { get; set; }

    public decimal EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }

    public required string OnshoreManagerName { get; set; }
    public required string OffshoreManagerName { get; set; }
    public required string AccountManagerName { get; set; }

    public required string WizardDetailsJson { get; set; }
    public required string TasksJson { get; set; }
    public required string EffortsJson { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.PendingApproval;

}