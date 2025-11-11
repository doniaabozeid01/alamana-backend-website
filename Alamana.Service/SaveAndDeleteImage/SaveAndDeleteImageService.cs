using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.SaveAndDeleteImage
{
    public class SaveAndDeleteImageService : ISaveAndDeleteImageService
    {

        //public async Task<string> UploadToCloudinary(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return null;

        //    var account = new Account(
        //        "dvo2qoi4s",                     // Cloud name
        //        "288973868219147",              // API Key
        //        "qk0I71nyL82G_v8cSBkfCbdom3s"   // API Secret
        //    );

        //    var cloudinary = new Cloudinary(account);

        //    await using var stream = file.OpenReadStream();
        //    var uploadParams = new ImageUploadParams
        //    {
        //        File = new FileDescription(file.FileName, stream),
        //        Folder = "categories" // الصور هتروح فولدر اسمه "categories"
        //    };

        //    var uploadResult = await cloudinary.UploadAsync(uploadParams);

        //    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        //        return uploadResult.SecureUrl.ToString(); // رابط مباشر للصورة

        //    return null;
        //}




    public async Task<string> UploadToCloudinary(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;

        var account = new Account(
            "dvo2qoi4s",           // Cloud name (خليه نفس المستخدم في الحذف)
            "288973868219147",     // API Key
            "qk0I71nyL82G_v8cSBkfCbdom3s" // API Secret
        );
        var cloudinary = new Cloudinary(account);

        bool isVideo = IsVideo(file);

        await using var stream = file.OpenReadStream();
        var fileDesc = new FileDescription(file.FileName, stream);

        if (isVideo)
        {
            // لو الفيديو كبير، استخدمي UploadLargeAsync (chunked)
            if (file.Length > 10_000_000) // ~10MB
            {
                var p = new VideoUploadParams
                {
                    File = fileDesc,
                    Folder = "categories",
                    //ResourceType = ResourceType.Video,
                    //ChunkSize = 6_000_000 // 6MB chunks
                };
                var r = await cloudinary.UploadLargeAsync(p);
                return r.StatusCode == HttpStatusCode.OK ? r.SecureUrl?.ToString() : null;
            }
            else
            {
                var p = new VideoUploadParams
                {
                    File = fileDesc,
                    Folder = "categories",
                    //ResourceType = ResourceType.Video
                };
                var r = await cloudinary.UploadAsync(p);
                return r.StatusCode == HttpStatusCode.OK ? r.SecureUrl?.ToString() : null;
            }
        }
        else
        {
            var p = new ImageUploadParams
            {
                File = fileDesc,
                Folder = "categories"
            };
            var r = await cloudinary.UploadAsync(p);
            return r.StatusCode == HttpStatusCode.OK ? r.SecureUrl?.ToString() : null;
        }
    }

    private static bool IsVideo(IFormFile file)
    {
        if (file?.ContentType?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) == true)
            return true;

        var ext = Path.GetExtension(file?.FileName ?? "").ToLowerInvariant();
        return new[] { ".mp4", ".mov", ".mkv", ".webm", ".avi" }.Contains(ext);
    }






        //public bool DeleteFromCloudinary(string publicId)
        //    {
        //        var account = new Account(
        //            "dxlfz2yce", // Cloud name
        //            "935569666187659", // API Key
        //            "4dhGKFY3PGgfimJV63jwEL1t3tY" // API Secret
        //        );

        //        var cloudinary = new Cloudinary(account);

        //        var deletionParams = new DeletionParams(publicId);
        //        var result = cloudinary.Destroy(deletionParams);

        //        return result.Result == "ok" || result.Result == "not found";
        //    }













        public bool DeleteFromCloudinary(string publicId)
        {
            var account = new Account(
                "dvo2qoi4s",           // نفس حساب الرفع
                "288973868219147",
                "qk0I71nyL82G_v8cSBkfCbdom3s"
            );
            var cloudinary = new Cloudinary(account);

            // جرّب كصورة أولاً
            var result = cloudinary.Destroy(new DeletionParams(publicId));
            if (result.Result == "ok" || result.Result == "not found") return true;

            // جرّب كفيديو
            var vres = cloudinary.Destroy(new DeletionParams(publicId) { ResourceType = ResourceType.Video });
            return vres.Result == "ok" || vres.Result == "not found";
        }







        //public string ExtractPublicIdFromUrl(string imageUrl)
        //{
        //    if (string.IsNullOrEmpty(imageUrl))
        //        return null;

        //    var uri = new Uri(imageUrl);
        //    var segments = uri.AbsolutePath.Split('/');
        //    var fileName = Path.GetFileNameWithoutExtension(segments.Last());
        //    var folder = segments[^2];

        //    return $"{folder}/{fileName}";
        //}











        public string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var uri = new Uri(url);
            var path = uri.AbsolutePath; // e.g. /dvo2qoi4s/video/upload/v172.../categories/sub/f.mp4

            var marker = "/upload/";
            var idx = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            var after = path.Substring(idx + marker.Length).Trim('/');
            var parts = after.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            // أول جزء ممكن يكون v123456 -> شيليه
            if (parts.Count > 0 && parts[0].Length > 1 && parts[0][0] == 'v' && long.TryParse(parts[0].Substring(1), out _))
                parts.RemoveAt(0);

            // شيل الامتداد من آخر عنصر
            if (parts.Count > 0)
                parts[^1] = Path.GetFileNameWithoutExtension(parts[^1]);

            return string.Join('/', parts); // categories/sub/f
        }


    }
}
