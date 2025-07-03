namespace Mentorea.Contracts.Fields
{
    public class FieldRequestValidator:AbstractValidator<FieldRequest>
    {
        public FieldRequestValidator()
        {
            RuleFor(x => x.SpcializationId)
                .NotEmpty()
                .Length(36);
            RuleFor(x => x.FieldName)
                .NotEmpty()
                .Length(3, 60);
        }
    }
}
