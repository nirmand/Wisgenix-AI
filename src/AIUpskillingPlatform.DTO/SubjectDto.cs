using System;

namespace AIUpskillingPlatform.DTO;

public abstract class WriteSubjectBaseDto
{
    public string SubjectName { get; set; } = string.Empty;
}


public class CreateSubjectDto: WriteSubjectBaseDto
{
}

public class UpdateSubjectDto: WriteSubjectBaseDto    
{
}

public class SubjectDto
{
    public int ID { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public ICollection<TopicDto> Topics { get; set; } = new List<TopicDto>();
} 