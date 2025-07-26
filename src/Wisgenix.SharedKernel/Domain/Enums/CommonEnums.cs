namespace Wisgenix.SharedKernel.Domain.Enums;

/// <summary>
/// Experience levels for users
/// </summary>
public enum ExperienceLevel
{
    Junior = 1,
    Mid = 2,
    Senior = 3
}

/// <summary>
/// Source of question generation
/// </summary>
public enum QuestionSource
{
    AI = 1,
    Manual = 2,
    Import = 3
}

/// <summary>
/// Assessment status
/// </summary>
public enum AssessmentStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}
