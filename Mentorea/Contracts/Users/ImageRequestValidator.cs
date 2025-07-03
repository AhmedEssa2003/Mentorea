namespace Mentorea.Contracts.Users
{
    public class ImageRequestValidator:AbstractValidator<ImageRequest>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public ImageRequestValidator()
        {
            RuleFor(x => x.Image)
                .Must(x => _allowedExtensions.Contains(Path.GetExtension(x!.FileName)))
                .WithMessage("File extension is allowed { \".jpg\", \".jpeg\", \".png\", \".webp\" } only ")
                .Must(x => x!.Length < 1 * 1024 * 1024)
                .WithMessage("File size should be less than 1MB")
                .When(x => x.Image != null);
        }
    }
}
