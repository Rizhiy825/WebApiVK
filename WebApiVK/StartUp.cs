using Microsoft.AspNetCore.Authentication;
using WebApiVK.Authorization;
using WebApiVK.Models;

namespace WebApiVK;

public class StartUp
{
    public IConfiguration Configuration { get; }

    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        // Внедрение реализации Basic-авторизации
        services.AddAuthentication("BasicAuthentication").
            AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);
        
        services.AddAuthorization();

        services.AddControllers(options =>
        {
            // Отвечаем 406 Not Acceptable на запросы неизвестных форматов
            options.ReturnHttpNotAcceptable = true;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<UserForAuthDTO, User>();
        }, new System.Reflection.Assembly[0]);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}