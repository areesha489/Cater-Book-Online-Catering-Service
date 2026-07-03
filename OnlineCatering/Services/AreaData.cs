namespace OnlineCatering.Services;

public static class AreaData
{
    public static readonly Dictionary<string, string[]> AreasByCity = new()
    {
        ["Lahore"] = new[] { "Gulberg", "DHA", "Model Town", "Johar Town", "Bahria Town", "Cantt", "Shadman", "Faisal Town", "Wapda Town", "Allama Iqbal Town" },
        ["Karachi"] = new[] { "Clifton", "DHA", "Gulshan-e-Iqbal", "North Nazimabad", "PECHS", "Malir", "Korangi", "Saddar", "Defence", "FB Area" },
        ["Islamabad"] = new[] { "F-6", "F-7", "F-8", "G-9", "G-10", "I-8", "Bahria Town", "DHA", "Blue Area", "E-11" },
        ["Rawalpindi"] = new[] { "Satellite Town", "Bahria Town", "Saddar", "Chaklala", "Peshawar Road", "Adyala Road" },
        ["Multan"] = new[] { "Cantt", "Gulgasht", "Bosan Road", "Shah Rukn-e-Alam", "MDA" },
        ["Faisalabad"] = new[] { "D Ground", "Susan Road", "Madina Town", "Peoples Colony", "Canal Road" }
    };

    public static string[] AllCities => AreasByCity.Keys.ToArray();

    public static string[] GetAreas(string? city)
    {
        if (string.IsNullOrWhiteSpace(city) || !AreasByCity.ContainsKey(city))
            return AreasByCity.Values.SelectMany(a => a).Distinct().OrderBy(a => a).ToArray();
        return AreasByCity[city];
    }

    public static string[] AllAreas => AreasByCity.Values.SelectMany(a => a).Distinct().OrderBy(a => a).ToArray();
}
