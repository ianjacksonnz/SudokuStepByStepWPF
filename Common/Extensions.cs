namespace SudokuStepByStep.Common
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static IEnumerable<List<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            if (k == 0)
            {
                yield return new List<T>();
                yield break;
            }

            var list = elements.ToList();
            if (list.Count < k) yield break;

            // only iterate where there are enough items left to complete a combo
            for (int i = 0; i <= list.Count - k; i++)
            {
                var head = list[i];
                var tail = list.Skip(i + 1);
                foreach (var tailCombo in tail.Combinations(k - 1))
                {
                    var result = new List<T> { head };
                    result.AddRange(tailCombo);
                    yield return result;
                }
            }
        }
    }
}
