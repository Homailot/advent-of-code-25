using System.Diagnostics;

namespace advent_of_code_25;

public static class Timer
{
    public static void TimeOperations(Action operation, long numIterations = 10000)
    {
        var nanosecondPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

        // Define variables for operation statistics.
        var numTicks = 0L;
        var maxTicks = 0L;
        var minTicks = long.MaxValue;
        var indexFastest = -1L;
        var indexSlowest = -1L;
        var milliSec = 0L;

        var time10KOperations = Stopwatch.StartNew();

        // Run the current operation 10001 times.
        // The first execution time will be tossed
        // out, since it can skew the average time.

        for (var i = 0L; i <= numIterations; i++)
        {
            var ticksThisTime = 0L;

            var timePerParse = Stopwatch.StartNew();
            operation.Invoke();
            timePerParse.Stop();
            
            ticksThisTime = timePerParse.ElapsedTicks;

            // Skip over the time for the first operation,
            // just in case it caused a one-time
            // performance hit.
            if (i == 0)
            {
                time10KOperations.Reset();
                time10KOperations.Start();
            }
            else
            {
                // Update operation statistics
                // for iterations 1-10000.
                if (maxTicks < ticksThisTime)
                {
                    indexSlowest = i;
                    maxTicks = ticksThisTime;
                }

                if (minTicks > ticksThisTime)
                {
                    indexFastest = i;
                    minTicks = ticksThisTime;
                }

                numTicks += ticksThisTime;
            }
        }

        // Display the statistics for 10000 iterations.

        time10KOperations.Stop();
        milliSec = time10KOperations.ElapsedMilliseconds;

        Console.WriteLine();
        Console.WriteLine("  Slowest time:  #{0}/{1} = {2} ticks",
            indexSlowest, numIterations, maxTicks);
        Console.WriteLine("  Fastest time:  #{0}/{1} = {2} ticks",
            indexFastest, numIterations, minTicks);
        Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds",
            numTicks / numIterations,
            (numTicks * nanosecondPerTick) / numIterations);
        Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds",
            numIterations, milliSec);
    }
}