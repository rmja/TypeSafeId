using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class TypeIdComparison
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private readonly string _suffix = "01h455vb4pex5vsknk084sn02q";

    private TypeId<Entity>[] _typeSafeIdGenericTypeIds = [];
    private TypeId[] _typeSafeIdNotGenericTypeIds = [];
    private FastIDs.TypeId.TypeId[] _fastIdTypeIds = [];
    private FastIDs.TypeId.TypeIdDecoded[] _fastIdTypeIdsDecoded = [];
    private TcKs.TypeId.TypeId[] _tcKsTypeIds = [];
    private global::TypeId.TypeId[] _cbuctokTypeIds = [];

    private readonly string _prefixFull;

    public TypeIdComparison()
    {
        var random = new Random(42);
        var sb = new StringBuilder(63);
        for (var i = 0; i < 63; i++)
        {
            var letter = (char)random.Next('a', 'z');
            sb.Append(letter);
        }
        _prefixFull = sb.ToString();
    }

    [GlobalSetup]
    public void Setup()
    {
        var prefix = PrefixLength > 0 ? _prefixFull[..PrefixLength] : string.Empty;
        var typeIdStr = PrefixLength > 0 ? $"{prefix}_{_suffix}" : _suffix;

#pragma warning disable CS0618 // Type or member is obsolete
        TypeId<Entity>.SetPrefix(prefix);
#pragma warning restore CS0618 // Type or member is obsolete

        _typeSafeIdGenericTypeIds =
        [
            TypeSafeId.TypeId<Entity>.Parse(typeIdStr),
            TypeSafeId.TypeId<Entity>.Parse(typeIdStr),
        ];

        _typeSafeIdNotGenericTypeIds =
        [
            TypeSafeId.TypeId.Parse(typeIdStr),
            TypeSafeId.TypeId.Parse(typeIdStr),
        ];

        _fastIdTypeIds =
        [
            FastIDs.TypeId.TypeId.Parse(typeIdStr),
            FastIDs.TypeId.TypeId.Parse(typeIdStr),
        ];
        _fastIdTypeIdsDecoded =
        [
            FastIDs.TypeId.TypeId.Parse(typeIdStr).Decode(),
            FastIDs.TypeId.TypeId.Parse(typeIdStr).Decode(),
        ];

        _tcKsTypeIds = [];
        if (TcKs.TypeId.TypeId.TryParse(typeIdStr, out var parsed))
        {
            _tcKsTypeIds = [parsed, parsed];
        }

        _cbuctokTypeIds =
        [
            global::TypeId.TypeId.Parse(typeIdStr),
            global::TypeId.TypeId.Parse(typeIdStr),
        ];
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Equality")]
    public bool TypeSafeIdGenericEquals() =>
        _typeSafeIdGenericTypeIds[0] == _typeSafeIdGenericTypeIds[1];

    [Benchmark]
    [BenchmarkCategory("Equality")]
    public bool TypeSafeIdNotGenericEquals() =>
        _typeSafeIdNotGenericTypeIds[0] == _typeSafeIdNotGenericTypeIds[1];

    [Benchmark]
    [BenchmarkCategory("Equality")]
    public bool FastIdsEquals() => _fastIdTypeIds[0] == _fastIdTypeIds[1];

    [Benchmark]
    [BenchmarkCategory("Equality")]
    public bool FastIdsDecodedEquals() => _fastIdTypeIdsDecoded[0] == _fastIdTypeIdsDecoded[1];

    [Benchmark]
    [BenchmarkCategory("Equality")]
    public bool TcKsEquals() => _tcKsTypeIds[0] == _tcKsTypeIds[1];

    [Benchmark]
    [BenchmarkCategory("Equality")]
    public bool CbuctokEquals() => _cbuctokTypeIds[0] == _cbuctokTypeIds[1];

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("HashCode")]
    public int TypeSafeIdGenericHash() => _typeSafeIdGenericTypeIds[0].GetHashCode();

    [Benchmark]
    [BenchmarkCategory("HashCode")]
    public int TypeSafeIdNotGenericHash() => _typeSafeIdNotGenericTypeIds[0].GetHashCode();

    [Benchmark]
    [BenchmarkCategory("HashCode")]
    public int FastIdsHash() => _fastIdTypeIds[0].GetHashCode();

    [Benchmark]
    [BenchmarkCategory("HashCode")]
    public int FastIdsDecodedHash() => _fastIdTypeIdsDecoded[0].GetHashCode();

    [Benchmark]
    [BenchmarkCategory("HashCode")]
    public int TcKsHash() => _tcKsTypeIds[0].GetHashCode();

    [Benchmark]
    [BenchmarkCategory("HashCode")]
    public int CbuctokHash() => _cbuctokTypeIds[0].GetHashCode();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Prefix")]
    public string TypeSafeIdGenericPrefix() => TypeId<Entity>.Prefix;

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public ReadOnlySpan<char> TypeSafeIdNotGenericPrefix() =>
        _typeSafeIdNotGenericTypeIds[0].Prefix;

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public string FastIdsPrefixString() => _fastIdTypeIds[0].Type.ToString();

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public ReadOnlySpan<char> FastIdsPrefixSpan() => _fastIdTypeIds[0].Type;

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public string FastIdsDecodedPrefix() => _fastIdTypeIdsDecoded[0].Type;

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public string TcKsPrefix() => _tcKsTypeIds[0].Type;

    [Benchmark]
    [BenchmarkCategory("Prefix")]
    public string CbuctokPrefix() => _cbuctokTypeIds[0].Type;

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Id")]
    public Guid TypeSafeIdGenericId() => _typeSafeIdGenericTypeIds[0].Uuid;

    [Benchmark]
    [BenchmarkCategory("Id")]
    public Guid TypeSafeIdNotGenericId() => _typeSafeIdNotGenericTypeIds[0].Uuid;

    [Benchmark]
    [BenchmarkCategory("Id")]
    public Guid FastIdsId() => _fastIdTypeIds[0].Decode().Id;

    [Benchmark]
    [BenchmarkCategory("Id")]
    public Guid FastIdsDecodedId() => _fastIdTypeIdsDecoded[0].Id;

    [Benchmark]
    [BenchmarkCategory("Id")]
    public Guid TcKsId() => _tcKsTypeIds[0].Id;

    [Benchmark]
    [BenchmarkCategory("Id")]
    public string CbuctokId() => _cbuctokTypeIds[0].Id;

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Suffix")]
    public string TypeSafeIdGenericSuffix() => _typeSafeIdGenericTypeIds[0].GetSuffix();

    [BenchmarkCategory("Suffix")]
    public int TypeSafeIdGenericSuffixSpan()
    {
        Span<char> buffer = stackalloc char[26];
        return _typeSafeIdGenericTypeIds[0].GetSuffix(buffer);
    }

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public string TypeSafeIdNotGenericSuffix() => _typeSafeIdNotGenericTypeIds[0].GetSuffix();

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public int TypeSafeIdNotGenericSuffixSpan()
    {
        Span<char> buffer = stackalloc char[26];
        return _typeSafeIdNotGenericTypeIds[0].GetSuffix(buffer);
    }

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public string FastIdsSuffixString() => _fastIdTypeIds[0].Suffix.ToString();

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public ReadOnlySpan<char> FastIdsSuffixSpan() => _fastIdTypeIds[0].Suffix;

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public string FastIdsDecodedSuffix() => _fastIdTypeIdsDecoded[0].GetSuffix();

    [Benchmark]
    [BenchmarkCategory("Suffix")]
    public string TcKsSuffix() => _tcKsTypeIds[0].Suffix;

    public record Entity();
}
