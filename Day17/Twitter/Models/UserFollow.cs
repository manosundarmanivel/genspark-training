using System.ComponentModel.DataAnnotations.Schema;

public class UserFollow
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FolloweeId { get; set; }

   
    public User Follower { get; set; } = null!;

    
    public User Followee { get; set; } = null!;
}
