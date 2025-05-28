using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Tweet
    {
    
        public int Id { get; set; }

    
        public string Content { get; set; }

  
        public DateTime CreatedAt { get; set; }

        
        public int UserId { get; set; }
      
        public User User { get; set; }

    
        public ICollection<Like> Likes { get; set; }
        public ICollection<TweetHashtag> TweetHashtags { get; set; }
    }
}
