using System;

namespace Wisgenix.Common.Exceptions
{
    public class DuplicateTopicException : Exception
    {
        public DuplicateTopicException(string topicName, int subjectId)
            : base($"A topic with the name '{topicName}' already exists for subject ID {subjectId}.")
        {
        }
    }
}
