using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using WebApiVK.Domain;

namespace Tests.Old;

public class GetUserByIdTests : UsersApiTestsBase
{
    [Fact]
    public void Code200_WhenAllIsFine()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildUsersByLoginUri("admin");
        request.Headers.Add("Accept", "application/json");

        var userLogin = "admin";
        var userPassword = "admin";

        var base64Line = ConvertToBase64AuthLine(userLogin, userPassword);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", base64Line);

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