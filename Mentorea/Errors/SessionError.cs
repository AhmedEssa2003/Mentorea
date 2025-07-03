namespace Mentorea.Errors
{
    public static class SessionError
    {
        public static Error NotFoundSession => new Error( "Session.NotFound", "Session not found", StatusCodes.Status404NotFound);
        public static Error ActionNotAllawed => new Error("Session.ActionNotAllowed", "This action is not allowed in the current state", StatusCodes.Status409Conflict);
        public static Error TimeError => new Error("Session.TimeError", "This action is not allowed in the current Time.", StatusCodes.Status400BadRequest);
        public static Error TimeTaken => new Error("Session.TimeTaken", "This time slot is already taken by another session. Please choose another time.", StatusCodes.Status400BadRequest);
        public static Error UserNotMentor => new Error("Session.UserNotMentor", "User is not a mentor", StatusCodes.Status400BadRequest);
        public static Error PaymentError => new Error("Session.PaymentError", "Not yet determined", StatusCodes.Status400BadRequest);
        public static Error Unauthorized => new Error("Session.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status401Unauthorized);
        public static Error FeedbackRequired => new Error(
                         "Session.FeedbackRequired",
                         "You need to provide feedback for your previous session before proceeding.",
                         StatusCodes.Status400BadRequest);
        public static Error InvalidScheduledTime => new Error("Session.InvalidScheduledTime", "The scheduled time is invalid. Please check the date and time format.", StatusCodes.Status400BadRequest);




    }
}
