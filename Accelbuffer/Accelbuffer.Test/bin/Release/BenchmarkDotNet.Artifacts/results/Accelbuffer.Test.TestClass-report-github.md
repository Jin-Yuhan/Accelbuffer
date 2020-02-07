``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)
Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores
Frequency=2836093 Hz, Resolution=352.5977 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  DefaultJob : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT


```
|                 Method |     Mean |     Error |    StdDev |    StdErr |   Median |      Min |       Q1 |       Q3 |      Max |      Op/s |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|----------:|-------:|------:|------:|----------:|
|   AccelbufferSerialize | 5.872 us | 0.0814 us | 0.0680 us | 0.0189 us | 5.860 us | 5.813 us | 5.820 us | 5.895 us | 6.056 us | 170,292.0 |      - |     - |     - |         - |
| AccelbufferDeserialize | 5.942 us | 0.1159 us | 0.1423 us | 0.0303 us | 5.903 us | 5.784 us | 5.834 us | 6.010 us | 6.385 us | 168,283.1 | 1.3046 |     - |     - |    2060 B |
|      ProtobufSerialize | 3.958 us | 0.0839 us | 0.0862 us | 0.0209 us | 3.943 us | 3.865 us | 3.880 us | 4.002 us | 4.160 us | 252,666.0 | 0.3433 |     - |     - |     548 B |
|    ProtobufDeserialize | 8.082 us | 0.3285 us | 0.7142 us | 0.0946 us | 7.796 us | 7.607 us | 7.691 us | 7.971 us | 9.949 us | 123,728.6 | 1.6327 |     - |     - |    2584 B |
