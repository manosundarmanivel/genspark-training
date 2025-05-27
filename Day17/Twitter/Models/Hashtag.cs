using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Hashtag
    {

        public int Id { get; set; }


        public string Tag { get; set; }

        public ICollection<TweetHashtag>? TweetHashtags { get; set; }
    }
}