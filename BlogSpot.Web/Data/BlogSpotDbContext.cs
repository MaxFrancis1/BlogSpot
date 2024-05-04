using BlogSpot.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogSpot.Web.Data
{
    public class BlogSpotDbContext : DbContext
    {
        public BlogSpotDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
