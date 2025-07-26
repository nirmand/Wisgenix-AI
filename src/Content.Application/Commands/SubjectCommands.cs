using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Commands;

/// <summary>
/// Command to create a new subject
/// </summary>
public class CreateSubjectCommand : ICommand<GetSubjectResponse>
{
    public string SubjectName { get; }

    public CreateSubjectCommand(string subjectName)
    {
        SubjectName = subjectName;
    }
}

/// <summary>
/// Command to update an existing subject
/// </summary>
public class UpdateSubjectCommand : ICommand<GetSubjectResponse>
{
    public int Id { get; }
    public string SubjectName { get; }

    public UpdateSubjectCommand(int id, string subjectName)
    {
        Id = id;
        SubjectName = subjectName;
    }
}

/// <summary>
/// Command to delete a subject
/// </summary>
public class DeleteSubjectCommand : ICommand
{
    public int Id { get; }

    public DeleteSubjectCommand(int id)
    {
        Id = id;
    }
}
