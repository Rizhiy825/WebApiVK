﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebApiVK.Interfaces;
using WebApiVK.Authorization;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

namespace WebApiVK.Authorization;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService userService;
    private readonly ICoder coder;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService,
        ICoder coder)
        : base(options, logger, encoder, clock)
    {
        this.userService = userService;
        this.coder = coder;
        //this.events = events;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        UserToAuth user;

        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();
        
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentials = coder.DecodeCredentials(authHeader.Parameter);
        var username = credentials.Item1;
        var password = credentials.Item2;
        user = await userService.Authenticate(username, password);

        if (user == null)
        {
            // Завершить выполнение запроса с ошибкой
            return AuthenticateResult.Fail("Invalid username or password");
        }
        
        // В случае прохождения аутентификации выдаем Claim
        var claims = new[]
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Login", user.Login),
            new Claim(ClaimTypes.Role, "admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    //protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    //{
    //    var result = new { message = "Неверный логин или пароль" };
    //    await Response.WriteAsync(JsonConvert.SerializeObject(result));
    //    await base.HandleChallengeAsync(properties);
    //}

    // TODO сделай фильтрацию NoResult
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        

        Response.StatusCode = 403;
        var result = new { message = "Invalid username or password" };
        
        await Response.WriteAsync(JsonConvert.SerializeObject(result));
        await Task.CompletedTask;
    }
}