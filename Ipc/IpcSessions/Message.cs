using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using A.TaskDispatching;

namespace IpcSessions;

public abstract class Message(string fromProcess, string toProcess)
{
    public static T Load<T>(string content)
        where T : Message => JsonConvert.DeserializeObject<T>(content);

    public DateTimeOffset Timestamp { get; set; } = MinashiDateTime.Now;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string From { get; set; } = fromProcess;

    public string To { get; set; } = toProcess;

    public abstract string Action { get; }

    public string ToJson() =>
        JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
}
