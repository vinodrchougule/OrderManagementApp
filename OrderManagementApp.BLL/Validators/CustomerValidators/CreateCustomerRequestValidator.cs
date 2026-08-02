using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(c => c.CustomerName)
                .NotEmpty().WithMessage("Customer Name is required.")
                .MinimumLength(3).WithMessage("Customer Name must be minimum 3 characters.")
                .MaximumLength(100).WithMessage("Customer Name can not exceed 100 characters.");
        }
    }
}
