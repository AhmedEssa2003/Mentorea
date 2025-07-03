namespace Mentorea.Errors
{
    public static class LikeError
    {
        public static readonly Error InvalidOperation = new Error("Like.InvalidOperation", "You can't like your own post", StatusCodes.Status400BadRequest);
        
    }
}
