using System.Numerics;

namespace advent_of_code_25.Puzzles;

public class Puzzle8 : IPuzzle
{
    private record JunctionBox(long X, long Y, long Z);
    private record Circuits(List<HashSet<JunctionBox>> Items, (JunctionBox, JunctionBox)? LastPair);
    
    private static List<JunctionBox> GetJunctionBoxes()
    {
        const string path = @"InputFiles/input-8";

        return File.ReadLines(path)
            .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
            .Select(numbers => new JunctionBox(numbers[0], numbers[1], numbers[2]))
            .ToList();
    }

    private static double GetDistance(JunctionBox left, JunctionBox right)
    {
        var (p1, p2, p3) = left;
        var (q1, q2, q3) = right;

        return Math.Sqrt(Math.Pow(p1 - q1, 2) + Math.Pow(p2 - q2, 2) + Math.Pow(p3 - q3, 2));
    }

    private static Circuits ConnectCircuitsUntilSingle(List<JunctionBox> boxes, long limit = 1000)
    {
        var sortedPairs = new PriorityQueue<(JunctionBox, JunctionBox), double>();
        var circuits = boxes.Select(box => new HashSet<JunctionBox> {box}).ToList();
        
        for (var i = 0; i < boxes.Count; i++)
        {
            for (var j = i + 1; j < boxes.Count; j++)
            {
                var left = boxes[i];
                var right = boxes[j];
                
                sortedPairs.Enqueue((left, right), GetDistance(left, right));
            }
        }

        (JunctionBox, JunctionBox)? last = null;
        for (var i = 0; i < limit && sortedPairs.Count > 0 && circuits.Count > 1; i++)
        {
            var (left, right) = sortedPairs.Dequeue();
            last = (left, right);
            var leftCircuitIndex = circuits.FindIndex(circuit => circuit.Contains(left));
            var rightCircuitIndex = circuits.FindIndex(circuit => circuit.Contains(right));

            var leftCircuit = circuits[leftCircuitIndex];
            var rightCircuit = circuits[rightCircuitIndex];

            leftCircuit.UnionWith(rightCircuit);
            if (leftCircuitIndex != rightCircuitIndex)
            {
                circuits.RemoveAt(rightCircuitIndex);
            }
        }

        return new Circuits(circuits, last);
    }

    private static long GetCircuitSizes(List<JunctionBox> boxes, long limit = 1000)
    {
        var circuits = ConnectCircuitsUntilSingle(boxes, limit).Items; 
        
        circuits.Sort((left, right) => right.Count - left.Count);
        return circuits.Take(3).Aggregate(1, (current, next) => current * next.Count);
    }

    private static long GetLastPairDistance(List<JunctionBox> boxes)
    {
        var lastPair = ConnectCircuitsUntilSingle(boxes, long.MaxValue).LastPair;
        return (lastPair?.Item1.X ?? 0) * (lastPair?.Item2.X ?? 0);
    }
    
    public void Solution()
    {
        var boxes = GetJunctionBoxes();
        Console.WriteLine(boxes);
        Console.WriteLine(GetCircuitSizes(boxes));
        Console.WriteLine(GetLastPairDistance(boxes));
    }
}