using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
    {
        public UpdateOrderRequestValidator() 
        {
            RuleFor(o => o.OrderId)
                .NotEmpty().WithMessage("OrderId is required.");

            RuleFor(o => o.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId is required.");

            RuleFor(o => o.OrderDate)
                .NotEmpty().WithMessage("OrderDate is requird.")
                .NotEqual(DateTime.MinValue).WithMessage("OrderDate is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("OrderDate cannot be future date.");

            RuleFor(o => o.RowVersion)
                .NotEmpty().WithMessage("RowVersion is required to prevent concurrency conflict.");

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
