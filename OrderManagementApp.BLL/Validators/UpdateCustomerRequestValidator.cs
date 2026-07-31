using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerRequestValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0).WithMessage("Id is required.");

            RuleFor(c => c.CustomerName)
                .NotEmpty().WithMessage("Customer Name is required.")
                .MinimumLength(3).WithMessage("Customer Name must be minimum 3 characters.")
                .MaximumLength(100).WithMessage("Customer Name can not exceed 100 characters.");
        }
    }
}
