using System.Text.Json;
using System.Text.Json.Serialization;
using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model.Input
{
    public class RequestParams : IValueObject
    {
        public Dictionary<string, string?> Data { get; private set; } = new();

        public RequestParams() { }

        public RequestParams(Dictionary<string, string?> data)
        {
            Data = data ?? new Dictionary<string, string?>();
        }

        public static RequestParams Empty()
        {
            return new RequestParams();
        }
        
        public static RequestParams With(string key, string value)
        {
            return new RequestParams(new Dictionary<string, string?> { { key, value } });
        }
        
        public static RequestParams FromJson(string json)
        {
            return new RequestParams(JsonSerializer.Deserialize<Dictionary<string, string?>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Dictionary<string, string?>());
        }
        
        public override string ToString() => JsonSerializer.Serialize(Data, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    }
}
