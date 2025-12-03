namespace advent_of_code_25.Puzzles;

public class Puzzle1 : IPuzzle
{
    private static string[] GetRotationsFromInput()
    {
        const string path = @"InputFiles/input-1";
    
        using var sr = File.OpenText(path);
        return sr.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    private static int GetRotationsToZero(string[] rotations)
    {
        var position = 50;
        var count = 0;

        foreach (var rotation in rotations)
        {
            var operation = rotation[0];
            if (!int.TryParse(rotation[1..], out var distance))
            {
                continue;
            }

            if (operation == 'L')
            {
                position -= distance;
            }
            else
            {
                position += distance;
            }

            position %= 100;
            if (position == 0)
            {
                count++;
            }
        }

        return count;
    }

    private static int GetClicksToZero(string[] rotations)
    {
        var position = 50;
        var count = 0;

        foreach (var rotation in rotations)
        {
            var operation = rotation[0];
            if (!int.TryParse(rotation[1..], out var distance))
            {
                continue;
            }

            var newPosition = position;
            if (operation == 'L')
            {
                newPosition -= distance;
            }
            else
            {
                newPosition += distance;
            }

            var revolutions = Math.Abs(newPosition) / 100;
            if (newPosition * position < 0 || newPosition == 0)
            {
                count++;
            }

            count += revolutions;
            position = newPosition % 100;
        }

        return count;
    }

    public void Solution()
    {
        var rotations = GetRotationsFromInput();
        Console.WriteLine(GetRotationsToZero(rotations));
        Console.WriteLine(GetClicksToZero(rotations));
    }
}