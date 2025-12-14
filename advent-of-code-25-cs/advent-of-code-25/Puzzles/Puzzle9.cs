using System.Collections;

namespace advent_of_code_25.Puzzles;

public class Puzzle9 : IPuzzle
{
    private record Tile(int X, int Y);

    private record Edge(int At, int From, int To);
    private record SortedEdges(List<Edge> HorizontalEdges, List<Edge> VerticalEdges);

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
        var maxArea = long.MinValue;
        
        for (var i = 0; i < tiles.Count; i++)
        {
            for (var j = i + 1; j < tiles.Count; j++)
            {
                var left = tiles[i];
                var right = tiles[j];

                var area = GetArea(left, right);
                if (area > maxArea)
                {
                    maxArea = area;
                }
            }
        }

        return maxArea;
    }

    private static (int From, int To) GetMinMax(int left, int right)
    {
        return left > right ? (right, left) : (left, right);
    }

    private static (int From, int To) GetIntersection((int From, int To) limit, (int From, int To) edge)
    {
        if (edge.From > limit.To || edge.To < limit.From)
        {
            return (-1, -1);
        }

        return (Math.Max(edge.From, limit.From), Math.Min(edge.To, limit.To));
    }
    
    private static SortedEdges GetSortedEdges(List<Tile> tiles)
    {
        List<Edge> horizontalEdges = [];
        List<Edge> verticalEdges = [];

        var (lastX, lastY) = tiles[^1];
        foreach (var (x, y) in tiles)
        {
            if (lastX == x)
            {
                var (from, to) = GetMinMax(lastY, y);
                verticalEdges.Add(
                    new Edge(
                        x,
                        from, 
                        to
                    ));
            }
            else
            {
                var (from, to) = GetMinMax(lastX, x);
                horizontalEdges.Add(
                    new Edge(
                        y,
                        from,
                        to
                    ));
            }

            (lastX, lastY) = (x, y);
        }
        
        verticalEdges.Sort((left, right) => left.At.CompareTo(right.At));
        horizontalEdges.Sort((left, right) => left.At.CompareTo(right.At));
        return new SortedEdges(horizontalEdges, verticalEdges);
    }

    private static void PrintArray(ref BitArray array)
    {
        foreach (var bit in array)
        {
            Console.Write(bit.Equals(true) ? 1 : 0); 
        }
        Console.WriteLine();
    }


    private static bool CastShadow(ref Tile left, ref Tile right, (int From, int To) search, (int From, int To) bounds,
        List<Edge> edges)
    {
        var accumulate = new BitArray(bounds.To - bounds.From + 1, false);
                
        var currentPosition = edges[0].At;
        foreach (var edge in edges)
        {
            if (currentPosition != edge.At)
            {
                //Console.WriteLine($"Column for tile {left} to tile {right} at x = {currentPosition}");
                //PrintArray(ref accumulate);
                if (edge.At > search.From && !accumulate.HasAllSet())
                {
                   // Console.WriteLine("Rejected!");
                    return false;
                }

                // this is not correct for areas of single lines, but those shouldn't be the largest areas
                if (edge.At >= search.To)
                {
                    //Console.WriteLine("Is Valid!");
                    return true;
                }

                currentPosition = edge.At;
            }

            var (from, to) = GetIntersection(bounds, (edge.From, edge.To));
            //Console.WriteLine($"Intersect at {(from, to)}");
            if (from == -1)
            {
                continue;
            }

            var edgeArray = new BitArray(to - from + 1, true);
            edgeArray.Length = bounds.To - bounds.From + 1;
            edgeArray.LeftShift(from - bounds.From);

            accumulate.Xor(edgeArray);
            accumulate.Set(from - bounds.From, true);
            accumulate.Set(to - bounds.From, true);
        }
        //Console.WriteLine($"Vertical Column for tile {left} to tile {right} at x = {currentPosition}");
        //PrintArray(ref accumulate);
        //Console.WriteLine("Rejected!");
        return false;
    } 
    
    
    private static long GetLargestColoredArea(List<Tile> tiles)
    {
        var edges = GetSortedEdges(tiles);
        var maxArea = long.MinValue;

        for (var i = 0; i < tiles.Count; i++)
        {
            for (var j = i + 1; j < tiles.Count; j++)
            {
                var left = tiles[i];
                var right = tiles[j];

                var minMaxY = GetMinMax(left.Y, right.Y);
                var minMaxX = GetMinMax(left.X, right.X);

                var verticalAccept = CastShadow(ref left, ref right, minMaxX, minMaxY, edges.VerticalEdges);
                var horizontalAccept = CastShadow(ref left, ref right, minMaxY, minMaxX, edges.HorizontalEdges);

                if (verticalAccept && horizontalAccept)
                {
                    var area = GetArea(left, right);

                    if (area > maxArea)
                    {
                        maxArea = area;
                    }
                }
            }
        }

        return maxArea;
    }

    public void Solution()
    {
        var tiles = GetTiles();
        Console.WriteLine(GetLargestArea(tiles));
        Console.WriteLine(GetLargestColoredArea(tiles));
    }
}