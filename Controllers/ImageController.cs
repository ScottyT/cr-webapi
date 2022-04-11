using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Text.Json;

namespace cr_app_webapi.Controllers
{
    [ApiController]
    [Route("image")]
    [Authorize("read:reports")]
    public class ImageController : ControllerBase
    {
        private readonly IMongoRepo<InventoryImage, InventoryImage> _image;
        private AuthServices _authService;
        public ImageController(IMongoRepo<InventoryImage, InventoryImage> image, AuthServices authServices)
        {
            _image = image;
            _authService = authServices;
        }

        [HttpGet("inventory")]
        public List<InventoryImage> GetAll()
        {
            return _image.AsQueryable().ToList();
        }

        /* [HttpGet("inventory/{jobid}")]
        public ActionResult<List<FileModel>> GetByJobId(string jobid)
        {
            var images = _image.FilterBy(
                filter => filter.JobId == jobid,
                project => project.img
            ).ToList();
            if (images.Count <= 0 && !ValidExtensions.IsValid<FileModel>(images[0]))
            {
                return NotFound();
            }
            
            return images;
        } */

        [HttpPost("upload/content-inventory-image")]
        public async Task<IActionResult> UploadImage([FromForm] InventoryImage? data)
        {
            IFormFileCollection? images = Request.Form.Files;
            IFormFile? image = Request.Form.Files[0];
            if (!ValidExtensions.IsValid<IFormFileCollection>(images))
            {
                return BadRequest("Image is not valid.");
            }
            if (images.Count <= 0) return Ok("no files uploaded");
            if (data is null) return BadRequest("No form data");
            var ogName = Path.GetFileName(image.FileName);
            var extensionType = image.ContentType.Split('/')[1];
            var filePath = Path.Combine("Uploads", "img");
            using (var stream = System.IO.File.Create(filePath))
            {
                await image.CopyToAsync(stream);
            }

            var file = System.IO.File.ReadAllBytes(filePath);
            var item = new FileModel
            {
                data = file,
                contentType = image.ContentType,
                size = file.Length,
                filename = ogName
            };

            InventoryImage formData = new InventoryImage()
            {
                img = item,
                JobId = data.JobId,
                ItemNumber = data.ItemNumber
            };

            Task task = _image.InsertOneAsync(formData);
            task.Wait();

            return CreatedAtAction(nameof(GetAll), new { id = formData.Id }, formData.Id);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var image = _image.FilterBy(f => f.Id == id).FirstOrDefault();
            if (image is null)
            {
                return NotFound();
            }

            await _image.DeleteByIdAsync(f => f.Id == id);
            return NoContent();
        }
    }
}