using System.Net;
using FluentAssertions;
using WebApiVK.Domain;
using WebApiVK.Models;

namespace Tests.Integration;

public class GetUserByLoginTests : UsersApiTestsBase
{
    [Fact]
    public void Code200_WhenAllIsFine()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildUsersByLoginUri("admin");
        request.Headers.Add("Accept", "application/json");
        
        RegisterAuthHeader(request.Headers, "admin", "admin");

        var response = httpClient.Send(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");

        response.RemoveProperties("id", "created")
            .ShouldHaveJsonContentEquivalentTo(new
            {
                login = "Admin",
                group = new UserGroup(GroupType.Admin, "Full access"),
                state = new UserState(StateType.Active, "Account is active")
            });
    }
}