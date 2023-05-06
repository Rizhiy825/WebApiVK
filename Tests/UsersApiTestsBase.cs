﻿using System.Web;

namespace Tests;

public abstract class UsersApiTestsBase
{
    protected readonly HttpClient httpClient = new HttpClient();

    protected Uri BuildUsersUri()
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl);
        uriBuilder.Path = $"/api/users";
        return uriBuilder.Uri;
    }

    protected Uri BuildUsersByIdUri(string userId)
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


}