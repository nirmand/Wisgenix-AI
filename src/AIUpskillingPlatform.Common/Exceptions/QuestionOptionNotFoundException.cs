namespace AIUpskillingPlatform.Common.Exceptions;

public class QuestionOptionNotFoundException : Exception
{
    public QuestionOptionNotFoundException(int id) : base($"Question option with ID {id} was not found")
    {
    }
} 