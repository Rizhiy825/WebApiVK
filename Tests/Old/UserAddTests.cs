using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApiVK;
using WebApiVK.Domain;
using Microsoft.SqlServer;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using FluentAssertions;

namespace Tests.Old;

public class UserAddTests : UsersApiTestsBase, IClassFixture<WebApplicationFactory<StartUp>>
{
    private readonly WebApplicationFactory<StartUp> _factory;

    public UserAddTests(WebApplicationFactory<StartUp> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<UsersContext>));

                services.Remove(descriptor);

                services.AddDbContext<UsersContext>(options =>
                {
                    options.UseSqlite("Data Source=:memory:");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<UsersContext>();

                    db.Database.Migrate();
                }
            });
        });
    }

    [Fact]
    public void Code_WhenAddSameLogin()
    {
        var client = _factory.CreateClient();

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

        var response = client.Send(request);

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

        var secondResponse = client.Send(request);


        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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