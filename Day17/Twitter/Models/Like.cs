using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TwitterApp.Models
{
    public class Like
    {

        public int Id { get; set; }


        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;


        public int TweetId { get; set; }
        [ForeignKey("TweetId")]
        public Tweet Tweet { get; set; } = null!;
    }
}