using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ImageUploadResultDto
    {
        public ImageUploadResultDto(string url)
        {
            Url = url;
        }

        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
