using System.Net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApiVK.Authorization;
using WebApiVK.Domain;
using WebApiVK.Interfaces;
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
        // Эти две строки подключают контекст БД postgre. Для работы с настоящей БД раскомментируй
        

        // Это контекст БД для тестов. Это имитация БД, так что для работы с настоящей БД закомментируй
        services.AddDbContext<UsersContext>(options =>
            //options.UseInMemoryDatabase("Default Base"));
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // TODO можно ли прокинуть зависимость DbContext вместо создания TestRepository?
        // БД postgre. Для работы с настоящей БД раскомментируй
        services.AddScoped<IUsersRepository, UsersRepository>();

        // БД для тестов. Для работы с настоящей БД закомментируй
        //services.AddScoped<IUsersRepository, TestRepository>();

        services.AddScoped<IEncryptor, EncryptorSha256>();
        services.AddScoped<ICoder, Base64Coder>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IDateTimeRecorder, DateTimeRecorder>();
        services.AddSingleton<ILoginsManager, LoginQueueManager>();

        // Внедрение реализации Basic-авторизации
        services.AddAuthentication("BasicAuthentication").
            AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);

        services.AddAuthorization();

        services.AddControllers(options =>
        {
            // Отвечаем 406 Not Acceptable на запросы неизвестных форматов
            options.ReturnHttpNotAcceptable = true;
        }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserToCreateDtoValidator>()); ;

        //services.AddFluentValidationAutoValidation();
        //services.AddFluentValidationClientsideAdapters();
        //services.AddValidatorsFromAssemblyContaining<UserToCreateDtoValidator>();
        
        
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<UserToAuth, UserEntity>();
            cfg.CreateMap<UserToCreateDto, UserEntity>();
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