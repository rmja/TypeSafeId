using System.Text;
using BenchmarkDotNet.Attributes;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
public class TypeIdToString
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private readonly string _prefixFull;
    private readonly Guid _uuidV7;

    private TypeId<Entity> _typeSafeIdGenericTypeId;
    private TypeId _typeSafeIdNotGenericTypeId;
    private FastIDs.TypeId.TypeId _fastIdTypeId;
    private FastIDs.TypeId.TypeIdDecoded _fastIdTypeIdDecoded;
    private TcKs.TypeId.TypeId _tcKsTypeId;
    private global::TypeId.TypeId _cbuctokTypeId;

    public TypeIdToString()
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

        _typeSafeIdGenericTypeId = new TypeId<Entity>(_uuidV7);
        _typeSafeIdNotGenericTypeId = new TypeId(prefix, _uuidV7);
        _fastIdTypeIdDecoded = FastIDs.TypeId.TypeId.FromUuidV7(prefix, _uuidV7);
        _fastIdTypeId = _fastIdTypeIdDecoded.Encode();
        _tcKsTypeId = new TcKs.TypeId.TypeId(prefix, _uuidV7);
        _cbuctokTypeId = new global::TypeId.TypeId(prefix, _uuidV7);
    }

    [Benchmark(Baseline = true)]
    public string TypeSafeIdGeneric()
    {
        return _typeSafeIdGenericTypeId.ToString();
    }

    [Benchmark]
    public string TypeSafeIdNotGeneric()
    {
        return _typeSafeIdNotGenericTypeId.ToString();
    }

    [Benchmark]
    public string FastIdsDecoded()
    {
        return _fastIdTypeIdDecoded.ToString();
    }

    [Benchmark]
    public string FastIdsEncoded()
    {
        return _fastIdTypeId.ToString();
    }

    [Benchmark]
    public string TcKs()
    {
        return _tcKsTypeId.ToString();
    }

    [Benchmark]
    public string Cbuctok()
    {
        return _cbuctokTypeId.ToString();
    }

    [TypeId("prefix")]
    public record Entity();
}
