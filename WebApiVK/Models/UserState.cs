using WebApiVK.Domain;

namespace WebApiVK.Models;

public class UserState
{
    public int Id { get; set; }
    public StateType Code { get; set; }
    public string? Description { get; set; }
    public UserState() { }
    public UserState(StateType state, string? description)
    {
        Code = state;
        Description = description;
    }
}