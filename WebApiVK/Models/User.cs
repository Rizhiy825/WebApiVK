using System.Text.Json.Serialization;

namespace WebApiVK.Models;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public DateTime Created { get; set; }
    public UserGroup Group { get; set; }
    public UserState State { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
}