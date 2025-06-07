using System;

namespace Wisgenix.Common.Exceptions;

public class SubjectNotFoundException: Exception
{
    public SubjectNotFoundException(int id) : base($"Subject with ID {id} was not found")
    {
    }
}