namespace Mentorea.Errors
{
    public static class UserError
    {
        public static readonly Error InvalidCredential = new("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

        public static readonly Error DisaledUser = new("User.DisaledUser", "Disaled user,please contact your administrator", StatusCodes.Status401Unauthorized);

        public static readonly Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email is Not Confirmed", StatusCodes.Status401Unauthorized);

        public static readonly Error LockedUser = new("User.LockedUser", "Locked user,please contact your administrator", StatusCodes.Status401Unauthorized);

        public static readonly Error NotFoundUser = new("User.NotFound", "No Exist User with this id", StatusCodes.Status404NotFound);

        public static readonly Error InvalidRole = new("User.Role ", "Invalid role ", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidToken = new("User.InvalidToken", "Invalid Token", StatusCodes.Status401Unauthorized);
        public static readonly Error DuplicateEmail = new("User.DuplicateEmail", "Email is Already Exists", StatusCodes.Status409Conflict);
        public static readonly Error DuplicateUnLock = new("User.DuplicateUnLock", "lock is Already false", StatusCodes.Status409Conflict);
       
        public static readonly Error InvalidCode = new("User.InvalidCode", "Invalid Code", StatusCodes.Status409Conflict);
        public static readonly Error DuplicatedConfirmation = new("User.DuplicatedConfirmation", "Email Already Confirmed", StatusCodes.Status400BadRequest);
    }
}
