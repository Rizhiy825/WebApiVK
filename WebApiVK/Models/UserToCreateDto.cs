using System.ComponentModel.DataAnnotations;

namespace WebApiVK.Models;

public class UserToCreateDto
{
    public string Login { get; set; }
    public string Password { get; set; }

    public UserToCreateDto(){}

    public UserToCreateDto(string login, string password)
    {
        Login = login;
        Password = password;
    }
}