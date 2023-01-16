using DemoApplication.Areas.Admin.ViewModels.BookImage;
using DemoApplication.Contracts;
using DemoApplication.Contracts.FileSizes;
using DemoApplication.Validators;
using FluentValidation;

namespace DemoApplication.Areas.Admin.Validators.BookImage
{
    public class AddViewModelValidator : AbstractValidator<AddViewModel>
    {
        public AddViewModelValidator()
        {
            RuleFor(avm => avm.Image)
               .Cascade(CascadeMode.Stop)

               .NotNull()
               .WithMessage("Image can't be empty")

               .SetValidator(
                    new FileValidator(2, FileSizes.Kilo,
                        FileExtensions.JPG.GetExtension(), FileExtensions.PNG.GetExtension())!);
        }
    }
}
