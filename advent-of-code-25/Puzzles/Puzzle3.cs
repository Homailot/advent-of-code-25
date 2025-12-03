namespace advent_of_code_25.Puzzles;

public class Puzzle3: IPuzzle
{
    private List<List<int>> GetBanksFromInput()
    {
        const string path = @"InputFiles/input-3";

        using var sr = File.OpenText(path);
        return sr.ReadToEnd()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(bankString => 
                bankString.Select(joltage => int.Parse(joltage.ToString())).ToList())
            .ToList();
    }

    private int GetMaxJoltage(List<List<int>> banks)
    {
        var sum = 0;
        foreach (var bank in banks)
        {
            var maxValue = 0;
            var firstDigit = int.MinValue;
            var secondDigit = int.MinValue;

            foreach (var joltage in bank)
            {
                if (joltage > secondDigit)
                {
                    secondDigit = joltage;
                    maxValue = firstDigit * 10 + secondDigit;
                }
                
                if (joltage > firstDigit)
                {
                    firstDigit = joltage;
                    secondDigit = int.MinValue;
                }
            }
            
            sum += maxValue;
        }

        return sum;
    }

    private void ZeroToEnd(ref int[] digits, int from)
    {
        for (var i = from; i < digits.Length; i++)
        {
            digits[i] = int.MinValue;
        } 
    }
    
    private long GetMaxJoltageTwelve(List<List<int>> banks)
    {
        var sum = 0L;
        foreach (var bank in banks)
        {
            var digits = Enumerable.Repeat(int.MinValue, 12).ToArray(); 

            for (var joltageIndex = 0; joltageIndex < bank.Count; joltageIndex++)
            {
                var joltage = bank[joltageIndex];
                for (var digitIndex = 0; digitIndex < digits.Length; digitIndex++)
                {
                    var digit = digits[digitIndex];
                    var remainingBankDigits = bank.Count - joltageIndex;
                    var remainingDigits = digits.Length - digitIndex;

                    if (joltage > digit && remainingBankDigits >= remainingDigits)
                    {
                        digits[digitIndex] = joltage;
                        ZeroToEnd(ref digits, digitIndex+1);
                        break;
                    }
                } 
            }

            var value = 0L;
            foreach (var digit in digits)
            {
                value = value * 10L + digit;
            }
            
            sum += value;
        }

        return sum;
    }
    
    public void Solution()
    {
        var banks = GetBanksFromInput();
        Console.WriteLine(GetMaxJoltage(banks));
        Console.WriteLine(GetMaxJoltageTwelve(banks));
    }
}