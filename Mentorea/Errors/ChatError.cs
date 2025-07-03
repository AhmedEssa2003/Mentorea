
namespace Mentorea.Errors
{
    public static class ChatError
    {
        public static Error UploadError =>new Error("Chat.UploadError", "File upload error try again",StatusCodes.Status400BadRequest );
        public static Error InvalidFileType => new Error("Chat.InvalidFileType", "Invalid file type", StatusCodes.Status400BadRequest);
        public static Error FileNotFound => new Error("Chat.FileNotFound", "File not found", StatusCodes.Status404NotFound);
        public static Error MessageNotFound => new Error("Chat.MessageNotFound", "Message not found", StatusCodes.Status404NotFound);
        public static Error NotAuthorized => new Error("Chat.NotAuthorized", "You are not authorized to delete this message", StatusCodes.Status403Forbidden);
        public static Error UserNotFound => new Error("Chat.UserNotFound", "User not found", StatusCodes.Status404NotFound);
    }
}
