using System.ComponentModel.DataAnnotations.Schema;

public class UserFollow
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FolloweeId { get; set; }

    [ForeignKey("FollowerId")]
    [InverseProperty("Following")]
    public User Follower { get; set; } = null!;

    [ForeignKey("FolloweeId")]
    [InverseProperty("Followers")]
    public User Followee { get; set; } = null!;
}
