using System;
using System.Diagnostics;

public class MethodTimer
{
    // Method to time the execution of another method and return only the elapsed time
    public static long TimeMethod(Action method)
    {
        // Create a Stopwatch instance to measure time
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Call the method
        method();

        // Stop the stopwatch
        stopwatch.Stop();

        // Return the elapsed time in milliseconds
        return stopwatch.ElapsedMilliseconds;
    }
}
