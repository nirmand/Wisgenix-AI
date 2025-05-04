namespace AIUpskillingPlatform.Common.Exceptions;

public class QuestionNotFoundException : Exception
{
    public QuestionNotFoundException(int id) : base($"Question with ID {id} was not found")
    {
    }
} 