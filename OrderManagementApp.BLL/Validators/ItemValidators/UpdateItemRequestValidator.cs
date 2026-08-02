using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
    {
        public UpdateItemRequestValidator()
        {
            RuleFor(i => i.Id)
                .GreaterThan(0).WithMessage("Id is required.");

            RuleFor(i => i.ItemName)
                .NotEmpty().WithMessage("Item Name is required.")
                .MinimumLength(3).WithMessage("Item Name must be minimum 3 characters.")
                .MaximumLength(100).WithMessage("Item Name can not exceed 100 characters.");
        }
    }
}
