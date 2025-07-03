namespace Mentorea.Errors
{
    public static class CommentError
    {
        public static Error NotFound => new ("comment_not_found", "Comment not found",StatusCodes.Status404NotFound);
        public static Error Unauthorized => new("comment_unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
    }
}
