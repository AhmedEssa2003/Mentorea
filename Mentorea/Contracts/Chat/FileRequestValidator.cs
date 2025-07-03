namespace Mentorea.Contracts.Chat
{
    public class FileRequestValidator : AbstractValidator<FileRequest>
    {
        public readonly string[]  _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".wav", ".ogg" };

    public FileRequestValidator()
        {

            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("File is required.")
                .Must(file => file.Length > 0)
                .Must(file => _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("File extension is not allowed.");

        }
    }
    
}
