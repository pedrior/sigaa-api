using System.Globalization;

namespace Sigaa.Api.Common.Scraping.Transformations;

internal sealed partial class TitlecaseTransform : IValueTransform
{
    public static TitlecaseTransform Instance { get; } = new();

    private static readonly HashSet<string> PtBrStopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Articles.
        "a", "o", "as", "os", "um", "uma", "uns", "umas",

        // Prepositions.
        "ante", "após", "até", "com", "contra", "de", "desde", "em", "entre", "para", "perante",
        "por", "sem", "sob", "sobre", "trás",

        // Contractions.
        "da", "do", "das", "dos", "na", "no", "nas", "nos", "num", "numa", "nuns", "numas",
        "pelo", "pela", "pelos", "pelas",

        // Conjunctions.
        "e", "ou", "mas", "que", "se", "nem"
    };

    private static readonly CultureInfo PtBrCulture = new("pt-BR");

    public string? Transform(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        // Apply the base title casing from the PT-BR culture.
        // Calling ToLower() first handles all-caps strings (e.g., "CENTRO DE INFORMATICA").
        var titled = PtBrCulture.TextInfo.ToTitleCase(value.ToLower(PtBrCulture));
        var builder = new StringBuilder(titled);

        var index = 0;
        var isFirstToken = true;

        while (index < builder.Length)
        {
            // Finds the next token (a sequence of non-whitespace characters).
            var tokenStartIndex = index;
            while (tokenStartIndex < builder.Length && char.IsWhiteSpace(builder[tokenStartIndex]))
            {
                tokenStartIndex++;
            }

            // If we reached the end of the string, we're done.
            if (tokenStartIndex >= builder.Length)
            {
                break;
            }

            var tokenEndIndex = tokenStartIndex;
            while (tokenEndIndex < builder.Length && !char.IsWhiteSpace(builder[tokenEndIndex]))
            {
                tokenEndIndex++;
            }

            // The first token always remains capitalized, so we just skip it.
            if (isFirstToken)
            {
                isFirstToken = false;
                index = tokenEndIndex;

                continue;
            }

            // Isolate the core word by stripping leading/trailing punctuation.
            var wordStartIndex = tokenStartIndex;
            while (wordStartIndex < tokenEndIndex && !char.IsLetterOrDigit(builder[wordStartIndex]))
            {
                wordStartIndex++;
            }

            var wordEndIndex = tokenEndIndex - 1;
            while (wordEndIndex > wordStartIndex && !char.IsLetterOrDigit(builder[wordEndIndex]))
            {
                wordEndIndex--;
            }

            //  Process the core word, which may contain hyphens.
            var partStartIndex = wordStartIndex;
            for (var index2 = wordStartIndex; index2 <= wordEndIndex; index2++)
            {
                if (builder[index2] is not '-')
                {
                    continue;
                }

                // Hyphen found, process the part before it.
                var partLength = index2 - partStartIndex;
                ProcessWordPart(builder, partStartIndex, partLength);
                partStartIndex = index2 + 1; // Start the next part after the hyphen.
            }

            // Process the final (or only) part of the word.
            var finalPartLength = wordEndIndex - partStartIndex + 1;
            ProcessWordPart(builder, partStartIndex, finalPartLength);

            // Move the main cursor to the end of the token.
            index = tokenEndIndex;
        }

        return builder.ToString();
    }

    private static void ProcessWordPart(StringBuilder builder, int startIndex, int length)
    {
        if (length <= 0)
        {
            return;
        }

        var part = builder.ToString(startIndex, length);

        if (IsRomanNumeral(part))
        {
            SetCaseInRange(builder, startIndex, length, toUpper: true);
        }
        else if (PtBrStopwords.Contains(part))
        {
            SetCaseInRange(builder, startIndex, length, toUpper: false);
        }
    }

    private static bool IsRomanNumeral(string word) => RomanNumeralRegex().IsMatch(word);

    private static void SetCaseInRange(StringBuilder builder, int startIndex, int length, bool toUpper)
    {
        if (length <= 0)
        {
            return;
        }

        for (var i = 0; i < length; i++)
        {
            var index = startIndex + i;
            builder[index] = toUpper
                ? PtBrCulture.TextInfo.ToUpper(builder[index])
                : PtBrCulture.TextInfo.ToLower(builder[index]);
        }
    }
    
    [GeneratedRegex(
        "^(?=[MDCLXVI])M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$",
        RegexOptions.IgnoreCase)]
    private static partial Regex RomanNumeralRegex();
}