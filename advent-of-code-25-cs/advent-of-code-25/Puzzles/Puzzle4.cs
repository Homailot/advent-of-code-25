namespace advent_of_code_25.Puzzles;

public class Puzzle4 : IPuzzle
{
    private static List<List<int>> GetRollsFromInput()
    {
        const string path = @"InputFiles/input-4";

        return File.ReadLines(path)
            .Select(line => line.Select(slot => slot == '@' ? 1 : 0).ToList())
            .ToList();
    }

    private static int CountNeighbors(ref List<List<int>> rolls, int row, int col)
    {
        var sum = 0;
        for (var i = Math.Max(0, row - 1); i <= Math.Min(rolls.Count - 1, row + 1); i++)
        {
            for (var j = Math.Max(0, col - 1); j <= Math.Min(rolls[i].Count - 1, col + 1); j++)
            {
                sum += rolls[i][j];
            }
        }

        return sum;
    }
    
    private static int CountAvailableRolls(List<List<int>> rolls)
    {
        var count = 0;
        for (var rowI = 0; rowI < rolls.Count; rowI++)
        {
            count += rolls[rowI]
                .Where((t, colI) => t != 0 && CountNeighbors(ref rolls, rowI, colI) <= 4)
                .Count();
        }

        return count;
    }
    
    private static int CountAvailableRollsUntilExhausted(List<List<int>> rolls)
    {
        var count = 0;
        var exhausted = false;
        while (!exhausted)
        {
            exhausted = true;
            List<List<int>> newRolls = [];
            
            for (var rowI = 0; rowI < rolls.Count; rowI++)
            {
                var row = rolls[rowI];
                List<int> newRow = [];

                for (var colI = 0; colI < row.Count; colI++)
                {
                    if (row[colI] == 0 || CountNeighbors(ref rolls, rowI, colI) > 4)
                    {
                        newRow.Add(row[colI]);
                    }
                    else
                    {
                        count++;
                        exhausted = false;
                        newRow.Add(0);
                    }
                }
                
                newRolls.Add(newRow);
            }

            rolls = newRolls;
        } 

        return count;
    }
    
    public void Solution()
    {
        var rolls = GetRollsFromInput();
        Console.WriteLine(CountAvailableRolls(rolls));
        Console.WriteLine(CountAvailableRollsUntilExhausted(rolls));
    }
}