namespace Paps.Persistence.SourceGeneration;

public readonly record struct DataStorageTypeRegistrationData
{
    public readonly string FullTypeMetadataName;
    public readonly string TypeDiscriminator;
    public readonly int InitialPoolCapacity;

    public DataStorageTypeRegistrationData(string fullTypeMetadataName, string typeDiscriminator, int initialPoolCapacity)
    {
        FullTypeMetadataName = fullTypeMetadataName;
        TypeDiscriminator = typeDiscriminator;
        InitialPoolCapacity = initialPoolCapacity;
    }
}
