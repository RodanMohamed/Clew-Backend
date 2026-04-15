using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Clew.Common;

namespace Clew.BLL
{
    public class ImageManager : IImageManager
    {
        private readonly IValidator<ImageUploadDto> _validator;
        private readonly IErrorMapper _errorMapper;
        /*------------------------------------------------------------------*/
        public ImageManager(IValidator<ImageUploadDto> validator, IErrorMapper errorMapper)
        {
            _validator = validator;
            _errorMapper = errorMapper;
        }
        public async Task<GeneralResult<ImageUploadResultDto>> UploadAsync(ImageUploadDto imageUploadDto, string basePath, string? schema, string? host)
        {
            if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(host))
            {
                return GeneralResult<ImageUploadResultDto>.FailResult("Missing Schema or host");
            }

            var result = await _validator.ValidateAsync(imageUploadDto);
            if (!result.IsValid)
            {
                var errors = _errorMapper.MapError(result);
                return GeneralResult<ImageUploadResultDto>.FailResult(errors);
            }
            //var _file = imageUploadDto.File;
            //if (_file == null || _file.Length == 0)
            //{
            //    return GeneralResult<ImageUploadResultDto>.FailResult("File is required");
            //}

            var file = imageUploadDto.File;
            var extention = Path.GetExtension(file.FileName).ToLower();
            var cleanName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "-").ToLower();
            var newFileName = $"{cleanName}-{Guid.NewGuid()}{extention}";
            var directoryPath = Path.Combine(basePath, "Files");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fullFilePath = Path.Combine(directoryPath, newFileName);
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            var url = $"{schema}://{host}/Files/{newFileName}";
            var imageUploadResultDto = new ImageUploadResultDto(url);
            return GeneralResult<ImageUploadResultDto>.SuccessResult(imageUploadResultDto);
        }
    }
}   

