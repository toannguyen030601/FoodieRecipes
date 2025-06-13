using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Extentions
{
    public class ImageExtentions
    {
        private readonly Cloudinary _cloudinary;

        public ImageExtentions(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<UploadImageResult> UploadImage(IFormFile file, string folder)
        {
            if (!IsValidImage(file))
            {
                return new UploadImageResult
                {
                    Success = false,
                    Message = "Invalid Image"
                };
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new UploadImageResult
                {
                    Success = true,
                    Message = "Upload file success",
                    FilePath = uploadResult.SecureUrl.ToString()
                };
            }
            return new UploadImageResult
            {
                Success = false,
                Message = uploadResult.Error?.Message
            };
        }
        public async Task<string> SaveImageFromBytesAsync(byte[] imageBytes, string fileName)
        {
            // Define the path to save the image in the wwwroot/images folder
            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","QRCodes");

            // Ensure the directory exists, if not, create it
            if (!Directory.Exists(wwwRootPath))
            {
                Directory.CreateDirectory(wwwRootPath);
            }

            // Create the full path for the image
            string filePath = Path.Combine(wwwRootPath, fileName);

            // Write the byte array as an image file
            await File.WriteAllBytesAsync(filePath, imageBytes);
            string relativePath = Path.Combine("images","QRCodes", fileName);
            return relativePath;
        }



        public async Task<UploadImageResult> UploadMutipleImages(List<IFormFile> files, string folder)
        {
            var listFilePath = new List<string>();
            foreach (var file in files)
            {
                if (!IsValidImage(file))
                {
                    return new UploadImageResult
                    {
                        Success = false,
                        Message = "Invalid Image"
                    };
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    listFilePath.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    return new UploadImageResult
                    {
                        Success = false,
                        Message = uploadResult.Error?.Message
                    };
                }
            }
            return new UploadImageResult
            {
                Success = true,
                Message = "Upload file success",
                FilePath = listFilePath
            };
        }

        public void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return;

                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Tìm "upload", vì sau đó là phần publicId
                var uploadIndex = Array.FindIndex(segments, s => s.Equals("upload", StringComparison.OrdinalIgnoreCase));
                if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
                    return;

                // Lấy phần sau "upload/"
                var afterUpload = segments.Skip(uploadIndex + 1).ToList();

                // Bỏ version nếu có (vd: v1718000000)
                if (afterUpload[0].StartsWith("v"))
                    afterUpload.RemoveAt(0);

                // Nối lại: "Users/avatar123.jpg" => "Users/avatar123"
                var publicIdWithExt = string.Join('/', afterUpload); // Users/avatar.jpg
                var publicId = Path.Combine(Path.GetDirectoryName(publicIdWithExt) ?? "", Path.GetFileNameWithoutExtension(publicIdWithExt))
                                   .Replace("\\", "/");

                // Xóa ảnh
                var deletionParams = new DeletionParams(publicId);
                var result = _cloudinary.Destroy(deletionParams);

                // (Optional) log kết quả
                Console.WriteLine($"DeleteImage: publicId={publicId}, result={result.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteImage error: " + ex.Message);
                // Có thể log vào file / hệ thống log nếu muốn
            }
        }


        public async Task<bool> UploadImageByName(IFormFile file, string folder, string fileName)
        {
            if (!IsValidImage(file)) return false;
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    PublicId = fileName
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            var allowExtentions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileExtention = Path.GetExtension(file.FileName);
            if (allowExtentions.Contains(fileExtention))
            {
                return true;
            }
            return false;
        }
    }
}
