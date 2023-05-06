using System.Net.Http.Headers;

namespace Tests
{
    public class CreateUser : UsersApiTestsBase
    {
        [Fact]
        public void Code201_WhenAllIsFine()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = BuildUsersUri();
            request.Headers.Add("Accept", "application/json");

            var userLogin = "primer";
            var userPassword = "qwerty";

            var authLine = $"{userLogin}:{userPassword}";
            var base64Line = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes(authLine));

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", base64Line);

        }
    }
}