using cr_app_webapi.Dto;
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
    public class ImageController : ControllerBase
    {
        private readonly IMongoRepo<InventoryImage, InventoryImage> _inventoryImage;
        private AuthServices _authService;
        public ImageController(IMongoRepo<InventoryImage, InventoryImage> inventoryImage, AuthServices authServices)
        {
            _inventoryImage = inventoryImage;
            _authService = authServices;
        }

        [HttpGet("images")]
        public List<InventoryImage> GetAll()
        {
            return _inventoryImage.AsQueryable().ToList();
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<ImageDTO> GetImage(string id)
        {
            var image = _inventoryImage.FilterBy<ImageDTO>(
                filter => filter.Id == id,
                project => new ImageDTO()
                {
                    data = project.img!.data,
                    extension = project.img.contentType
                }
            ).FirstOrDefault();
            if (image is null)
            {
                return NotFound();
            }
            return image;
        }

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
            var fileDir = Path.GetFullPath("Uploads");
            if (!Directory.Exists(fileDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(fileDir);
            }
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

            /* Task task = _image.InsertOneAsync(formData);
            task.Wait(); */
            await _inventoryImage.GenericFindOneUpdate<InventoryImage>(
                f => f.ItemNumber == data.ItemNumber && f.JobId == data.JobId,
                formData, upsert: true
            );

            return CreatedAtAction(nameof(GetAll), new { id = formData.Id }, formData.Id);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var image = _inventoryImage.FilterBy(f => f.Id == id).FirstOrDefault();
            if (image is null)
            {
                return NotFound();
            }

            await _inventoryImage.DeleteByIdAsync(f => f.Id == id);
            return NoContent();
        }
    }
}