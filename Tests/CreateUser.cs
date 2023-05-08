using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Tests
{
    public class CreateUser : UsersApiTestsBase
    {
        [Fact]
        public void Code201_WhenNotAuthorizedUser()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = BuildUsersUri();
            request.Headers.Add("Accept", "application/json");

            var userLogin = "primer3";
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
        }

        [Fact]
        public void Code201_WhenAdminCreateNewUser()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = BuildUsersUri();
            request.Headers.Add("Accept", "application/json");

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
        public void Code403_WhenUserHasAlreadyAuthenticated()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = BuildUsersUri();
            request.Headers.Add("Accept", "application/json");

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
           
        }
    }
}