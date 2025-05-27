using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitterApp.Models;

public class User
{
    
    public int Id { get; set; }

   
    public string Username { get; set; } = null!;

    
    public string Email { get; set; } = null!;


    [InverseProperty("Followee")]
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();

   
    [InverseProperty("Follower")]
    public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
}
