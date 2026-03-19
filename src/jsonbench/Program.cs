// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using jsonbench;
using jsonbench.csly;



var summary = BenchmarkRunner.Run<BenchJson>();

