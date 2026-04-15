using Clew.Common;

namespace Clew.BLL
{
    public interface IImageManager
    {
        Task<GeneralResult<ImageUploadResultDto>> UploadAsync(ImageUploadDto imageUploadDto, string basePath, string? schema, string? host);
    }
}