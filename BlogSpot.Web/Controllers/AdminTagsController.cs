using Azure.Core;
using BlogSpot.Web.Data;
using BlogSpot.Web.Models.Domain;
using BlogSpot.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogSpot.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly BlogSpotDbContext _blogSpotDbContext;

        public AdminTagsController(BlogSpotDbContext blogSpotDbContext)
        {
            this._blogSpotDbContext = blogSpotDbContext;
        }

        [HttpGet]
        public IActionResult TagAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TagAdd(AddTagRequest addTagRequest)
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName,
            };

            try
            {
                var exists = await _blogSpotDbContext.Tags.AnyAsync(e => e.Name == tag.Name && e.DisplayName == tag.DisplayName);

                if (!exists)
                {
                    await _blogSpotDbContext.Tags.AddAsync(tag);
                    await _blogSpotDbContext.SaveChangesAsync();

                    // Tag added successfully, redirect to a success page or display a success message
                    return RedirectToAction("TagAdd");
                } else
                {
                    // Duplicate tag found, return an error or display a message to the user
                    ModelState.AddModelError("", "Tag with the same Name and DisplayName already exists.");
                    return View("TagAdd");
                }
            }
            catch (Exception)
            {
                // Handle database operation errors
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View("TagAdd");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TagList()
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            // Getting list of all Tags from DB
            var tagList = await _blogSpotDbContext.Tags.ToListAsync();

            return View(tagList);
        }

        [HttpPost]
        public async Task<IActionResult> TagDelete(string id)
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            try
            {

                var tagId = await _blogSpotDbContext.Tags.FindAsync(id);

                if (tagId != null)
                {
                    _blogSpotDbContext.Tags.Remove(tagId);
                    await _blogSpotDbContext.SaveChangesAsync();
                } else
                {
                    // Tag ID does not exist
                    ModelState.AddModelError("", "Tag with this Id does not exist.");
                    return View("TagList");
                }

            }
            catch (Exception)
            {
                // Handle database operation errors
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View("TagList");
            }

            return View("tagList");
        }
    }
}
