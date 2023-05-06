using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Tests;

public class GetUserById : UsersApiTestsBase
{
    [Fact]
    public void Code201_WhenAllIsFine()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildUsersByIdUri("77777777-7777-7777-7777-777777777777");
        request.Headers.Add("Accept", "application/json");

        var userLogin = "admin";
        var userPassword = "admin";

        var authLine = $"{userLogin}:{userPassword}";
        var base64Line = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(authLine));

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", base64Line);

        var response = httpClient.Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}