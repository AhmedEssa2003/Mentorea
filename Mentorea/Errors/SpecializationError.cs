namespace Mentorea.Errors
{
    public static class SpecializationError
    {
        public static Error NotFound =>
            new("Specialization_not_found", "Specialization not found", StatusCodes.Status404NotFound);

        public static Error NameAlreadyExists =>
            new("Specialization_name_exists", "A Specialization with the same name already exists", StatusCodes.Status409Conflict);

        public static Error HasDependentSpecializations =>
            new("Specialization_has_specializations", "Cannot delete Specialization because it has related specializations", StatusCodes.Status400BadRequest);

        //public static Error Unauthorized =>
        //    new("Specialization_unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
    }
}
