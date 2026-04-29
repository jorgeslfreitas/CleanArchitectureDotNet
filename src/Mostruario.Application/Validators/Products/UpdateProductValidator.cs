using FluentValidation;
using UpdateProductDto = Mostruario.Application.DTOs.UpdateProductDto;

namespace Mostruario.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Categoria é obrigatória");
    }
}