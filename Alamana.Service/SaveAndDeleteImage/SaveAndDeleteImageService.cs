using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.SaveAndDeleteImage
{
    public class SaveAndDeleteImageService : ISaveAndDeleteImageService
    {

        public async Task<string> UploadToCloudinary(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var account = new Account(
                "dvo2qoi4s",                     // Cloud name
                "288973868219147",              // API Key
                "qk0I71nyL82G_v8cSBkfCbdom3s"   // API Secret
            );

            var cloudinary = new Cloudinary(account);

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "categories" // الصور هتروح فولدر اسمه "categories"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                return uploadResult.SecureUrl.ToString(); // رابط مباشر للصورة

            return null;
        }





        public bool DeleteFromCloudinary(string publicId)
        {
            var account = new Account(
                "dxlfz2yce", // Cloud name
                "935569666187659", // API Key
                "4dhGKFY3PGgfimJV63jwEL1t3tY" // API Secret
            );

            var cloudinary = new Cloudinary(account);

            var deletionParams = new DeletionParams(publicId);
            var result = cloudinary.Destroy(deletionParams);

            return result.Result == "ok" || result.Result == "not found";
        }








        public string ExtractPublicIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var fileName = Path.GetFileNameWithoutExtension(segments.Last());
            var folder = segments[^2];

            return $"{folder}/{fileName}";
        }



    }
}
