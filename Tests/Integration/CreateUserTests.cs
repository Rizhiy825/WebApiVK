using FluentAssertions;
using System.Net;
using WebApiVK.Domain;
using WebApiVK.Models;

namespace Tests.Integration
{
    public class CreateUserTests : UsersApiTestsBase
    {
        [Fact]
        public void Code201_WhenNotAuthorizedUserTriesToCreate()
        {
            var request = PrepareHttpRequestMessage();

            var userLogin = DateTime.Now.ToString("MM/dd/yyyy:H/mm/ss");
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

            response.RemoveProperties("id", "created")
                .ShouldHaveJsonContentEquivalentTo(new
                {
                    login = "commonUser1",
                    group = new UserGroup(GroupType.User, "Limited access"),
                    state = new UserState(StateType.Active, "Account is active")
                });
        }

        [Fact]
        public void Code201_WhenAdminCreateNewUser()
        {
            var request = PrepareHttpRequestMessage();
            
            var userLogin = DateTime.Now.ToString("MM/dd/yyyy:H/mm/ss");
            var userPassword = "qwerty";

            var convertedLogin = ConvertToBase64(userLogin);
            var convertedPassword = ConvertToBase64(userPassword);

            RegisterAuthHeader(request.Headers, "admin", "admin");

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
            var userLogin = "user";
            var userPassword = "qwerty";

            var task = CreateUser(userLogin, userPassword);
            task.Wait();

            var request = PrepareHttpRequestMessage();

            var convertedLogin = ConvertToBase64(DateTime.Now.ToString("MM/dd/yyyy:H/mm/ss"));
            var convertedPassword = ConvertToBase64(userPassword);

            RegisterAuthHeader(request.Headers, userLogin, userPassword);

            request.Content = new
            {
                login = convertedLogin,
                password = convertedPassword
            }.SerializeToJsonContent();

            var response = httpClient.Send(request);

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
    }
}