``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836025 Hz, Resolution=352.6062 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  Job-SWSWQT : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT

Runtime=.NET 4.7.2  

```
|                 Method |       Mean |    Error |   StdDev |  StdErr |        Min |         Q1 |     Median |         Q3 |        Max |        Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |-----------:|---------:|---------:|--------:|-----------:|-----------:|-----------:|-----------:|-----------:|------------:|-------:|------:|------:|----------:|
|   AccelbufferSerialize | 1,049.0 ns |  8.39 ns |  7.44 ns | 1.99 ns | 1,040.5 ns | 1,045.3 ns | 1,046.1 ns | 1,050.7 ns | 1,067.0 ns |   953,290.1 |      - |     - |     - |         - |
| AccelbufferDeserialize |   897.3 ns |  6.39 ns |  5.98 ns | 1.54 ns |   889.2 ns |   893.2 ns |   896.2 ns |   901.3 ns |   912.6 ns | 1,114,412.9 | 0.1545 |     - |     - |     244 B |
|      ProtobufSerialize | 1,324.1 ns | 11.47 ns | 10.17 ns | 2.72 ns | 1,313.7 ns | 1,317.0 ns | 1,321.0 ns | 1,327.3 ns | 1,348.5 ns |   755,206.3 | 0.3185 |     - |     - |     504 B |
|    ProtobufDeserialize | 2,550.1 ns | 24.72 ns | 23.12 ns | 5.97 ns | 2,519.8 ns | 2,532.1 ns | 2,542.8 ns | 2,570.8 ns | 2,591.7 ns |   392,146.9 | 0.4730 |     - |     - |     748 B |
