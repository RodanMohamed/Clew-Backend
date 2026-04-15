using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ImageUploadDto
    {
        public IFormFile File { get; set; } = null!;
    }
}
