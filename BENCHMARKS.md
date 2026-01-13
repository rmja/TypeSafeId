
# TypeSafeId Benchmarks

This document presents performance benchmarks for the TypeSafeId library and related implementations. The benchmarks are designed to compare the efficiency of various operations such as ID generation, parsing, and serialization across different libraries and approaches.

The results below are generated using BenchmarkDotNet and are intended to provide insight into the relative performance of TypeSafeId and alternative solutions. Benchmarks are run on a modern Windows system with .NET 10. 


## Libraries Compared

The following libraries and implementations are included in these benchmarks:

- **TypeSafeId** ([GitHub](https://github.com/rmja/TypeSafeId)) — The library under test, providing type-safe, prefix-based unique identifiers.
- **FastIDs** ([GitHub](https://github.com/firenero/TypeId)) — Performance-oriented TypeId C# library.
- **TcKs** ([GitHub](https://github.com/TenCoKaciStromy/typeid-dotnet)) — The .NET implementation of TypeID.
- **Cbuctok** ([GitHub](https://github.com/cbuctok/typeId)) — Type-safe, K-sortable, globally unique identifier inspired by Stripe IDs.


## TypeId Generation

This benchmark compares the generation of a new TypeId (benchmark name: `TypeIdGeneration`).

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen AI 9 HX PRO 370 w/ Radeon 890M 2.00GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method                       | PrefixLength | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------- |------------- |----------:|---------:|---------:|----------:|------:|--------:|-------:|----------:|------------:|
| **TypeSafeIdGenericGenerate**    | **0**            |  **78.07 ns** | **0.217 ns** | **0.193 ns** |  **78.08 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericGenerate | 0            |  78.02 ns | 0.213 ns | 0.189 ns |  77.98 ns |  1.00 |    0.00 |      - |         - |          NA |
| FastIdsGenerate              | 0            |  94.81 ns | 0.121 ns | 0.101 ns |  94.83 ns |  1.21 |    0.00 |      - |         - |          NA |
| FastIdsNoCheckGenerate       | 0            |  89.68 ns | 0.756 ns | 0.707 ns |  89.33 ns |  1.15 |    0.01 |      - |         - |          NA |
| TcKsGenerate                 | 0            | 159.70 ns | 3.197 ns | 3.283 ns | 157.45 ns |  2.05 |    0.04 | 0.0324 |     272 B |          NA |
| CbuctokGenerate              | 0            | 175.30 ns | 3.530 ns | 3.923 ns | 177.23 ns |  2.25 |    0.05 | 0.0515 |     432 B |          NA |
|                              |              |           |          |          |           |       |         |        |           |             |
| **TypeSafeIdGenericGenerate**    | **5**            |  **78.50 ns** | **0.212 ns** | **0.177 ns** |  **78.49 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericGenerate | 5            |  78.30 ns | 0.149 ns | 0.132 ns |  78.35 ns |  1.00 |    0.00 |      - |         - |          NA |
| FastIdsGenerate              | 5            |  95.92 ns | 0.585 ns | 0.548 ns |  96.07 ns |  1.22 |    0.01 |      - |         - |          NA |
| FastIdsNoCheckGenerate       | 5            |  89.26 ns | 0.303 ns | 0.253 ns |  89.24 ns |  1.14 |    0.00 |      - |         - |          NA |
| TcKsGenerate                 | 5            | 161.31 ns | 1.558 ns | 1.457 ns | 161.06 ns |  2.05 |    0.02 | 0.0334 |     280 B |          NA |
| CbuctokGenerate              | 5            | 165.64 ns | 0.684 ns | 0.571 ns | 165.48 ns |  2.11 |    0.01 | 0.0515 |     432 B |          NA |
|                              |              |           |          |          |           |       |         |        |           |             |
| **TypeSafeIdGenericGenerate**    | **10**           |  **77.93 ns** | **0.101 ns** | **0.084 ns** |  **77.90 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericGenerate | 10           |  77.86 ns | 0.373 ns | 0.312 ns |  77.89 ns |  1.00 |    0.00 |      - |         - |          NA |
| FastIdsGenerate              | 10           |  93.92 ns | 0.282 ns | 0.264 ns |  93.84 ns |  1.21 |    0.00 |      - |         - |          NA |
| FastIdsNoCheckGenerate       | 10           |  88.85 ns | 0.315 ns | 0.280 ns |  88.70 ns |  1.14 |    0.00 |      - |         - |          NA |
| TcKsGenerate                 | 10           | 148.97 ns | 2.944 ns | 4.998 ns | 145.76 ns |  1.91 |    0.06 | 0.0343 |     288 B |          NA |
| CbuctokGenerate              | 10           | 148.80 ns | 0.739 ns | 0.655 ns | 148.91 ns |  1.91 |    0.01 | 0.0515 |     432 B |          NA |
|                              |              |           |          |          |           |       |         |        |           |             |
| **TypeSafeIdGenericGenerate**    | **30**           |  **78.02 ns** | **0.116 ns** | **0.097 ns** |  **78.03 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericGenerate | 30           |  78.13 ns | 0.147 ns | 0.130 ns |  78.15 ns |  1.00 |    0.00 |      - |         - |          NA |
| FastIdsGenerate              | 30           |  93.85 ns | 0.456 ns | 0.426 ns |  93.67 ns |  1.20 |    0.01 |      - |         - |          NA |
| FastIdsNoCheckGenerate       | 30           |  88.18 ns | 0.205 ns | 0.171 ns |  88.12 ns |  1.13 |    0.00 |      - |         - |          NA |
| TcKsGenerate                 | 30           | 147.93 ns | 0.697 ns | 0.582 ns | 147.88 ns |  1.90 |    0.01 | 0.0391 |     328 B |          NA |
| CbuctokGenerate              | 30           | 148.38 ns | 0.617 ns | 0.577 ns | 148.51 ns |  1.90 |    0.01 | 0.0515 |     432 B |          NA |
|                              |              |           |          |          |           |       |         |        |           |             |
| **TypeSafeIdGenericGenerate**    | **63**           |  **77.52 ns** | **0.445 ns** | **0.372 ns** |  **77.65 ns** |  **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericGenerate | 63           |  78.00 ns | 0.069 ns | 0.061 ns |  78.00 ns |  1.01 |    0.00 |      - |         - |          NA |
| FastIdsGenerate              | 63           |  94.04 ns | 0.093 ns | 0.073 ns |  94.05 ns |  1.21 |    0.01 |      - |         - |          NA |
| FastIdsNoCheckGenerate       | 63           |  88.88 ns | 0.307 ns | 0.272 ns |  88.87 ns |  1.15 |    0.01 |      - |         - |          NA |
| TcKsGenerate                 | 63           | 150.64 ns | 0.842 ns | 0.658 ns | 150.88 ns |  1.94 |    0.01 | 0.0477 |     400 B |          NA |
| CbuctokGenerate              | 63           | 147.76 ns | 0.873 ns | 0.774 ns | 147.82 ns |  1.91 |    0.01 | 0.0515 |     432 B |          NA |


## TypeId Creation from Uuid

This benchmarks compares the creation of a TypeId from an existing Uuid Guid instance (benchmark name: `TypeIdCreateFromUuid`).
This is the typical creation operation if ID's are stored as `Guid` in a database.

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen AI 9 HX PRO 370 w/ Radeon 890M 2.00GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method                     | PrefixLength | Mean       | Error     | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |------------- |-----------:|----------:|----------:|-------:|--------:|-------:|----------:|------------:|
| **TypeSafeIdGenericFromUuid**    | **0**            |  **0.2134 ns** | **0.0117 ns** | **0.0109 ns** |   **1.00** |    **0.07** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericFromUuid | 0            |  0.8446 ns | 0.0069 ns | 0.0061 ns |   3.97 |    0.20 |      - |         - |          NA |
| FastIdsFromUuid              | 0            |  0.9085 ns | 0.0047 ns | 0.0041 ns |   4.27 |    0.22 |      - |         - |          NA |
| TcKsFromUuid                 | 0            | 54.3491 ns | 0.4366 ns | 0.4084 ns | 255.32 |   13.04 | 0.0325 |     272 B |          NA |
| CbuctokFromUuid              | 0            | 62.2420 ns | 0.3217 ns | 0.3009 ns | 292.40 |   14.84 | 0.0516 |     432 B |          NA |
|                            |              |            |           |           |        |         |        |           |             |
| **TypeSafeIdGenericFromUuid**    | **5**            |  **0.2017 ns** | **0.0052 ns** | **0.0040 ns** |   **1.00** |    **0.03** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericFromUuid | 5            |  3.1257 ns | 0.0215 ns | 0.0180 ns |  15.50 |    0.32 |      - |         - |          NA |
| FastIdsFromUuid              | 5            |  2.2592 ns | 0.0200 ns | 0.0177 ns |  11.20 |    0.24 |      - |         - |          NA |
| TcKsFromUuid                 | 5            | 53.3572 ns | 0.3725 ns | 0.3303 ns | 264.58 |    5.44 | 0.0334 |     280 B |          NA |
| CbuctokFromUuid              | 5            | 69.3469 ns | 0.8604 ns | 0.7627 ns | 343.86 |    7.68 | 0.0516 |     432 B |          NA |
|                            |              |            |           |           |        |         |        |           |             |
| **TypeSafeIdGenericFromUuid**    | **10**           |  **0.2101 ns** | **0.0026 ns** | **0.0021 ns** |   **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericFromUuid | 10           |  1.4954 ns | 0.0042 ns | 0.0037 ns |   7.12 |    0.07 |      - |         - |          NA |
| FastIdsFromUuid              | 10           |  3.3152 ns | 0.0610 ns | 0.0510 ns |  15.78 |    0.28 |      - |         - |          NA |
| TcKsFromUuid                 | 10           | 53.9465 ns | 0.3048 ns | 0.2546 ns | 256.82 |    2.75 | 0.0344 |     288 B |          NA |
| CbuctokFromUuid              | 10           | 69.3106 ns | 0.5036 ns | 0.4464 ns | 329.97 |    3.80 | 0.0516 |     432 B |          NA |
|                            |              |            |           |           |        |         |        |           |             |
| **TypeSafeIdGenericFromUuid**    | **30**           |  **0.2141 ns** | **0.0046 ns** | **0.0039 ns** |   **1.00** |    **0.02** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericFromUuid | 30           |  1.4926 ns | 0.0112 ns | 0.0100 ns |   6.97 |    0.13 |      - |         - |          NA |
| FastIdsFromUuid              | 30           |  3.0692 ns | 0.0110 ns | 0.0092 ns |  14.34 |    0.26 |      - |         - |          NA |
| TcKsFromUuid                 | 30           | 56.0648 ns | 0.3639 ns | 0.3404 ns | 261.90 |    4.90 | 0.0392 |     328 B |          NA |
| CbuctokFromUuid              | 30           | 79.1997 ns | 0.2889 ns | 0.2561 ns | 369.98 |    6.67 | 0.0516 |     432 B |          NA |
|                            |              |            |           |           |        |         |        |           |             |
| **TypeSafeIdGenericFromUuid**    | **63**           |  **0.2109 ns** | **0.0016 ns** | **0.0013 ns** |   **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| TypeSafeIdNotGenericFromUuid | 63           |  1.9753 ns | 0.0183 ns | 0.0171 ns |   9.37 |    0.10 |      - |         - |          NA |
| FastIdsFromUuid              | 63           |  3.7944 ns | 0.0128 ns | 0.0114 ns |  17.99 |    0.12 |      - |         - |          NA |
| TcKsFromUuid                 | 63           | 56.9043 ns | 0.1503 ns | 0.1255 ns | 269.84 |    1.73 | 0.0478 |     400 B |          NA |
| CbuctokFromUuid              | 63           | 92.8322 ns | 0.4198 ns | 0.3722 ns | 440.20 |    3.16 | 0.0516 |     432 B |          NA |

## TypeId Parse and Uuid Access

This benchmark compares the parsing of a string representation in TypeId format into a TypeId and accessing the UUID part as a `Guid` (benchmark name: `TypeIdParseToUuid`).
This is the typical parse operation if the TypeId is obtained as a parameter, and is used for query in a database storing the TypeId as a `Guid`.

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen AI 9 HX PRO 370 w/ Radeon 890M 2.00GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method                       | PrefixLength | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------- |------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **TypeSafeIdGenericParseToUuid**       | **0**            |  **32.433 ns** | **0.1086 ns** | **0.1207 ns** |  **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| TypeSafeIdGenericTryParseToUuid    | 0            |  30.479 ns | 0.0461 ns | 0.0385 ns |  0.94 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericParseToUuid    | 0            |  33.038 ns | 0.0834 ns | 0.0739 ns |  1.02 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericTryParseToUuid | 0            |  30.296 ns | 0.1924 ns | 0.1607 ns |  0.93 |    0.01 |      - |         - |          NA |
| FastIdsParseToUuid                 | 0            |  38.026 ns | 0.0960 ns | 0.0801 ns |  1.17 |    0.00 |      - |         - |          NA |
| FastIdsTryParseToUuid              | 0            |  34.836 ns | 0.2585 ns | 0.2159 ns |  1.07 |    0.01 |      - |         - |          NA |
| TcKsParseToUuid                    | 0            |         NA |        NA |        NA |     ? |       ? |     NA |        NA |           ? |
| TcKsTryParseToUuid                 | 0            |   1.756 ns | 0.0357 ns | 0.0298 ns |  0.05 |    0.00 |      - |         - |          NA |
| CbuctokParseToUuid                 | 0            | 109.098 ns | 2.1151 ns | 2.1720 ns |  3.36 |    0.07 | 0.0381 |     320 B |          NA |
| CbuctokTryParseToUuid              | 0            |  72.997 ns | 0.6307 ns | 0.5267 ns |  2.25 |    0.02 | 0.0381 |     320 B |          NA |
|                              |              |            |           |           |       |         |        |           |             |
| **TypeSafeIdGenericParseToUuid**       | **5**            |  **28.424 ns** | **0.1183 ns** | **0.1107 ns** |  **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| TypeSafeIdGenericTryParseToUuid    | 5            |  26.720 ns | 0.0737 ns | 0.0615 ns |  0.94 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericParseToUuid    | 5            |  35.170 ns | 0.1897 ns | 0.1682 ns |  1.24 |    0.01 | 0.0038 |      32 B |          NA |
| TypeSafeIdNotGenericTryParseToUuid | 5            |  32.079 ns | 0.2756 ns | 0.2578 ns |  1.13 |    0.01 | 0.0038 |      32 B |          NA |
| FastIdsParseToUuid                 | 5            |  41.073 ns | 0.2347 ns | 0.2196 ns |  1.45 |    0.01 | 0.0038 |      32 B |          NA |
| FastIdsTryParseToUuid              | 5            |  39.416 ns | 0.3902 ns | 0.3650 ns |  1.39 |    0.01 | 0.0038 |      32 B |          NA |
| TcKsParseToUuid                    | 5            |  41.718 ns | 0.1642 ns | 0.1455 ns |  1.47 |    0.01 |      - |         - |          NA |
| TcKsTryParseToUuid                 | 5            |  41.924 ns | 0.1449 ns | 0.1210 ns |  1.47 |    0.01 |      - |         - |          NA |
| CbuctokParseToUuid                 | 5            |  83.986 ns | 0.6953 ns | 0.6164 ns |  2.95 |    0.02 | 0.0526 |     440 B |          NA |
| CbuctokTryParseToUuid              | 5            |  88.349 ns | 0.5520 ns | 0.4893 ns |  3.11 |    0.02 | 0.0526 |     440 B |          NA |
|                              |              |            |           |           |       |         |        |           |             |
| **TypeSafeIdGenericParseToUuid**       | **10**           |  **28.196 ns** | **0.0299 ns** | **0.0234 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdGenericTryParseToUuid    | 10           |  26.934 ns | 0.0773 ns | 0.0723 ns |  0.96 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericParseToUuid    | 10           |  30.927 ns | 0.1620 ns | 0.1516 ns |  1.10 |    0.01 | 0.0057 |      48 B |          NA |
| TypeSafeIdNotGenericTryParseToUuid | 10           |  30.366 ns | 0.2035 ns | 0.1804 ns |  1.08 |    0.01 | 0.0057 |      48 B |          NA |
| FastIdsParseToUuid                 | 10           |  41.528 ns | 0.0927 ns | 0.0867 ns |  1.47 |    0.00 | 0.0057 |      48 B |          NA |
| FastIdsTryParseToUuid              | 10           |  62.254 ns | 0.2293 ns | 0.1915 ns |  2.21 |    0.01 | 0.0057 |      48 B |          NA |
| TcKsParseToUuid                    | 10           |  45.887 ns | 0.0614 ns | 0.0513 ns |  1.63 |    0.00 |      - |         - |          NA |
| TcKsTryParseToUuid                 | 10           |  43.681 ns | 0.1401 ns | 0.1170 ns |  1.55 |    0.00 |      - |         - |          NA |
| CbuctokParseToUuid                 | 10           | 102.241 ns | 1.1283 ns | 1.0554 ns |  3.63 |    0.04 | 0.0545 |     456 B |          NA |
| CbuctokTryParseToUuid              | 10           |  94.371 ns | 0.6466 ns | 0.6048 ns |  3.35 |    0.02 | 0.0545 |     456 B |          NA |
|                              |              |            |           |           |       |         |        |           |             |
| **TypeSafeIdGenericParseToUuid**       | **30**           |  **28.334 ns** | **0.0846 ns** | **0.0660 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdGenericTryParseToUuid    | 30           |  26.997 ns | 0.1146 ns | 0.1016 ns |  0.95 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericParseToUuid    | 30           |  33.141 ns | 0.0984 ns | 0.0920 ns |  1.17 |    0.00 | 0.0105 |      88 B |          NA |
| TypeSafeIdNotGenericTryParseToUuid | 30           |  32.991 ns | 0.0944 ns | 0.0837 ns |  1.16 |    0.00 | 0.0105 |      88 B |          NA |
| FastIdsParseToUuid                 | 30           |  42.142 ns | 0.2229 ns | 0.1861 ns |  1.49 |    0.01 | 0.0105 |      88 B |          NA |
| FastIdsTryParseToUuid              | 30           |  58.449 ns | 0.2256 ns | 0.2000 ns |  2.06 |    0.01 | 0.0105 |      88 B |          NA |
| TcKsParseToUuid                    | 30           |  50.832 ns | 0.1342 ns | 0.1189 ns |  1.79 |    0.01 |      - |         - |          NA |
| TcKsTryParseToUuid                 | 30           |  50.704 ns | 0.1895 ns | 0.1680 ns |  1.79 |    0.01 |      - |         - |          NA |
| CbuctokParseToUuid                 | 30           | 105.187 ns | 1.3236 ns | 1.2381 ns |  3.71 |    0.04 | 0.0592 |     496 B |          NA |
| CbuctokTryParseToUuid              | 30           | 118.593 ns | 0.7938 ns | 0.7426 ns |  4.19 |    0.03 | 0.0592 |     496 B |          NA |
|                              |              |            |           |           |       |         |        |           |             |
| **TypeSafeIdGenericParseToUuid**       | **63**           |  **28.901 ns** | **0.0560 ns** | **0.0437 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| TypeSafeIdGenericTryParseToUuid    | 63           |  27.024 ns | 0.0985 ns | 0.0873 ns |  0.94 |    0.00 |      - |         - |          NA |
| TypeSafeIdNotGenericParseToUuid    | 63           |  37.216 ns | 0.2514 ns | 0.2228 ns |  1.29 |    0.01 | 0.0181 |     152 B |          NA |
| TypeSafeIdNotGenericTryParseToUuid | 63           |  35.498 ns | 0.1269 ns | 0.1059 ns |  1.23 |    0.00 | 0.0181 |     152 B |          NA |
| FastIdsParseToUuid                 | 63           |  45.429 ns | 0.2722 ns | 0.2546 ns |  1.57 |    0.01 | 0.0181 |     152 B |          NA |
| FastIdsTryParseToUuid              | 63           |  62.904 ns | 0.4238 ns | 0.3964 ns |  2.18 |    0.01 | 0.0181 |     152 B |          NA |
| TcKsParseToUuid                    | 63           |  55.723 ns | 0.1123 ns | 0.1051 ns |  1.93 |    0.00 |      - |         - |          NA |
| TcKsTryParseToUuid                 | 63           |  55.582 ns | 0.4259 ns | 0.3984 ns |  1.92 |    0.01 |      - |         - |          NA |
| CbuctokParseToUuid                 | 63           | 133.325 ns | 1.7824 ns | 1.5801 ns |  4.61 |    0.05 | 0.0668 |     560 B |          NA |
| CbuctokTryParseToUuid              | 63           | 144.891 ns | 2.7046 ns | 2.5299 ns |  5.01 |    0.09 | 0.0668 |     560 B |          NA |

Benchmarks with issues:
  TypeIdParseToUuid.TcKsParseToUuid: DefaultJob [PrefixLength=0]


## TypeId Serialization to String

This benchmark compares the formatting of a TypeId to its string representation (benchmark name: `TypeIdToString`).
This is the typical operatin when fetched database TypeIds stored as `Guid` are serialized, for example as json.

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen AI 9 HX PRO 370 w/ Radeon 890M 2.00GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method               | PrefixLength | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------- |------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| **TypeSafeIdGenericToString**    | **0**            | **18.5613 ns** | **0.2023 ns** | **0.1689 ns** | **18.5483 ns** |  **1.00** |    **0.01** | **0.0095** |      **80 B** |        **1.00** |
| TypeSafeIdNotGenericToString | 0            | 18.2821 ns | 0.1141 ns | 0.1012 ns | 18.3015 ns |  0.99 |    0.01 | 0.0095 |      80 B |        1.00 |
| FastIdsDecodedToString       | 0            | 17.0064 ns | 0.1510 ns | 0.1412 ns | 16.9530 ns |  0.92 |    0.01 | 0.0095 |      80 B |        1.00 |
| FastIdsEncodedToString       | 0            |  0.2152 ns | 0.0037 ns | 0.0029 ns |  0.2147 ns |  0.01 |    0.00 |      - |         - |        0.00 |
| TcKsToString                 | 0            |  1.2691 ns | 0.1198 ns | 0.3494 ns |  1.3199 ns |  0.07 |    0.02 |      - |         - |        0.00 |
| CbuctokToString              | 0            | 24.7056 ns | 0.2106 ns | 0.1867 ns | 24.7325 ns |  1.33 |    0.02 | 0.0038 |      32 B |        0.40 |
|                      |              |            |           |           |            |       |         |        |           |             |
| **TypeSafeIdGenericToString**    | **5**            | **19.5156 ns** | **0.0837 ns** | **0.1612 ns** | **19.5103 ns** |  **1.00** |    **0.01** | **0.0105** |      **88 B** |        **1.00** |
| TypeSafeIdNotGenericToString | 5            | 20.2737 ns | 0.0807 ns | 0.0715 ns | 20.2533 ns |  1.04 |    0.01 | 0.0105 |      88 B |        1.00 |
| FastIdsDecodedToString       | 5            | 27.7151 ns | 0.0913 ns | 0.0809 ns | 27.6862 ns |  1.42 |    0.01 | 0.0105 |      88 B |        1.00 |
| FastIdsEncodedToString       | 5            |  0.2228 ns | 0.0240 ns | 0.0225 ns |  0.2092 ns |  0.01 |    0.00 |      - |         - |        0.00 |
| TcKsToString                 | 5            |  0.5515 ns | 0.1838 ns | 0.5419 ns |  0.1982 ns |  0.03 |    0.03 |      - |         - |        0.00 |
| CbuctokToString              | 5            | 23.3241 ns | 0.3092 ns | 0.2741 ns | 23.3324 ns |  1.20 |    0.02 | 0.0143 |     120 B |        1.36 |
|                      |              |            |           |           |            |       |         |        |           |             |
| **TypeSafeIdGenericToString**    | **10**           | **20.1190 ns** | **0.1206 ns** | **0.1128 ns** | **20.0985 ns** |  **1.00** |    **0.01** | **0.0115** |      **96 B** |        **1.00** |
| TypeSafeIdNotGenericToString | 10           | 20.5725 ns | 0.0421 ns | 0.0329 ns | 20.5734 ns |  1.02 |    0.01 | 0.0115 |      96 B |        1.00 |
| FastIdsDecodedToString       | 10           | 27.5041 ns | 0.2376 ns | 0.2222 ns | 27.4032 ns |  1.37 |    0.01 | 0.0115 |      96 B |        1.00 |
| FastIdsEncodedToString       | 10           |  0.2227 ns | 0.0101 ns | 0.0084 ns |  0.2191 ns |  0.01 |    0.00 |      - |         - |        0.00 |
| TcKsToString                 | 10           |  0.2320 ns | 0.0156 ns | 0.0138 ns |  0.2299 ns |  0.01 |    0.00 |      - |         - |        0.00 |
| CbuctokToString              | 10           | 23.7421 ns | 0.1030 ns | 0.0804 ns | 23.7690 ns |  1.18 |    0.01 | 0.0153 |     128 B |        1.33 |
|                      |              |            |           |           |            |       |         |        |           |             |
| **TypeSafeIdGenericToString**    | **30**           | **22.1418 ns** | **0.1878 ns** | **0.1664 ns** | **22.1373 ns** | **1.000** |    **0.01** | **0.0162** |     **136 B** |        **1.00** |
| TypeSafeIdNotGenericToString | 30           | 22.3060 ns | 0.1667 ns | 0.1559 ns | 22.3070 ns | 1.007 |    0.01 | 0.0162 |     136 B |        1.00 |
| FastIdsDecodedToString       | 30           | 29.0123 ns | 0.1525 ns | 0.1352 ns | 29.0288 ns | 1.310 |    0.01 | 0.0162 |     136 B |        1.00 |
| FastIdsEncodedToString       | 30           |  0.2179 ns | 0.0082 ns | 0.0077 ns |  0.2162 ns | 0.010 |    0.00 |      - |         - |        0.00 |
| TcKsToString                 | 30           |  0.2197 ns | 0.0070 ns | 0.0062 ns |  0.2169 ns | 0.010 |    0.00 |      - |         - |        0.00 |
| CbuctokToString              | 30           | 28.2824 ns | 0.2869 ns | 0.2684 ns | 28.3756 ns | 1.277 |    0.01 | 0.0201 |     168 B |        1.24 |
|                      |              |            |           |           |            |       |         |        |           |             |
| **TypeSafeIdGenericToString**    | **63**           | **24.0398 ns** | **0.0889 ns** | **0.0788 ns** | **24.0340 ns** | **1.000** |    **0.00** | **0.0249** |     **208 B** |        **1.00** |
| TypeSafeIdNotGenericToString | 63           | 24.0871 ns | 0.1280 ns | 0.1197 ns | 24.0197 ns | 1.002 |    0.01 | 0.0249 |     208 B |        1.00 |
| FastIdsDecodedToString       | 63           | 30.5317 ns | 0.2732 ns | 0.2555 ns | 30.4904 ns | 1.270 |    0.01 | 0.0249 |     208 B |        1.00 |
| FastIdsEncodedToString       | 63           |  0.2163 ns | 0.0074 ns | 0.0066 ns |  0.2138 ns | 0.009 |    0.00 |      - |         - |        0.00 |
| TcKsToString                 | 63           |  0.2248 ns | 0.0060 ns | 0.0056 ns |  0.2252 ns | 0.009 |    0.00 |      - |         - |        0.00 |
| CbuctokToString              | 63           | 35.9769 ns | 0.7500 ns | 0.5855 ns | 35.9572 ns | 1.497 |    0.02 | 0.0287 |     240 B |        1.15 |
