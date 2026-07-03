namespace OnlineCatering.Services;

public class ImageUploadService
{
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public ImageUploadService(IWebHostEnvironment env) => _env = env;

    public async Task<(bool Success, string RelativePath, string Error)> SaveCatererImageAsync(IFormFile file, int catererId)
    {
        if (file == null || file.Length == 0)
            return (false, string.Empty, "No image file selected.");

        if (file.Length > MaxFileSize)
            return (false, string.Empty, "Image must be 5 MB or smaller.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return (false, string.Empty, "Only JPG, PNG and WEBP images are allowed.");

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "caterers");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"caterer_{catererId}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (true, $"/uploads/caterers/{fileName}", string.Empty);
    }

    public void DeleteLocalFile(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return;

        var fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
