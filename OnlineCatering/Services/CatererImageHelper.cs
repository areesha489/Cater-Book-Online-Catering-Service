using OnlineCatering.Models;

namespace OnlineCatering.Services;

public static class CatererImageHelper
{
    // Royal Feast uses a dish photo as the main restaurant image.
    public const string RoyalFeastImageUrl =
        "https://images.pexels.com/photos/29685045/pexels-photo-29685045.jpeg?auto=compress&cs=tinysrgb&w=800";

    public const string RoyalFeastMuttonKarahiUrl =
        "https://images.pexels.com/photos/29685045/pexels-photo-29685045.jpeg?auto=compress&cs=tinysrgb&w=400";

    public const string RoyalFeastSeekhKebabUrl =
        "https://images.pexels.com/photos/2338407/pexels-photo-2338407.jpeg?auto=compress&cs=tinysrgb&w=400";

    public static readonly string[] DefaultImageUrls =
    [
        RoyalFeastImageUrl,
        "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=600&q=80",
        "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600&q=80",
        "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&q=80",
        "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600&q=80",
        "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80",
        "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=600&q=80"
    ];

    public const string FallbackUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&q=80";

    public static string[] Defaults => DefaultImageUrls;

    public static bool IsRoyalFeast(Caterer caterer) =>
        caterer.BusinessName.Contains("Royal Feast", StringComparison.OrdinalIgnoreCase);

    public static string GetImageUrl(Caterer caterer, int index = 0)
    {
        if (IsRoyalFeast(caterer))
        {
            if (!string.IsNullOrWhiteSpace(caterer.ImageUrl) &&
                caterer.ImageUrl.Contains("/uploads/", StringComparison.OrdinalIgnoreCase))
                return NormalizePath(caterer.ImageUrl);
            return RoyalFeastImageUrl;
        }

        if (!string.IsNullOrWhiteSpace(caterer.ImageUrl) && !IsPlaceholderPath(caterer.ImageUrl))
            return NormalizePath(caterer.ImageUrl);

        var key = caterer.CatererId > 0 ? caterer.CatererId : index;
        return DefaultImageUrls[Math.Abs(key) % DefaultImageUrls.Length];
    }

    public static string GetDefaultForIndex(int index) =>
        DefaultImageUrls[Math.Abs(index) % DefaultImageUrls.Length];

    public static bool IsPlaceholderPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return true;
        path = path.Trim().Replace('\\', '/');
        return path.Contains("/images/caterers/", StringComparison.OrdinalIgnoreCase) &&
               path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return FallbackUrl;
        path = path.Trim();
        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return path;
        return path.StartsWith('/') ? path : "/" + path;
    }
}

