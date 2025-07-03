namespace SurveyBasket.Errors
{
    public static class RoleError
    {
        public static readonly Error RoleNotFound = new("Role.NotFound", "No Role Was Found With Given Id", StatusCodes.Status404NotFound);
        public static readonly Error InvalidPermission = new("Role.Permission ", "Invalid Permission ", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicateRole = new("Role.DuplicateRole", "Role is Already Exists", StatusCodes.Status409Conflict);

    }
}
