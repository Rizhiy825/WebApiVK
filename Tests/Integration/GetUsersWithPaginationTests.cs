using FluentAssertions;
using Newtonsoft.Json;

namespace Tests.Integration;

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

        RegisterAuthHeader(request.Headers, "admin", "admin");
        
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
            tasks[i] = CreateUser($"login_{i}", $"password_{i}");
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