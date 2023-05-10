using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Tests.Old;

public class GetUsersWithPaginationTests : UsersApiTestsBase
{
    [Fact]
    public void Code200_WhenSelectSecondPage()
    {
        CreateUniqueUsers(30);

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

        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");

        var paginationString = response.GetRequiredHeader("X-Pagination").SingleOrDefault();
        var pagination = JsonConvert.DeserializeObject<Pagination>(paginationString);

        pagination.PreviousPageLink.Should().NotBeNull();
        pagination.NextPageLink.Should().NotBeNull();
        pagination.TotalCount.Should().BeGreaterThan(0);
        pagination.PageSize.Should().Be(10);
        pagination.CurrentPage.Should().Be(2);
        pagination.TotalPages.Should().BeGreaterThan(0);
    }

    private void CreateUniqueUsers(int count)
    {
        var tasks = new Task<string>[count];

        for (var i = 0; i < count; i++)
        {
            tasks[i] = CreateUser(new
            {
                login = ConvertToBase64($"login_{i}"),
                password = ConvertToBase64($"password_{i}")
            });
        }

        Task.WaitAll(tasks);
    }

    private class Pagination
    {
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        public int? TotalCount { get; set; }
        public int? PageSize { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
    }
}