using FluentValidation;
using UpdateCategoryDto = Mostruario.Application.DTOs.UpdateCategoryDto;

namespace Mostruario.Application.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
    }
}