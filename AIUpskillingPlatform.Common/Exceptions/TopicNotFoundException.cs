namespace AIUpskillingPlatform.Common.Exceptions;

public class TopicNotFoundException : Exception
{
    public TopicNotFoundException(int id) : base($"Topic with ID {id} was not found.")
    {
    }
} 