using Newtonsoft.Json.Linq;

namespace IpcSessions;

internal static class MessageTypes
{
    public const string Text = "Text";

    public const string Progress = "Progress";

    public const string SessionCloseRequest = "SessionCloseRequest";

    public static string ProbMessageType(string message)
    {
        JToken token = JToken.Parse(message);

        if (token.Type != JTokenType.Object || 
            ((JObject)token).Properties().All(
                p => !string.Equals(p.Name, nameof(Message.Action), StringComparison.OrdinalIgnoreCase)
            ))
        {
            throw new FormatException();
        }

        return ((JObject)token)[nameof(Message.Action).ToLower()].Value<string>();
    }
}
