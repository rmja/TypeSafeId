namespace TypeSafeId.Tests;

public class TypeIdConcurrencyTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [Fact]
    public void Create_CalledConcurrently_GeneratesUniqueIds()
    {
        // Arrange
        const int threadCount = 10;
        const int idsPerThread = 100;
        var allIds = new System.Collections.Concurrent.ConcurrentBag<TypeId<User>>();

        // Act
        Parallel.For(
            0,
            threadCount,
            _ =>
            {
                for (int i = 0; i < idsPerThread; i++)
                {
                    allIds.Add(TypeId<User>.Create());
                }
            }
        );

        // Assert
        var distinctIds = allIds.Distinct().ToList();
        Assert.Equal(threadCount * idsPerThread, distinctIds.Count);
    }

    [Fact]
    public void Prefix_AccessedConcurrently_ReturnsSameInstance()
    {
        // Arrange
        const int threadCount = 10;
        var prefixes = new System.Collections.Concurrent.ConcurrentBag<string>();

        // Act
        Parallel.For(
            0,
            threadCount,
            _ =>
            {
                prefixes.Add(TypeId<User>.Prefix);
            }
        );

        // Assert
        Assert.All(prefixes, p => Assert.Equal("user", p));
        // All should reference the same string instance
        var distinctReferences = prefixes.Distinct(ReferenceEqualityComparer.Instance).ToList();
        Assert.Single(distinctReferences);
    }

    private class ReferenceEqualityComparer : IEqualityComparer<string>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public bool Equals(string? x, string? y) => ReferenceEquals(x, y);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
