using System.Data.SqlTypes;

namespace advent_of_code_25.Puzzles;

public class Puzzle2 : IPuzzle
{
    private static List<Tuple<long, long>> GetIDsFromInput()
    {
        const string path = @"InputFiles/input-2";
    
        using var sr = File.OpenText(path);
        var rangeStrings = sr.ReadToEnd()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(rangeString => rangeString.Split("-"));
        var ranges = new List<Tuple<long,long>>();

        foreach (var limits in rangeStrings)
        {
            if (limits.Length != 2
                || !long.TryParse(limits[0], out var start)
                || !long.TryParse(limits[1], out var end))
            {
                continue;
            }
            
            ranges.Add(new Tuple<long, long>(start, end));
        }

        return ranges;
    }

    private static long GetNumberOfDigits(long id)
    {
        var digits = 0L;
        while (id > 0L)
        {
            id /= 10L;
            digits++;
        }

        return digits;
    }

    private static long Pow(long a, long b)
    {
        var ret = 1L;
        while (b != 0L)
        {
            if ((b & 1L) == 1L)
                ret *= a;
            a *= a;
            b >>= 1;
        }
        return ret;
    }

    private static long SumInvalidIDs(IEnumerable<Tuple<long,long>> idRanges)
    {
        var sum = 0L;
        foreach (var idRange in idRanges)
        {
            var start = idRange.Item1;
            var end = idRange.Item2;

            while (start <= end)
            {
                var numDigits = GetNumberOfDigits(start);
                
                if (numDigits % 2L != 0L)
                {
                    start = Pow(10L, numDigits);
                    continue;
                }

                var factor = Pow(10L, numDigits / 2); 
                var upperHalf = start / factor;
                start = (upperHalf) * factor + (upperHalf);

                if (start > end)
                {
                    break;
                }

                if (start >= idRange.Item1)
                {
                    sum += start;
                }

                if ((upperHalf + 1) % factor == 0)
                {
                    start = Pow(10L, numDigits + 1);
                }
                else
                {
                    start = (upperHalf + 1) * factor;
                }
            }
        }

        return sum;
    }

    private static long RepeatSequence(long sequence, long sequenceLength, long times)
    {
        var ret = sequence;
        while (times > 0)
        {
            ret = ret * Pow(10L, sequenceLength) + sequence;
            times -= 1;
        }

        return ret;
    }

    private static long SumInvalidIDsAtLeastTwo(IEnumerable<Tuple<long, long>> idRanges)
    {
        var sum = 0L;
        var addedNumbers = new HashSet<long>();
        foreach (var idRange in idRanges)
        {
            var start = idRange.Item1;
            var end = idRange.Item2;

            var startDigits = GetNumberOfDigits(start);
            var endDigits = GetNumberOfDigits(end);

            for (var currentDigits = startDigits; currentDigits <= endDigits; currentDigits++)
            {
                for (var sequenceLength = 1L; sequenceLength <= currentDigits / 2L; sequenceLength++)
                {
                    if (currentDigits % sequenceLength != 0)
                    {
                        continue;
                    }
                    
                    var factor = Pow(10L, currentDigits - sequenceLength);
                    var currentSequence = start / factor;

                    while (true)
                    {
                        var currentValue = RepeatSequence(currentSequence, sequenceLength, (currentDigits / sequenceLength) - 1);
                        
                        if (currentValue > end)
                        {
                            break;
                        }

                        if (currentValue >= start && addedNumbers.Add(currentValue))
                        {
                            sum += currentValue;
                        }
                        
                        currentSequence += 1;
                        if (currentSequence % Pow(10L, sequenceLength) == 0)
                        {
                            break;
                        } 
                    }
                }
            }
        }

        return sum;
    }

    public void Solution()
    {
        var ranges = GetIDsFromInput();
        Timer.TimeOperations(() => SumInvalidIDs(ranges));
        Timer.TimeOperations(() => SumInvalidIDsAtLeastTwo(ranges));
        
        Console.WriteLine(SumInvalidIDs(ranges));
        Console.WriteLine(SumInvalidIDsAtLeastTwo(ranges));
    }
}