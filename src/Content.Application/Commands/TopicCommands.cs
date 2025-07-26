using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Commands;

/// <summary>
/// Command to create a new topic
/// </summary>
public class CreateTopicCommand : ICommand<GetTopicResponse>
{
    public string TopicName { get; }
    public int SubjectId { get; }

    public CreateTopicCommand(string topicName, int subjectId)
    {
        TopicName = topicName;
        SubjectId = subjectId;
    }
}

/// <summary>
/// Command to update an existing topic
/// </summary>
public class UpdateTopicCommand : ICommand<GetTopicResponse>
{
    public int Id { get; }
    public string TopicName { get; }
    public int SubjectId { get; }

    public UpdateTopicCommand(int id, string topicName, int subjectId)
    {
        Id = id;
        TopicName = topicName;
        SubjectId = subjectId;
    }
}

/// <summary>
/// Command to delete a topic
/// </summary>
public class DeleteTopicCommand : ICommand
{
    public int Id { get; }

    public DeleteTopicCommand(int id)
    {
        Id = id;
    }
}
