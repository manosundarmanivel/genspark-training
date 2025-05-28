using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TwitterApp.Models
{
    public class TweetHashtag
    {

        public int Id { get; set; }


        public int TweetId { get; set; }
       
        public Tweet Tweet { get; set; }


        public int HashtagId { get; set; }
       
        public Hashtag Hashtag { get; set; }
    }
}