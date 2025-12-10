namespace advent_of_code_25.Puzzles;

public class Puzzle9 : IPuzzle
{
    private record Tile(long X, long Y);

    private static List<Tile> GetTiles()
    {
        const string path = @"InputFiles/input-9";

        return File.ReadLines(path)
            .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
            .Select(numbers => new Tile(numbers[0], numbers[1]))
            .ToList();
    }

    private static long GetArea(Tile left, Tile right)
    {
        var (x1, y1) = left;
        var (x2, y2) = right;

        return (Math.Abs(x2 - x1) + 1) * (Math.Abs(y2 - y1) + 1);
    }

    private static long GetLargestArea(List<Tile> tiles)
    {
        var comparer = Comparer<long>.Create((x, y) => y.CompareTo(x));
        var sortedPairs = new PriorityQueue<(Tile, Tile), long>(comparer);
        for (var i = 0; i < tiles.Count; i++)
        {
            for (var j = i + 1; j < tiles.Count; j++)
            {
                var left = tiles[i];
                var right = tiles[j];
                
                sortedPairs.Enqueue((left, right), GetArea(left, right));
            }
        }

        sortedPairs.TryDequeue(out var element, out var priority);
        Console.WriteLine(element);
        return priority;
    }

    public void Solution()
    {
        var tiles = GetTiles();
        Console.WriteLine(GetLargestArea(tiles));
    }
}