namespace AdventureEngine.Services;

/// <summary>
/// Parses natural language input into structured commands
/// Phase 1: Enhanced pattern matching with prepositions and conjunctions
/// </summary>
public class CommandParser
{
    /// <summary>
    /// Parse user input into a structured command
    /// Supports patterns like:
    /// - "take lamp"
    /// - "take lamp and sword"
    /// - "put lamp in box"
    /// - "use key on door"
    /// - "put the golden lamp in the wooden box"
    /// </summary>
    public static ParsedInput Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ParsedInput { RawInput = input };
        }

        var trimmed = input.Trim().ToLower();
        var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return new ParsedInput { RawInput = input };
        }

        // First word is the verb
        var verb = words[0];
        var parsed = new ParsedInput
        {
            Verb = verb,
            RawInput = input
        };

        if (words.Length == 1)
        {
            // Just a verb, no arguments
            return parsed;
        }

        // Get all words after the verb
        var remainingWords = words[1..];

        // Find preposition if any
        var prepositionIndex = FindPrepositionIndex(remainingWords);

        if (prepositionIndex == -1)
        {
            // No preposition - just direct objects (possibly with "and")
            // Example: "take lamp and sword"
            var directObjectText = string.Join(" ", remainingWords);

            // Check for multi-object keywords (all, everything)
            if (remainingWords.Any(PrepositionHelper.IsMultiObjectKeyword))
            {
                parsed.IsMultiObjectCommand = true;
                parsed.DirectObjects.Add("all"); // Special marker
            }
            // Check for pronouns (it, that, them)
            else if (remainingWords.Any(PrepositionHelper.IsPronoun))
            {
                parsed.UsesPronoun = true;
                parsed.DirectObjects.Add(remainingWords.First(PrepositionHelper.IsPronoun));
            }
            else
            {
                parsed.DirectObjects = PrepositionHelper.SplitByConjunction(directObjectText);
            }
        }
        else
        {
            // Has preposition - split into direct and indirect objects
            // Example: "put lamp in box"
            // Example: "put lamp and sword in box"

            // Words before preposition are direct objects
            var beforePrep = remainingWords[..prepositionIndex];
            var directObjectText = string.Join(" ", beforePrep);
            parsed.DirectObjects = PrepositionHelper.SplitByConjunction(directObjectText);

            // The preposition itself
            var preposition = remainingWords[prepositionIndex];
            parsed.Preposition = PrepositionHelper.NormalizePreposition(preposition);

            // Words after preposition are indirect object
            if (prepositionIndex < remainingWords.Length - 1)
            {
                var afterPrep = remainingWords[(prepositionIndex + 1)..];
                var indirectObjectWords = PrepositionHelper.StripArticles(afterPrep);
                parsed.IndirectObject = string.Join(" ", indirectObjectWords);
            }
        }

        return parsed;
    }

    /// <summary>
    /// Find the index of the first preposition in the word list
    /// Returns -1 if no preposition found
    /// </summary>
    private static int FindPrepositionIndex(string[] words)
    {
        for (var i = 0; i < words.Length; i++)
        {
            if (PrepositionHelper.IsPreposition(words[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Check if input looks like it needs special parsing (has prepositions or conjunctions)
    /// </summary>
    public bool NeedsEnhancedParsing(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var words = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return words.Any(w => PrepositionHelper.IsPreposition(w) ||
                             PrepositionHelper.IsConjunction(w));
    }
}
