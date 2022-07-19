using Attrecto.Data;
using Attrecto.Dtos.User;
using Attrecto.EmailService;
using Attrecto.IdentityServer;
using Attrecto.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Attrecto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IClaimsHelper _claimsHelper;

        public ImageController(IClaimsHelper claimsHelper)
        {
            _claimsHelper = claimsHelper;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if(file == null)
            {
                return BadRequest("No file attached");
            }
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
            string uploads = Path.Combine(Environment.CurrentDirectory, "Images");
            Directory.CreateDirectory(uploads);
            if (file.Length > 0)
            {
                string filePath = Path.Combine(uploads, $"{claimId}_profile_image.jpg");
                using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(fileStream);
                }
            }
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetImage()
        {
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
            string uploads = Path.Combine(Environment.CurrentDirectory, "Images");
            string filePath = Path.Combine(uploads, $"{claimId}_profile_image.jpg");
            try
            {
                var content = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(content, "image/jpeg", $"{claimId}_profile_image.jpg");
            }
            catch(FileNotFoundException)
            {
                return BadRequest("File not found!");
            }
        }
    }
}
