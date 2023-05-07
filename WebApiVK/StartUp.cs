using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
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
        services.AddDbContext<UsersContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUsersRepository, NpgsqlUsersRepository>();

        services.AddScoped<IEncryptor, EncryptorSha256>();
        services.AddScoped<ICoder, Base64Coder>();
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
        }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserToCreateDtoValidator>());
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<UserToAuthDto, UserEntity>();
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