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
                bool exists = await _blogSpotDbContext.Tags.AnyAsync(e => e.Name == tag.Name);

                if (!exists)
                {
                    await _blogSpotDbContext.Tags.AddAsync(tag);
                    await _blogSpotDbContext.SaveChangesAsync();

                    // Tag added successfully, redirect to a success page or display a success message
                    return RedirectToAction("TagAdd");
                }
                else
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
        public async Task<IActionResult> TagDelete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                // Invalid input, return a bad request or display validation errors
                return BadRequest(ModelState);
            }

            try
            {
                //Find Tag and then delete it if it exists
                var tag = await _blogSpotDbContext.Tags.FindAsync(id);

                if (tag != null)
                {
                    _blogSpotDbContext.Tags.Remove(tag);
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

        [HttpGet]
        public async Task<IActionResult> TagEdit(Guid id)
        {
            var tag = await _blogSpotDbContext.Tags.FindAsync(id);

            return View(tag);
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
