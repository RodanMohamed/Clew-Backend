using Clew.BLL;
using Clew.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clew.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserOnly")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageManager _imageManager;

        public ImagesController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto dto)
        {
            var result = await _imageManager.UploadAsync(
                dto,
                Directory.GetCurrentDirectory(),
                Request.Scheme,
                Request.Host.Value);

            return ToActionResult(result);
        }

        private IActionResult ToActionResult<T>(GeneralResult<T> result)
        {
            if (result.Success) return Ok(result);
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)) return NotFound(result);
            if (result.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase)) return Unauthorized(result);
            return BadRequest(result);
        }
    }
}