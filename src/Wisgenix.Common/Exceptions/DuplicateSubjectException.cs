using System;

namespace Wisgenix.Common.Exceptions
{
    public class DuplicateSubjectException : Exception
    {
        public DuplicateSubjectException(string subjectName)
            : base($"A subject with the name '{subjectName}' already exists.")
        {
        }
    }
}
