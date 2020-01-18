``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836142 Hz, Resolution=352.5917 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  DefaultJob : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT


```
|                             Method |     Mean |    Error |   StdDev |   StdErr |       Min |        Q1 |   Median |       Q3 |      Max |     Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------- |---------:|---------:|---------:|---------:|----------:|----------:|---------:|---------:|---------:|---------:|-------:|------:|------:|----------:|
| AccelbufferSerializeAndDeserialize | 10.11 us | 0.197 us | 0.312 us | 0.054 us |  9.764 us |  9.815 us | 10.04 us | 10.36 us | 10.80 us | 98,887.0 | 1.1902 |     - |     - |   1.84 KB |
|  ProtobufDeserializeAndDeserialize | 12.39 us | 0.244 us | 0.387 us | 0.067 us | 11.794 us | 12.045 us | 12.38 us | 12.70 us | 13.18 us | 80,735.2 | 1.9836 |     - |     - |   3.06 KB |
