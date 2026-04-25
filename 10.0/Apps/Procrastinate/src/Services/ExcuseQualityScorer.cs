using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Uses NLEmbeddingGenerator (IEmbeddingGenerator) to score excuse quality
/// by comparing against a set of known-good excuses via cosine similarity.
/// </summary>
public static class ExcuseQualityScorer
{
    private static readonly string[] GoldenExcuses =
    {
        "I can't come in today because my pet goldfish is having an existential crisis and needs emotional support.",
        "Sorry, but Mercury is in retrograde and my horoscope specifically warned against productivity.",
        "I would, but a mysterious stranger told me that if I do any work today the timeline collapses.",
        "I tried, but my WiFi became sentient and is now holding my files hostage.",
        "I'd love to help, but my neighbor's cat has filed a restraining order against my productivity.",
        "Unfortunately, I accidentally invented time travel and I'm stuck in yesterday.",
        "I must decline because a fortune cookie predicted catastrophe if I'm productive today.",
        "Not today — my shadow called in sick and I can't function without it.",
        "I was going to, but my coffee achieved consciousness and we need to have a talk.",
        "Sorry, a philosophical duck won't leave my porch and I need to hear its argument first."
    };

    // Cache golden embeddings to avoid recomputing on every call
    private static GeneratedEmbeddings<Embedding<float>>? _cachedGoldenEmbeddings;
    private static IEmbeddingGenerator<string, Embedding<float>>? _cachedGenerator;

    /// <summary>
    /// Score an excuse against golden examples. Returns 0.0 to 1.0 (higher = more similar to good excuses).
    /// </summary>
    public static async Task<float> ScoreAsync(
        IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator,
        string excuse)
    {
        if (embeddingGenerator is null || string.IsNullOrWhiteSpace(excuse))
            return 0.5f;

        try
        {
            var excuseEmbeddings = await embeddingGenerator.GenerateAsync([excuse]);
            var excuseVector = excuseEmbeddings[0].Vector;

            // Reuse cached golden embeddings if same generator instance
            if (_cachedGoldenEmbeddings is null || _cachedGenerator != embeddingGenerator)
            {
                _cachedGoldenEmbeddings = await embeddingGenerator.GenerateAsync(GoldenExcuses.ToList());
                _cachedGenerator = embeddingGenerator;
            }

            float maxSimilarity = 0f;
            foreach (var golden in _cachedGoldenEmbeddings)
            {
                var similarity = CosineSimilarity(excuseVector, golden.Vector);
                if (similarity > maxSimilarity)
                    maxSimilarity = similarity;
            }

            return maxSimilarity;
        }
        catch
        {
            return 0.5f;
        }
    }

    private static float CosineSimilarity(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
    {
        var spanA = a.Span;
        var spanB = b.Span;
        var len = Math.Min(spanA.Length, spanB.Length);

        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < len; i++)
        {
            dot += spanA[i] * spanB[i];
            magA += spanA[i] * spanA[i];
            magB += spanB[i] * spanB[i];
        }

        var denom = MathF.Sqrt(magA) * MathF.Sqrt(magB);
        return denom < 1e-10f ? 0f : dot / denom;
    }
}
