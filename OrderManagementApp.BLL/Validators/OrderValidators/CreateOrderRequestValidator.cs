using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator() 
        {
            RuleFor(o => o.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId is required.");

            RuleFor(o => o.OrderItems)
                .NotEmpty().WithMessage("Order must have atleast one item.");

            RuleForEach(o => o.OrderItems).ChildRules(item =>
            {
                item.RuleFor(i => i.ItemId)
                    .GreaterThan(0).WithMessage("ItemId is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity is required.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("UnitPrice is required.");
            });
        }
    }
}
