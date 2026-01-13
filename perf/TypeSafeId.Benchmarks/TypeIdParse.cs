using System.Text;
using BenchmarkDotNet.Attributes;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
[MarkdownExporterAttribute.GitHub]
public class TypeIdParse
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private string _typeIdString = "";

    private readonly string _prefixFull;
    private readonly Guid _uuidV7;

    public TypeIdParse()
    {
        var random = new Random(42);
        var sb = new StringBuilder(63);
        for (var i = 0; i < 63; i++)
        {
            var letter = (char)random.Next('a', 'z');
            sb.Append(letter);
        }
        _prefixFull = sb.ToString();
        _uuidV7 = new Guid("01890a5d-ac96-774b-bcce-b302099a8057");
    }

    [GlobalSetup]
    public void Setup()
    {
        var prefix = _prefixFull[..PrefixLength];

#pragma warning disable CS0618 // Type or member is obsolete
        TypeId<Entity>.SetPrefix(prefix);
#pragma warning restore CS0618 // Type or member is obsolete

        _typeIdString = new TypeId(prefix, _uuidV7).ToString();
    }

    [Benchmark(Baseline = true)]
    public TypeId<Entity> TypeSafeIdGenericParse()
    {
        return TypeId<Entity>.Parse(_typeIdString);
    }

    [Benchmark]
    public TypeId<Entity> TypeSafeIdGenericTryParse()
    {
        TypeId<Entity>.TryParse(_typeIdString, out var typeId);
        return typeId;
    }

    [Benchmark]
    public TypeId TypeSafeIdNotGenericParse()
    {
        return TypeId.Parse(_typeIdString);
    }

    [Benchmark]
    public TypeId TypeSafeIdNotGenericTryParse()
    {
        TypeId.TryParse(_typeIdString, out var typeId);
        return typeId;
    }

    [Benchmark]
    public FastIDs.TypeId.TypeId FastIdsParse()
    {
        return FastIDs.TypeId.TypeId.Parse(_typeIdString);
    }

    [Benchmark]
    public FastIDs.TypeId.TypeId FastIdsTryParse()
    {
        FastIDs.TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId;
    }

    [Benchmark]
    public TcKs.TypeId.TypeId TcKsParse()
    {
        return TcKs.TypeId.TypeId.Parse(_typeIdString);
    }

    [Benchmark]
    public TcKs.TypeId.TypeId TcKsTryParse()
    {
        TcKs.TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId;
    }

    [Benchmark]
    public global::TypeId.TypeId CbuctokParse()
    {
        return global::TypeId.TypeId.Parse(_typeIdString);
    }

    [Benchmark]
    public global::TypeId.TypeId CbuctokTryParse()
    {
        global::TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId;
    }

    public record Entity();
}
