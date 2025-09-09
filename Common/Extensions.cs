namespace SudokuStepByStep.Common;

// Helper extension for combinations
public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        if (k == 0) yield return new List<T>();
        else
        {
            int i = 0;

            foreach (var element in elements)
            {
                foreach (var combo in elements.Skip(i + 1).Combinations(k - 1))
                {
                    yield return new List<T> { element }.Concat(combo);
                }

                i++;
            }
        }
    }
}
