namespace Mentorea.Contracts.Chat
{
    public class DeleteRequestValidator:AbstractValidator<DeleteRequest>
    {
        public DeleteRequestValidator()
        {
            RuleFor(x => x.MessagedId)
                .NotEmpty();
                
        }
    }
}
