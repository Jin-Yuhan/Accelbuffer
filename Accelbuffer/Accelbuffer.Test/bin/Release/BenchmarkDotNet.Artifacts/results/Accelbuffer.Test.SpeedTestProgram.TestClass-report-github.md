``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836113 Hz, Resolution=352.5953 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  Job-SWSWQT : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT

Runtime=.NET 4.7.2  

```
|                 Method |     Mean |     Error |    StdDev |    StdErr |    Median |       Min |        Q1 |       Q3 |      Max |      Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |---------:|----------:|----------:|----------:|----------:|----------:|----------:|---------:|---------:|----------:|-------:|------:|------:|----------:|
|   AccelbufferSerialize | 1.037 us | 0.0104 us | 0.0087 us | 0.0024 us | 1.0330 us | 1.0296 us | 1.0317 us | 1.045 us | 1.057 us | 964,311.4 |      - |     - |     - |         - |
| AccelbufferDeserialize | 1.021 us | 0.0440 us | 0.1299 us | 0.0130 us | 0.9530 us | 0.8817 us | 0.8900 us | 1.162 us | 1.176 us | 979,276.8 | 0.1545 |     - |     - |     244 B |
|      ProtobufSerialize | 1.341 us | 0.0142 us | 0.0126 us | 0.0034 us | 1.3364 us | 1.3241 us | 1.3317 us | 1.349 us | 1.364 us | 745,954.7 | 0.3185 |     - |     - |     504 B |
|    ProtobufDeserialize | 2.315 us | 0.0442 us | 0.0345 us | 0.0100 us | 2.3001 us | 2.2806 us | 2.2900 us | 2.341 us | 2.395 us | 432,011.2 | 0.4730 |     - |     - |     748 B |
