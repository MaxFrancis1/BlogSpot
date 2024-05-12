using Azure.Core;
using BlogSpot.Web.Data;
using BlogSpot.Web.Models.Domain;
using BlogSpot.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Azure;

namespace BlogSpot.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogSpotDbContext _blogSpotDbContext;

        public BlogController(BlogSpotDbContext blogSpotDbContext)
        {
            this._blogSpotDbContext = blogSpotDbContext;
        }

        [HttpGet]
        public IActionResult BlogAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BlogAdd(AddBlogRequest addBlogRequest)
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            Tag foundTag = _blogSpotDbContext.Tags.Single(t => t.Name == addBlogRequest.Tag);

            if (foundTag.Id == Guid.Empty)
            {
                // Tag was not found
                ModelState.AddModelError("", "Submitted Tag was not found. Please enter a valid tag.");
                return View("BlogAdd");
            } else
            {
                var blog = new BlogPost
                {
                    Heading = addBlogRequest.Heading,
                    Content = addBlogRequest.Content,
                    ShortDescription = addBlogRequest.ShortDescription,
                    FeaturedImageUrl = addBlogRequest.FeaturedImageUrl,
                    Author = addBlogRequest.Author,
                    Tag = foundTag,
                    Visible = false,
                    PublishedDate = DateTime.Now,
                };

                try
                {
                    var exists = await _blogSpotDbContext.BlogPosts.AnyAsync(e => e.Heading == blog.Heading);

                    if (!exists)
                    {
                        await _blogSpotDbContext.BlogPosts.AddAsync(blog);
                        await _blogSpotDbContext.SaveChangesAsync();

                        // Tag added successfully, redirect to a success page or display a success message
                        return RedirectToAction("BlogAdd");
                    }
                    else
                    {
                        // Duplicate tag found, return an error or display a message to the user
                        ModelState.AddModelError("", "Blog with the same Heading already exists.");
                        return View("BlogAdd");
                    }
                }
                catch (Exception)
                {
                    // Handle database operation errors
                    ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                    return View("BlogAdd");
                }
            }            
        }

        [HttpGet]
        public async Task<IActionResult> BlogList()
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            // Getting list of all Tags from DB
            var blogList = await _blogSpotDbContext.BlogPosts.ToListAsync();


            return View(blogList);
        }

        [HttpGet]
        public async Task<IActionResult> BlogView(Guid id)
        {
            var blog = await _blogSpotDbContext.BlogPosts.FindAsync(id);

            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> TagEdit(Tag viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            try
            {
                //Find Tag and then update it with new info
                var tag = await _blogSpotDbContext.Tags.FindAsync(viewModel.Id);

                if (tag != null)
                {
                    tag.Name = viewModel.Name;
                    tag.DisplayName = viewModel.DisplayName;

                    await _blogSpotDbContext.SaveChangesAsync();
                    return View("TagList", await _blogSpotDbContext.Tags.ToListAsync()); //Review this return, is this the best way to do it?
                }
                else
                {
                    // Tag ID does not exist
                    ModelState.AddModelError("", "Tag with this Id does not exist.");
                    return View("TagList", await _blogSpotDbContext.Tags.ToListAsync());
                }
            }
            catch (Exception)
            {
                // Handle database operation errors
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View("TagList", await _blogSpotDbContext.Tags.ToListAsync());
            }
        }
    }
}
