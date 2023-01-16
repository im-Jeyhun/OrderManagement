using DemoApplication.Contracts.FileSizes;
using FluentValidation;

namespace DemoApplication.Validators
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator() { }

        public FileValidator(long size, FileSizes fileSize, params string[] extensions)
        {
            RuleFor(i => i.Length)
                .LessThanOrEqualTo(size * fileSize.GetSize())
                .WithMessage($"Image size should be less that : {size} {fileSize.GetShortNameWithByte()}");

            RuleFor(i => i.FileName)
                .Must(fn => extensions.Contains(Path.GetExtension(fn)))
                .WithMessage($"Image extension shold be {string.Join(", ", extensions)}");
        }
    }
}
