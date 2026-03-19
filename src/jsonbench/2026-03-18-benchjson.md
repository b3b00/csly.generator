```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7781/24H2/2024Update/HudsonValley)
Intel Core Ultra 7 265H 2.20GHz, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.200
  [Host]   : .NET 9.0.13 (9.0.13, 9.0.1326.6317), X64 RyuJIT x86-64-v3
  .NET 9.0 : .NET 9.0.13 (9.0.13, 9.0.1326.6317), X64 RyuJIT x86-64-v3

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean       | Error     | StdDev    | Median     |
|---------- |-----------:|----------:|----------:|-----------:|
| Generator | 6,612.4 ms | 183.10 ms | 539.88 ms | 6,521.0 ms |
| Csly      |   140.9 ms |   2.76 ms |   4.54 ms |   138.8 ms |
