using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace ApiAccountService.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class CloudinaryUploadExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ImageUploadResult UploadImageUser(this IFormFile file)
        {
            Account account = new Account();
            Cloudinary cloudinary = new Cloudinary(account);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Transformation = new Transformation().Crop("thumb").Gravity("face").Width(150).Height(150)
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult;
        }
    }
}
