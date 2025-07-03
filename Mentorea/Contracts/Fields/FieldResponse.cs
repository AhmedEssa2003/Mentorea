namespace Mentorea.Contracts.Fields
{
    public record FieldResponse(
        string SpecializationName,
        IEnumerable<FieldSheep> Fields
    );
}
