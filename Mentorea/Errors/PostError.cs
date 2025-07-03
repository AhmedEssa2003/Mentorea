namespace Mentorea.Errors
{
    public static class PostError
    {
        public static readonly Error NotFound = new("Post.NotFound", "No Exist Post with this id", StatusCodes.Status404NotFound);
        public static readonly Error Ctreate = new("Post.CreateError", "Error while creating post", StatusCodes.Status500InternalServerError);
        public static readonly Error Unauthorized = new("Post.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
    }
}
