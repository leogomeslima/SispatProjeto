using FluentValidation;
using Sispat.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.Validators
{
    public class CreateOrUpdateCategoryValidator : AbstractValidator<CreateOrUpdateCategoryDto>
    {
        public CreateOrUpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");
        }
    }
}
