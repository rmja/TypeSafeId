using System.Text;
using BenchmarkDotNet.Attributes;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
[MarkdownExporterAttribute.GitHub]
public class TypeIdParseToUuid
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private string _typeIdString = "";

    private readonly string _prefixFull;
    private readonly Guid _uuidV7;

    public TypeIdParseToUuid()
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
    public Guid TypeSafeIdGenericParseToUuid()
    {
        return TypeId<Entity>.Parse(_typeIdString).Uuid;
    }

    [Benchmark]
    public Guid TypeSafeIdGenericTryParseToUuid()
    {
        TypeId<Entity>.TryParse(_typeIdString, out var typeId);
        return typeId.Uuid;
    }

    [Benchmark]
    public Guid TypeSafeIdNotGenericParseToUuid()
    {
        return TypeId.Parse(_typeIdString).Uuid;
    }

    [Benchmark]
    public Guid TypeSafeIdNotGenericTryParseToUuid()
    {
        TypeId.TryParse(_typeIdString, out var typeId);
        return typeId.Uuid;
    }

    [Benchmark]
    public Guid FastIdsParseToUuid()
    {
        return FastIDs.TypeId.TypeId.Parse(_typeIdString).Decode().Id;
    }

    [Benchmark]
    public Guid FastIdsTryParseToUuid()
    {
        FastIDs.TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId.Decode().Id;
    }

    [Benchmark]
    public Guid TcKsParseToUuid()
    {
        return TcKs.TypeId.TypeId.Parse(_typeIdString).Id;
    }

    [Benchmark]
    public Guid TcKsTryParseToUuid()
    {
        TcKs.TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId.Id;
    }

    [Benchmark]
    public Guid CbuctokParseToUuid()
    {
        return global::TypeId.TypeId.Parse(_typeIdString).GetUuid();
    }

    [Benchmark]
    public Guid CbuctokTryParseToUuid()
    {
        global::TypeId.TypeId.TryParse(_typeIdString, out var typeId);
        return typeId.GetUuid();
    }

    public record Entity();
}
