namespace TypeSafeId.Tests;

public class TypeIdGenericConstraintTests
{
    [TypeId("entity")]
    class EntityClass
    {
        public TypeId<EntityClass> Id { get; set; }
    }

    [TypeId("entity")]
    struct EntityStruct
    {
        public TypeId<EntityStruct> Id { get; set; }
    }

    [TypeId("record")]
    record EntityRecord(TypeId<EntityRecord> Id);

    record struct EntityRecordStruct(TypeId<EntityRecordStruct> Id);

    [Fact]
    public void TypeId_WorksWithClass()
    {
        // Arrange & Act
        var typeId = TypeId<EntityClass>.Create();

        // Assert
        Assert.StartsWith("entity_", typeId.ToString());
        Assert.Equal("entity", TypeId<EntityClass>.Prefix);
    }

    [Fact]
    public void TypeId_WorksWithStruct()
    {
        // Arrange & Act
        var typeId = TypeId<EntityStruct>.Create();

        // Assert
        Assert.StartsWith("entity_", typeId.ToString());
        Assert.Equal("entity", TypeId<EntityStruct>.Prefix);
    }

    [Fact]
    public void TypeId_WorksWithRecord()
    {
        // Arrange & Act
        var typeId = TypeId<EntityRecord>.Create();

        // Assert
        Assert.StartsWith("record_", typeId.ToString());
        Assert.Equal("record", TypeId<EntityRecord>.Prefix);
    }

    [Fact]
    public void TypeId_WorksWithRecordStruct_UsesDefaultPrefix()
    {
        // Arrange & Act
        var typeId = TypeId<EntityRecordStruct>.Create();
        var prefix = TypeId<EntityRecordStruct>.Prefix;

        // Assert
        Assert.StartsWith("entity_record_struct_", typeId.ToString());
        Assert.Equal("entity_record_struct", prefix);
    }

    [Fact]
    public void TypeId_DifferentEntityTypes_HaveDifferentPrefixes()
    {
        // Arrange & Act
        var classPrefix = TypeId<EntityClass>.Prefix;
        var recordPrefix = TypeId<EntityRecord>.Prefix;

        // Assert
        Assert.NotEqual(classPrefix, recordPrefix);
    }
}
