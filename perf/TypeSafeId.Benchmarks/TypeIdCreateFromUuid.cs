using System.Text;
using BenchmarkDotNet.Attributes;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
[MarkdownExporterAttribute.GitHub]
public class TypeIdCreateFromUuid
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private string _prefix = "";
    private readonly string _prefixFull;
    private readonly Guid _uuidV7;

    public TypeIdCreateFromUuid()
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
        _prefix = _prefixFull[..PrefixLength];

#pragma warning disable CS0618 // Type or member is obsolete
        TypeId<Entity>.SetPrefix(_prefix);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Benchmark(Baseline = true)]
    public TypeId<Entity> TypeSafeIdGenericFromUuid()
    {
        return new TypeId<Entity>(_uuidV7);
    }

    [Benchmark]
    public TypeId TypeSafeIdNotGenericFromUuid()
    {
        return new TypeId(_prefix, _uuidV7);
    }

    [Benchmark]
    public FastIDs.TypeId.TypeIdDecoded FastIdsFromUuid()
    {
        return FastIDs.TypeId.TypeId.FromUuidV7(_prefix, _uuidV7);
    }

    [Benchmark]
    public TcKs.TypeId.TypeId TcKsFromUuid()
    {
        return new TcKs.TypeId.TypeId(_prefix, _uuidV7);
    }

    [Benchmark]
    public global::TypeId.TypeId CbuctokFromUuid()
    {
        return new global::TypeId.TypeId(_prefix, _uuidV7);
    }

    public record Entity();
}
