using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace A.UI.Service;

[XmlRoot("root")]
public class TaskConfig
{
    public static TaskConfig Load(TextReader reader)
    {
        XmlSerializer serializer = new(typeof(TaskConfig));
        TaskConfig config = (TaskConfig)serializer.Deserialize(reader);

        return config;
    }

    [XmlElement("shopRow")]
    public ShopRow ShopRow { get; set; }

    [XmlElement("schedulerProcessName")]
    public string SchedulerProcessName { get; set; }

    [XmlArray("tasks")]
    [XmlArrayItem("task")]
    public TaskItem[] Tasks { get; set; }
}

public sealed class ShopRow
{

}

public sealed class TaskItem
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("localPath")]
    public string LocalPath { get; set; }

    [XmlAttribute("args")]
    public string Arguments { get; set; }

    [XmlAttribute("number")]
    public int Number { get; set; }

    [XmlAttribute("shouldWait")]
    public bool ShouldWait { get; set; }

    [XmlAttribute("runNextOnFailed")]
    public bool RunNextOnFailed { get; set; }    
}
