namespace Mentorea.Contracts.Common
{
    public class RequestFiltersValidator:AbstractValidator<RequestFilters>
    {
        
        public RequestFiltersValidator()
        {
            var allowedSortDirections = new[] { "ASC", "DESC" };
            RuleFor(x => x.PageNumber)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(50)
                .WithMessage("Page size must be greater than 0 and less than 50");
            RuleFor(x=>x.SortDirection)
                .Must(x => allowedSortDirections.Contains(x))
                .WithMessage($"Sort direction must be one of the following: {string.Join(", ", allowedSortDirections)}");
        }
    }
   
}
