``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836123 Hz, Resolution=352.5940 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  DefaultJob : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT


```
|                             Method |     Mean |    Error |   StdDev |   StdErr |      Min |       Q1 |   Median |       Q3 |      Max |     Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------:|------:|------:|----------:|
| AccelbufferSerializeAndDeserialize | 10.85 us | 0.265 us | 0.505 us | 0.075 us | 10.64 us | 10.68 us | 10.72 us | 10.80 us | 13.35 us | 92,206.5 | 1.1902 |     - |     - |   1.84 KB |
|  ProtobufDeserializeAndDeserialize | 11.95 us | 0.074 us | 0.066 us | 0.018 us | 11.87 us | 11.90 us | 11.92 us | 11.99 us | 12.07 us | 83,681.4 | 1.9836 |     - |     - |   3.06 KB |
