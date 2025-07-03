namespace Mentorea.Errors
{
    public static class CardError
    {
        public static Error AlreadyExist = new("Card.AlreadyExist", "you Already Have Card Id Can update it ", StatusCodes.Status400BadRequest);
        public static Error NotFoundCardId = new("card.NotFoundCardId", "you dont have Card Id", StatusCodes.Status404NotFound);
    }
}
