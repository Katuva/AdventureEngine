namespace AdventureEngine.Services;

/// <summary>
/// Provides fuzzy string matching for handling typos and partial matches
/// Phase 3: Typo tolerance
/// </summary>
public static class FuzzyMatcher
{
    /// <summary>
    /// Calculate Levenshtein distance between two strings
    /// Returns the number of edits (insertions, deletions, substitutions) needed
    /// </summary>
    public static int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.IsNullOrEmpty(target) ? 0 : target.Length;
        }

        if (string.IsNullOrEmpty(target))
        {
            return source.Length;
        }

        var sourceLength = source.Length;
        var targetLength = target.Length;
        var distance = new int[sourceLength + 1, targetLength + 1];

        // Initialize first column and row
        for (var i = 0; i <= sourceLength; i++)
        {
            distance[i, 0] = i;
        }

        for (var j = 0; j <= targetLength; j++)
        {
            distance[0, j] = j;
        }

        // Calculate distances
        for (var i = 1; i <= sourceLength; i++)
        {
            for (var j = 1; j <= targetLength; j++)
            {
                var cost = target[j - 1] == source[i - 1] ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(
                        distance[i - 1, j] + 1,      // Deletion
                        distance[i, j - 1] + 1),     // Insertion
                    distance[i - 1, j - 1] + cost);  // Substitution
            }
        }

        return distance[sourceLength, targetLength];
    }

    /// <summary>
    /// Check if two strings are similar enough (fuzzy match)
    /// </summary>
    public static bool IsSimilar(string source, string target, int maxDistance = 2)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
        {
            return false;
        }

        var distance = LevenshteinDistance(source.ToLower(), target.ToLower());
        return distance <= maxDistance;
    }

    /// <summary>
    /// Find the best fuzzy match from a list of candidates
    /// Returns null if no good match found
    /// </summary>
    public static string? FindBestMatch(string input, IEnumerable<string> candidates, int maxDistance = 2)
    {
        var inputLower = input.ToLower();
        var bestMatch = candidates
            .Select(c => new { Candidate = c, Distance = LevenshteinDistance(inputLower, c.ToLower()) })
            .Where(x => x.Distance <= maxDistance)
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        return bestMatch?.Candidate;
    }

    /// <summary>
    /// Find all fuzzy matches from a list of candidates
    /// </summary>
    public static List<string> FindMatches(string input, IEnumerable<string> candidates, int maxDistance = 2)
    {
        var inputLower = input.ToLower();
        return candidates
            .Where(c => LevenshteinDistance(inputLower, c.ToLower()) <= maxDistance)
            .OrderBy(c => LevenshteinDistance(inputLower, c.ToLower()))
            .ToList();
    }

    /// <summary>
    /// Check if input is a partial match (starts with)
    /// </summary>
    public static bool IsPartialMatch(string input, string candidate)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(candidate))
        {
            return false;
        }

        return candidate.ToLower().StartsWith(input.ToLower());
    }

    /// <summary>
    /// Find all partial matches
    /// </summary>
    public static List<string> FindPartialMatches(string input, IEnumerable<string> candidates)
    {
        var inputLower = input.ToLower();
        return candidates
            .Where(c => c.ToLower().StartsWith(inputLower))
            .OrderBy(c => c.Length) // Prefer shorter matches
            .ToList();
    }
}
