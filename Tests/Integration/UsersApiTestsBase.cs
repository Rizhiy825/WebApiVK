using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace Tests.Integration;

public abstract class UsersApiTestsBase
{
    protected readonly HttpClient httpClient = new ();

    protected Uri BuildUsersUri()
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl);
        uriBuilder.Path = $"/api/users";
        return uriBuilder.Uri;
    }

    protected Uri BuildUsersByLoginUri(string userId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl);
        uriBuilder.Path = $"/api/users/{HttpUtility.UrlEncode(userId)}";
        return uriBuilder.Uri;
    }

    protected Uri BuildUsersWithPagesUri(int? pageNumber, int? pageSize)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (pageNumber.HasValue)
            query.Add("pageNumber", pageNumber.Value.ToString());
        if (pageSize.HasValue)
            query.Add("pageSize", pageSize.Value.ToString());

        var uriBuilder = new UriBuilder(Configuration.BaseUrl);
        uriBuilder.Path = $"/api/users";
        uriBuilder.Query = query.ToString();
        return uriBuilder.Uri;
    }

    protected string ConvertToBase64AuthLine(string userLogin, string userPassword)
    {
        var authLine = $"{userLogin}:{userPassword}";
        var base64Line = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(authLine));

        return base64Line;
    }

    protected string ConvertToBase64(string lineToConvert)
    {
        var base64Line = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(lineToConvert));

        return base64Line;
    }

    protected void CheckUserCreated(string createdUserId, string createdUserUri, object expectedUser)
    {
        // Проверка, что идентификатор созданного пользователя возвращается в теле ответа
        CheckUser(createdUserId, expectedUser);

        // Проверка, что ссылка на созданного пользователя возвращается в заголовке Location
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri(createdUserUri);
        request.Headers.Add("Accept", "application/json");
        var response = httpClient.Send(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        response.ReadContentAsJsonObj().ShouldHaveJsonContentEquivalentTo(expectedUser);
    }
    protected async Task<string> CreateUser(string login, string password)
    {
        var request = PrepareHttpRequestMessage();

        var convertedLogin = ConvertToBase64(login);
        var convertedPassword = ConvertToBase64(password);

        request.Content = new
        {
            login = convertedLogin,
            password = convertedPassword
        }.SerializeToJsonContent();

        RegisterAuthHeader(request.Headers, "admin", "admin");

        var response = await httpClient.SendAsync(request);

        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);

        return login;
    }

    protected void CheckUser(string userId, object expectedUser)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildUsersByLoginUri(userId);
        request.Headers.Add("Accept", "application/json");
        var response = httpClient.Send(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        response.ReadContentAsJsonObj().ShouldHaveJsonContentEquivalentTo(expectedUser);
    }

    protected HttpRequestMessage PrepareHttpRequestMessage()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = BuildUsersUri();
        request.Headers.Add("Accept", "application/json");
        return request;
    }

    protected void RegisterAuthHeader(HttpRequestHeaders headers, string userLogin, string userPassword)
    {
        var base64Line = ConvertToBase64AuthLine(userLogin, userPassword);
        headers.Authorization =
            new AuthenticationHeaderValue("Basic", base64Line);
    }
}