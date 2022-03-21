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
    [Route("api/image")]
    [Authorize("read:reports")]
    public class ImageController : ControllerBase
    {
        private readonly IMongoRepo<InventoryImage,InventoryImage> _image;
        private readonly ReportsService _reportsService;
        public ImageController(IMongoRepo<InventoryImage,InventoryImage> image, ReportsService reportsService)
        {
            _image = image;
            _reportsService = reportsService;
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

        [HttpPut("upload/content-inventory-image")]
        public async Task<IActionResult> UploadImage([FromForm] InventoryImage? data)
        {
            IFormFileCollection? images = Request.Form.Files;
            if (!ValidExtensions.IsValid<IFormFileCollection>(images))
            {
                return BadRequest("Image is not valid.");
            }
            if (images.Count <= 0) return Ok("no files");
            if (data is null) return BadRequest("No form data");
            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var ogName = Path.GetFileName(image.FileName);
                    var extensionType = image.ContentType.Split('/')[1];
                    var filePath = Path.Combine("Uploads", "single");
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var file = System.IO.File.ReadAllBytes(filePath);
                    FileModel img = new FileModel()
                    {
                        data = file,
                        contentType = image.ContentType,
                        size = file.Length,
                        filename = ogName
                    };

                    InventoryImage formData = new InventoryImage()
                    {
                        img = img,
                        JobId = data.JobId
                    };
                   
                    await _image.InsertOneAsync(formData);
                }
            }

            return CreatedAtAction(nameof(GetAll), "Uploaded image");
        }
    }
}