namespace WebApiVK.Models;

public class UserToReturnDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public DateTime Created { get; set; }
    public UserGroup Group { get; set; }
    public UserState State { get; set; }
}