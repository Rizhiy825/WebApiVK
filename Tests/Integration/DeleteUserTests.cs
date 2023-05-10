using FluentAssertions;
using System.Net;

namespace Tests.Integration;

public class DeleteUserTests : UsersApiTestsBase
{
    [Fact]
    public async void Code204_WhenAdminBlockingActiveUser()
    {
        var createdUserLogin = await CreateUser(DateTime.Now.ToString("MM/dd/yyyy:H/mm/ss"), "password");

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Delete;
        request.RequestUri = BuildUsersByLoginUri(createdUserLogin);
        request.Headers.Add("Accept", "*/*");

        RegisterAuthHeader(request.Headers, "admin", "admin");

        var response = httpClient.Send(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Fact]
    public async void Code401_WhenNotAdminBlockingActiveUser()
    {
        var createdUserId = await CreateUser(DateTime.Now.ToString("MM/dd/yyyy:H/mm/ss"), "password");

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Delete;
        request.RequestUri = BuildUsersByLoginUri(createdUserId);
        request.Headers.Add("Accept", "*/*");
        var response = httpClient.Send(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}