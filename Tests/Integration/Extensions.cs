using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests.Integration;

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

    public static JToken ReadContentAsJsonToken(this HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync().Result;
        return JToken.Parse(content);
    }

    public static JObject ReadContentAsJsonObj(this HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync().Result;
        return JObject.Parse(content);
    }

    public static string GetPropertyValue(this JObject token, string propName)
    {
        foreach (var property in token.Properties())
        {
            if (property.Name == propName)
            {
                var first = property.Value.First;
                var errorMessage = first.First.ToString();
                var messageStart = errorMessage.IndexOf('"');
                var messageEnd = errorMessage.LastIndexOf('"');
                return errorMessage.Substring(messageStart + 1, messageEnd - messageStart - 1);
            }
        }

        return null;
    }

    public static string[] GetRequiredHeader(this HttpResponseMessage response, string headerName)
    {
        var hasResponseHeader = response.Headers.TryGetValues(headerName, out var responseHeaderValues);
        var hasContentHeader = response.Content.Headers.TryGetValues(headerName, out var contentHeaderValues);

        if (hasResponseHeader && hasContentHeader)
            Assert.Fail($"Should have only one '{headerName}' header");

        if (hasResponseHeader)
        {
            return responseHeaderValues.ToArray();
        }
        else if (hasContentHeader)
        {
            return contentHeaderValues.ToArray();
        }
        Assert.Fail($"Should have '{headerName}' header");
        return null;
    }

    public static void ShouldHaveHeader(this HttpResponseMessage response, string headerName, string headerValue)
    {
        var actualHeaderValue = response.GetRequiredHeader(headerName);
        actualHeaderValue.Should().BeEquivalentTo(headerValue);
    }

    public static void ShouldNotHaveHeader(this HttpResponseMessage response, string headerName)
    {
        var hasResponseHeader = response.Headers.TryGetValues(headerName, out var _);
        var hasContentHeader = response.Content.Headers.TryGetValues(headerName, out var _);
        var hasHeader = hasResponseHeader || hasContentHeader;

        hasHeader.Should().BeFalse();
    }

    public static JObject RemoveProperties(this HttpResponseMessage response, params string[] properties)
    {
        var content = response.ReadContentAsJsonObj();

        foreach (var property in properties)
        {
            content.Remove(property);
        }

        return content;
    }

    public static void ShouldHaveJsonContentEquivalentTo(this JObject content, object expected)
    {
        content.Should().BeEquivalentTo(JObject.FromObject(expected));
    }


}