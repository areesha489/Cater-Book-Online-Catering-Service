namespace OnlineCatering.Services;

public static class DishImageHelper
{
    public const string DefaultUrl =
        "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&q=80";

    private static readonly Dictionary<string, string> Images = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Chicken Biryani"] = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=400&q=80",
        ["Mutton Karahi"] = CatererImageHelper.RoyalFeastMuttonKarahiUrl,
        ["Seekh Kebab"] = CatererImageHelper.RoyalFeastSeekhKebabUrl,
        ["Vegetable Pulao"] = "https://images.unsplash.com/photo-1512058564366-7d402e7c0b6b?w=400&q=80",
        ["Paneer Tikka"] = "https://images.unsplash.com/photo-1601050690597-df0568f70950?w=400&q=80",
        ["Gulab Jamun"] = "https://images.unsplash.com/photo-1582878826629-29ae7d1622cd?w=400&q=80",
        ["Kheer"] = "https://images.unsplash.com/photo-1587132137056-bfbf0166836e?w=400&q=80",
        ["Fresh Juice"] = "https://images.unsplash.com/photo-1622597467836-f0533f1f1d5e?w=400&q=80",
        ["Dal Makhani"] = "https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=400&q=80",
        ["Samosa Chaat"] = "https://images.unsplash.com/photo-1606491956689-2ea866858667?w=400&q=80",
        ["Paneer Butter Masala"] = "https://images.unsplash.com/photo-1631452180519-c014fe946bc7?w=400&q=80",
        ["Chicken Tikka"] = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=400&q=80",
        ["Beef Nihari"] = "https://images.pexels.com/photos/35287423/pexels-photo-35287423.jpeg?auto=compress&cs=tinysrgb&w=400",
        ["Fish Fry"] = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=400&q=80",
        ["Special Biryani"] = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=400&q=80",
        ["BBQ Platter"] = "https://images.pexels.com/photos/2338407/pexels-photo-2338407.jpeg?auto=compress&cs=tinysrgb&w=400",
        ["Dessert Box"] = "https://images.unsplash.com/photo-1582878826629-29ae7d1622cd?w=400&q=80"
    };

    public static (string Name, string Price, string Category)[] HomeSliderItems =>
    [
        ("Chicken Biryani", "Rs. 450", "Dinner"),
        ("Mutton Karahi", "Rs. 650", "Dinner"),
        ("Seekh Kebab", "Rs. 350", "BBQ"),
        ("Vegetable Pulao", "Rs. 350", "Lunch"),
        ("Paneer Tikka", "Rs. 280", "Starters"),
        ("Gulab Jamun", "Rs. 80", "Desserts"),
        ("Fresh Juice", "Rs. 120", "Beverages"),
        ("Dal Makhani", "Rs. 320", "Lunch")
    ];

    public static string GetImageUrl(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return DefaultUrl;
        return Images.TryGetValue(itemName.Trim(), out var url) ? url : DefaultUrl;
    }
}
