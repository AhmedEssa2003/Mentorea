namespace Mentorea.Errors
{
    public static class ResultError
    {
        public static readonly Error FailedToGetMentors = new("User.FailedToGetMentors", "Unable to retrieve mentors for the user due to an unexpected error.", StatusCodes.Status409Conflict);
    }
}
