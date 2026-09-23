using System.Globalization;
using System.Text;

namespace Arahk.Neighbor.Domain.Services;

/// <summary>
/// Derives avatar initials from DisplayName (UX: first grapheme of first two words;
/// single word → up to 2 graphemes). Thai leading vowels (เ แ โ ใ ไ) are skipped
/// so 「สมชาย ใจดี」→ สจ per approved UI.
/// </summary>
public static class DisplayNameInitials
{
    // Thai vowels that precede the consonant in writing order.
    private static readonly HashSet<char> ThaiLeadingVowels = ['เ', 'แ', 'โ', 'ใ', 'ไ'];

    public static string FromDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return "?";

        var parts = displayName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return "?";

        if (parts.Length >= 2)
            return FirstInitial(parts[0]) + FirstInitial(parts[1]);

        return TakeGraphemes(parts[0], 2);
    }

    private static string FirstInitial(string word)
    {
        foreach (var rune in word.EnumerateRunes())
        {
            var ch = rune.ToString();
            if (ch.Length == 1 && ThaiLeadingVowels.Contains(ch[0]))
                continue;
            return ch;
        }

        return FirstGrapheme(word);
    }

    private static string FirstGrapheme(string text)
    {
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        return enumerator.MoveNext() ? enumerator.GetTextElement() : string.Empty;
    }

    private static string TakeGraphemes(string text, int count)
    {
        var sb = new StringBuilder();
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        var taken = 0;
        while (enumerator.MoveNext() && taken < count)
        {
            sb.Append(enumerator.GetTextElement());
            taken++;
        }
        return sb.Length == 0 ? "?" : sb.ToString();
    }
}
