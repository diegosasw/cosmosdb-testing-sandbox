using System.Text.Json;

namespace Test.Common.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> ToPayload<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = Deserialize<T>(content);
        return result;
    }
    
    private static T Deserialize<T>(string json) => (T) Deserialize(json, typeof(T));
    
    private static object Deserialize(string json, Type destinationType)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var result =
            JsonSerializer.Deserialize(json, destinationType, jsonSerializerOptions)
            ?? throw new Exception($"Unable to deserialize json {json} into {destinationType}");
        return result;
    }
}
