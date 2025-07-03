namespace Mentorea.Errors
{
    public static class FieldError
    {
        public static readonly Error NotFound = new ("Field.NotFound", "No Exist Field with this id", StatusCodes.Status404NotFound);
        public static readonly Error Duplicate = new ("Field.Duplicate", "Already Exist Field with this Name", StatusCodes.Status409Conflict);
        public static readonly Error HasDependentField = new("Field.HasDependentField", "Cannot delete Field because it has related fields", StatusCodes.Status400BadRequest);
    }
}
