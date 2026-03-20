```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7979/24H2/2024Update/HudsonValley)
Intel Core Ultra 7 265H 2.20GHz, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.201
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3
  .NET 9.0 : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3

Runtime=.NET 9.0  

```
| Method    | Job      | IterationCount | LaunchCount | WarmupCount | Mean     | Error     | StdDev   | Ratio | RatioSD | Baseline | Gen0       | Gen1      | Gen2      | Allocated | Alloc Ratio |
|---------- |--------- |--------------- |------------ |------------ |---------:|----------:|---------:|------:|--------:|--------- |-----------:|----------:|----------:|----------:|------------:|
| Generator | .NET 9.0 | Default        | Default     | Default     | 124.9 ms |   2.47 ms |  4.13 ms |  0.83 |    0.04 | No       | 17000.0000 | 5000.0000 | 1400.0000 |  192.4 MB |        0.93 |
| Csly      | .NET 9.0 | Default        | Default     | Default     | 151.0 ms |   2.99 ms |  4.74 ms |  1.00 |    0.04 | Yes      | 19000.0000 | 7000.0000 | 2666.6667 | 207.52 MB |        1.00 |
|           |          |                |             |             |          |           |          |       |         |          |            |           |           |           |             |
| Generator | ShortRun | 3              | 1           | 3           | 130.8 ms |  52.81 ms |  2.89 ms |  0.80 |    0.09 | No       | 17250.0000 | 5000.0000 | 1500.0000 |  192.4 MB |        0.93 |
| Csly      | ShortRun | 3              | 1           | 3           | 166.0 ms | 402.39 ms | 22.06 ms |  1.01 |    0.16 | Yes      | 19000.0000 | 7000.0000 | 2666.6667 | 207.52 MB |        1.00 |
