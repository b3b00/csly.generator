// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using backtrackbench;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using benchgenerator;




public class Program

{
    public static void Main(string[] args)
    {
        BenchThisBackTrack();
        //BenchThisJson();

        //SmallBench();
    }

    private static void BenchThisJson()
    {
        var config = DefaultConfig.Instance
            .AddLogger(ConsoleLogger.Default);   // <-- FIX

        var summary = BenchmarkRunner.Run<BenchJson>( config );
    }
    
    private static void BenchThisBackTrack()
    {
        var config = DefaultConfig.Instance
            .AddLogger(ConsoleLogger.Default);   // <-- FIX

        var summary = BenchmarkRunner.Run<BenchBackTrack>( config );
    }

    private static void SmallBench()
    {
        BenchJson bench = new BenchJson();
        bench.Setup();
        Stopwatch stopWatch = new Stopwatch();
        for (int i = 0; i < 5; i++)
        {
            stopWatch.Start();
            bench.Generator();
            stopWatch.Stop();
            Console.WriteLine($"generator #{i}: {stopWatch.ElapsedMilliseconds}ms");
            stopWatch.Reset();
        }

        for (int i = 0; i < 5; i++)
        {
            stopWatch.Start();
            bench.Csly();
            stopWatch.Stop();
            Console.WriteLine($"csly #{i}: {stopWatch.ElapsedMilliseconds}ms");
            stopWatch.Reset();
        }
    }
}