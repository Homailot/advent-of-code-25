using System.Collections;

namespace advent_of_code_25.Puzzles;

public class Puzzle9 : IPuzzle
{
    private record Tile(int X, int Y);
    private record Vector(short X, short Y);

    private static List<Tile> GetTiles()
    {
        const string path = @"InputFiles/input-9-test-3";

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

    private static int Modulo(int a, int b)
    {
        var result = a % b;
        return result < 0 ? result + b : result;
    }

    // not mathematically correct, but works since adjacent tiles are connected by straight lines
    private static Vector GetUnitVector(Tile from, Tile to)
    {
        var horizontal = to.X - from.X;
        if (horizontal != 0)
        {
            return new Vector((short)(horizontal / Math.Abs(horizontal)), 0);
        }
        
        var vertical = to.Y - from.Y;
        return vertical != 0 ? new Vector(0, (short)(vertical / Math.Abs(vertical))) : new Vector(0, 0);
    }

    // > 0: +90º; < 0: -90º; = 0: -180º or 180º
    private static int CalculateAngleSign(Vector first, Vector second)
    {
        return -first.X * second.Y + first.Y * second.X;
    } 
    
    // > 0: angles are on the inside of the shape; < 0: angles are on the outside of the shape;
    private static short GetShapeDirection(List<Tile> tiles)
    {
        var sum = 0;
        // assuming tiles has at least three tiles
        for (var i = 0; i < tiles.Count; i++)
        {
            var first = GetUnitVector(tiles[i], tiles[Modulo(i - 1, tiles.Count)]);
            var second = GetUnitVector(tiles[i], tiles[Modulo(i + 1, tiles.Count)]);

            sum += CalculateAngleSign(first, second);
        }

        return sum == 0 ? (short)0 : (short) (sum / Math.Abs(sum));
    }

    private static Dictionary<Tile, HashSet<Vector>> GetEdges(List<Tile> tiles)
    {
        var shapeDirection = GetShapeDirection(tiles);
        var edges = new Dictionary<Tile, HashSet<Vector>>();

        for (var i = 0; i < tiles.Count; i++)
        {
            var from = tiles[Modulo(i - 1, tiles.Count)];
            var to = tiles[i];

            var horizontal = to.X - from.X;
            var vertical = to.Y - from.Y;

            var horizontalUnit = horizontal == 0 ? (short) 0 : (short) (horizontal / Math.Abs(horizontal));
            var verticalUnit = vertical == 0 ? (short) 0 : (short) (vertical / Math.Abs(vertical));

            var forbiddenMovement = new Vector((short)(verticalUnit * shapeDirection),(short) (-horizontalUnit * shapeDirection));
            
            var increment = new Vector(horizontalUnit, verticalUnit);
            var current = from;
            while (true)
            {
                if (edges.TryGetValue(current, out var set))
                {
                    set.Add(forbiddenMovement);
                }
                else
                {
                    edges[current] = new HashSet<Vector>([forbiddenMovement]);
                }

                if (current == to)
                {
                    break;
                }

                current = new Tile(current.X + increment.X, current.Y + increment.Y);
            }
        }
        
        return edges;
    }

    private static List<BitArray> FillInterior(List<Tile> tiles)
    {
        var edges = GetEdges(tiles);
        var startPosition = tiles[0];
        var queue = new Queue<Tile>([startPosition]);
        var visited = new HashSet<Tile>();
        var maxX = (tiles.MaxBy(t => t.X)?.X + 1) ?? 0;
        var maxY = tiles.MaxBy(t => t.Y)?.Y + 1 ?? 0;
        var grid = new List<BitArray>();

        for (int y = 0; y < maxY; y++)
        {
            grid.Add(new BitArray(maxX));
        }

        var c = 0;
        while (queue.Count > 0)
        {
            if (c == 0)
            {
                Console.WriteLine(queue.Count);
            }
            c = Modulo(c + 1, 1000);
            var tile = queue.Dequeue();
            visited.Add(tile);
            grid[tile.Y].Set(tile.X, true);

            var movements = new HashSet<Vector>([
                new Vector(-1, 0),
                new Vector(1, 0),
                new Vector(0, 1),
                new Vector(0, -1)
            ]);
            
            if (edges.TryGetValue(tile, out var forbiddenMovements))
            {
                movements.ExceptWith(forbiddenMovements);     
            }

            foreach (var movement in movements)
            {
                var newTile = new Tile(tile.X + movement.X, tile.Y + movement.Y);
                if (!visited.Contains(newTile))
                {
                    queue.Enqueue(newTile);
                }
            } 
        }

        return grid;
    }
    
    private static long GetLargestColoredArea(List<Tile> tiles)
    {
        var grid = FillInterior(tiles);

        using StreamWriter outputFile = new StreamWriter("OutputGrid.txt");
        foreach (var array in grid)
        {
            foreach (var bit in array)
            {
                outputFile.Write((bool) bit? "#" : ".");
            }
            outputFile.WriteLine();
        }

        return 0;
    }

    public void Solution()
    {
        var tiles = GetTiles();
        Console.WriteLine(GetLargestArea(tiles));
        Console.WriteLine(GetLargestColoredArea(tiles));
    }
}