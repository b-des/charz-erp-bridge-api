namespace CharzPiexApi.Utils;

public static class VehicleUtils
{
    private static readonly Dictionary<string, string> KnownNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["weichai"] = "Вейчай"
    };

    public static string GetReadableName(string value)
    {
        var parts = System.Text.RegularExpressions.Regex
            .Replace(value.Trim(), @"-|(?<=\d)(?=[A-Za-z])", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return string.Join(' ', parts.Select(ToUkrainianTitleCase));
    }

    private static string ToUkrainianTitleCase(string value)
    {
        if (KnownNames.TryGetValue(value, out var knownName))
        {
            return knownName;
        }

        var transliterated = string.Concat(value.ToLowerInvariant().Select(TransliterateCharacter));

        return char.IsLetter(transliterated[0])
            ? char.ToUpper(transliterated[0]) + transliterated[1..]
            : transliterated;
    }

    private static string TransliterateCharacter(char character) => character switch
    {
        'a' => "а", 'b' => "б", 'c' => "к", 'd' => "д", 'e' => "е",
        'f' => "ф", 'g' => "г", 'h' => "г", 'i' => "і", 'j' => "й",
        'k' => "к", 'l' => "л", 'm' => "м", 'n' => "н", 'o' => "о",
        'p' => "п", 'q' => "к", 'r' => "р", 's' => "с", 't' => "т",
        'u' => "у", 'v' => "в", 'w' => "в", 'x' => "кс", 'y' => "и",
        'z' => "з",
        _ => character.ToString()
    };
}
