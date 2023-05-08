using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests;

public static class Extensions
{
    public static ByteArrayContent SerializeToJsonContent(this object obj,
        string contentType = "application/json")
    {
        string json = JsonConvert.SerializeObject(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    public static JToken ReadContentAsJson(this HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync().Result;
        return JToken.Parse(content);
    }
}