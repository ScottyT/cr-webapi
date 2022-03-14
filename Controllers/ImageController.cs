using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace cr_app_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IMongoRepo<InventoryModel> _image;
        public ImageController(IMongoRepo<InventoryModel> image)
        {
            _image = image;
        }

        [HttpGet("inventory")]
        public List<InventoryModel> GetAll()
        {
            return _image.AsQueryable().ToList();
        }

        [HttpGet("inventory/{jobid}")]
        public ActionResult<IEnumerable<FileModel>> GetByJobId(string jobid)
        {
            var images = _image.FilterBy(
                filter => filter.JobId == jobid,
                project => project.img
            );
            if (images is null)
            {
                return NotFound();
            }
            return images.ToList();
        }

        [HttpGet()]

        [HttpPost("upload/content-inventory-image")]
        public async Task<IActionResult> UploadImage([FromForm] InventoryModel data)
        {
            IFormFile? image = Request.Form.Files.FirstOrDefault();
            if (image.Length <= 0) return BadRequest("Empty file");
            if (data is null) return BadRequest("No form data");
            var ogName = Path.GetFileName(image.FileName);
            var extensionType = image?.ContentType.Split('/')[1];
            var filePath = Path.Combine("Uploads", "single");
            using (var stream = System.IO.File.Create(filePath))
            {
                await image.CopyToAsync(stream);
            }
            FileModel img = new FileModel()
            {
                data = System.IO.File.ReadAllBytes(filePath),
                contentType = image?.ContentType
            };

            InventoryModel formData = new InventoryModel()
            {
                img = img,
                JobId = data.JobId,
                ItemNumber = data.ItemNumber
            };
            await _image.InsertOneAsync(formData);
            return CreatedAtAction(nameof(GetAll), "Uploaded image.");
        }
    }
}