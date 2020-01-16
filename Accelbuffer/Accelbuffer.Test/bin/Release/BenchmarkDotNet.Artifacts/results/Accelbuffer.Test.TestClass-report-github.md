``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836162 Hz, Resolution=352.5892 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  DefaultJob : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT


```
|                             Method |     Mean |    Error |   StdDev |   StdErr |      Min |       Q1 |   Median |       Q3 |      Max |     Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------:|------:|------:|----------:|
| AccelbufferSerializeAndDeserialize | 10.66 us | 0.093 us | 0.082 us | 0.022 us | 10.57 us | 10.60 us | 10.63 us | 10.72 us | 10.85 us | 93,790.7 | 1.1902 |     - |     - |   1.84 KB |
|  ProtobufDeserializeAndDeserialize | 11.90 us | 0.114 us | 0.106 us | 0.027 us | 11.78 us | 11.82 us | 11.88 us | 12.03 us | 12.07 us | 84,001.9 | 1.9836 |     - |     - |   3.06 KB |
