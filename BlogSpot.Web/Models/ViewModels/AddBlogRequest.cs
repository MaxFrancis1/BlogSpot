using BlogSpot.Web.Models.Domain;

namespace BlogSpot.Web.Models.ViewModels
{
    public class AddBlogRequest
    {
        public string Heading { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string Author { get; set; }
        public Tag Tag { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
