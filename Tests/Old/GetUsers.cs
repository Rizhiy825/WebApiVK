using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace Tests.Old;

public class GetUsers : UsersApiTestsBase
{
    [Fact]
    public void Code200_WhenSecondPage()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildUsersWithPagesUri(2, 10);
        request.Headers.Add("Accept", "*/*");

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