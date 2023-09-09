namespace Unifico.Core;

public static class RandomUtils
{
    public static void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Shared.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static T Choice<T>(IEnumerable<T> values)
    {
        var list = values.ToList();
        var k = Random.Shared.Next(list.Count);
        return list[k];
    }

    public static T Choice<T>() where T : Enum
    {
        var values = (T[])Enum.GetValues(typeof(T));
        return Choice(values);
    }
}