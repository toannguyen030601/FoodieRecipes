namespace FoodieHub.API.Models.Response
{
    public class UploadImageResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public string FilePath { get; set; } = string.Empty;
    }
}
