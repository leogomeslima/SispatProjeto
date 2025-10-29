using FluentValidation;
using Sispat.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.Validators
{
    public class CreateAssetValidator : AbstractValidator<CreateAssetDto>
    {
        public CreateAssetValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(200);

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("O número de série é obrigatório.");

            RuleFor(x => x.PurchaseValue)
                .GreaterThan(0).WithMessage("O valor de compra deve ser maior que zero.");

            RuleFor(x => x.PurchaseDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data da compra não pode ser no futuro.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("O status fornecido não é válido.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");
        }
    }

    public class UpdateAssetValidator : AbstractValidator<UpdateAssetDto>
    {
        public UpdateAssetValidator()
        {
            // Validações similares ao Create
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PurchaseValue).GreaterThan(0);
            RuleFor(x => x.PurchaseDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
