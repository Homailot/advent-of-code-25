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
    
    private static int CountAvailableRolls(List<List<int>> rolls)
    {
        var count = 0;
        for (var rowI = 0; rowI < rolls.Count; rowI++)
        {
            var row = rolls[rowI];

            for (var colI = 0; colI < row.Count; colI++)
            {
                if (row[colI] == 0)
                {
                    continue;
                }
                
                var sum = 0;
                for (var i = Math.Max(0, rowI - 1); i <= Math.Min(rolls.Count - 1, rowI + 1); i++)
                {
                    for (var j = Math.Max(0, colI - 1); j <= Math.Min(row.Count - 1, colI + 1); j++)
                    {
                        sum += rolls[i][j];
                    }
                }

                if (sum <= 4)
                {
                    count++;
                }
            }
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
                    if (row[colI] == 0)
                    {
                        newRow.Add(0);
                        continue;
                    }
                
                    var sum = 0;
                    for (var i = Math.Max(0, rowI - 1); i <= Math.Min(rolls.Count - 1, rowI + 1); i++)
                    {
                        for (var j = Math.Max(0, colI - 1); j <= Math.Min(row.Count - 1, colI + 1); j++)
                        {
                            sum += rolls[i][j];
                        }
                    }

                    if (sum <= 4)
                    {
                        count++;
                        exhausted = false;
                        newRow.Add(0);
                    }
                    else
                    {
                        newRow.Add(1); 
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