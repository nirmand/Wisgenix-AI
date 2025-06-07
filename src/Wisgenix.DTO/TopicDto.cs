namespace Wisgenix.DTO;

public abstract class WriteTopicBaseDto
{
    public string TopicName { get; set; }

    public int SubjectID { get; set; }   
}

public class CreateTopicDto: WriteTopicBaseDto
{
}

public class UpdateTopicDto: WriteTopicBaseDto
{
}

public class TopicDto
{
    public int ID { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int SubjectID { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}
