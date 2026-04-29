using FluentValidation;
namespace Mostruario.Application.Validators;

public class CreateProductValidator : AbstractValidator<DTOs.CreateProductDto>
{
    public CreateProductValidator()
    {
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