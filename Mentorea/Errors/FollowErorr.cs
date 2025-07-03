using Mentorea.Abstractions;

namespace Mentorea.Errors
{
    public static class FollowErorr
    {
        public static readonly Error InvalidOperation = new Error("Follow.InvalidOperation","the same person ",StatusCodes.Status400BadRequest);
        public static readonly Error NotFoundFollowed = new Error("Follow.NotFoundFollowed", "No Exist Followed with this id", StatusCodes.Status404NotFound);
    }
}
