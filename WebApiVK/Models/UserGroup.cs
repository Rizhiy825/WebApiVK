using WebApiVK.Domain;

namespace WebApiVK.Models;

public class UserGroup
{
    public int Id { get; set; }
    public GroupType Code { get; set; }
    public string? Description { get; set; }
    public UserGroup() { }
    public UserGroup(GroupType group, string? description)
    {
        Code = group;
        Description = description;
    }
}