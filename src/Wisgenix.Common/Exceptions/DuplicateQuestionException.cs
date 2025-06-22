using System;

namespace Wisgenix.Common.Exceptions
{
    public class DuplicateQuestionException : Exception
    {
        public DuplicateQuestionException(string questionText, int topicId)
            : base($"A question with the same text already exists for topic ID {topicId}.")
        {
        }
    }
}
