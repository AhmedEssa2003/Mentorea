namespace Mentorea.Contracts.Card
{
    public class CardRequestValidator:AbstractValidator<CardRequest>
    {
        public CardRequestValidator()
        {
            RuleFor(x=>x.CardId)
                .NotEmpty()
                .Length(16);
        }
    }
}
