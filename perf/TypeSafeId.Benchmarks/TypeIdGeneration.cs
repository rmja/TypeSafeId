using System.Text;
using BenchmarkDotNet.Attributes;

namespace TypeSafeId.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
[MarkdownExporterAttribute.Default]
public class TypeIdGeneration
{
    [Params(0, 5, 10, 30, 63)]
    public int PrefixLength;

    private string _prefix = "";
    private readonly string _prefixFull;

    public TypeIdGeneration()
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
        _prefix = _prefixFull[..PrefixLength];

#pragma warning disable CS0618 // Type or member is obsolete
        TypeId<Entity>.SetPrefix(_prefix);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Benchmark(Baseline = true)]
    public TypeSafeId.TypeId<Entity> TypeSafeIdGenericGenerate()
    {
        return TypeSafeId.TypeId<Entity>.Create();
    }

    [Benchmark]
    public TypeSafeId.TypeId TypeSafeIdNotGenericGenerate()
    {
        return TypeSafeId.TypeId.Create(_prefix);
    }

    [Benchmark]
    public FastIDs.TypeId.TypeIdDecoded FastIdsGenerate()
    {
        return FastIDs.TypeId.TypeId.New(_prefix);
    }

    [Benchmark]
    public FastIDs.TypeId.TypeIdDecoded FastIdsNoCheckGenerate()
    {
        return FastIDs.TypeId.TypeId.New(_prefix, false);
    }

    [Benchmark]
    public TcKs.TypeId.TypeId TcKsGenerate()
    {
        return TcKs.TypeId.TypeId.NewId(_prefix);
    }

    [Benchmark]
    public global::TypeId.TypeId CbuctokGenerate()
    {
        return global::TypeId.TypeId.NewTypeId(_prefix);
    }

    public record Entity();
}
