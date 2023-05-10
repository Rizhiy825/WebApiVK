using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Tests.Old
{
    public class CreateUser : UsersApiTestsBase
    {
        [Fact]
        public void Code201_WhenNotAuthorizedUserTriesToCreate()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "commonUser1";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUserId = response.ReadContentAsJsonToken().ToString();

            createdUserId.Should().NotBeNullOrEmpty();
            var createdUserUri = response.GetRequiredHeader("Location").SingleOrDefault();
            createdUserUri.Should().NotBeNullOrEmpty();

            CheckUserCreated(createdUserId, createdUserUri, new
            {
                id = createdUserId,
                login = "commonUser",
                groupType = "User",
                stateType = "Active"
            });
        }

        [Fact]
        public void Code201_WhenAdminCreateNewUser()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "primer5";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            var base64Line = ConvertToBase64AuthLine("admin", "admin");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public void Code401_WhenUserPasswordWrong()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "primer5";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            var base64Line = ConvertToBase64AuthLine("admin", "wrong password");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.SendAsync(request);
            var res = response.Result;

            var body = res.ReadContentAsJsonToken();
            var str = body.ToString();

        }

        [Fact]
        public void Code403_WhenUserHasAlreadyAuthenticated()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "primer";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            var base64Line = ConvertToBase64AuthLine(userLogin, userPassword);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Code_WhenAddSameLogin()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "primer";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            var base64Line = ConvertToBase64AuthLine(userLogin, userPassword);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

            request = PrepareHttpRequestMessage();

            convertedLogin = ConvertToBase64(userLogin);
            convertedPassword = ConvertToBase64(userPassword);

            base64Line = ConvertToBase64AuthLine(userLogin, userPassword);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var secondResponse = httpClient.Send(request);


            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }


        [Fact]
        public void Code400_WhenNotValidLogin()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = "a";
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

            response.IsSuccessStatusCode.Should().Be(false);

            var errorMessage = response
                .ReadContentAsJsonObj()
                .GetPropertyValue("errors");

            errorMessage.Should().Be("Login must be between 4 and 30 characters.");

        }

        public HttpRequestMessage PrepareHttpRequestMessage()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = BuildUsersUri();
            request.Headers.Add("Accept", "application/json");
            return request;
        }
    }
}