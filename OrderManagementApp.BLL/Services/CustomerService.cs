using FluentValidation;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.BLL.Validators;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CreateCustomerRequestValidator _createCustomerRequestValidator;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _createCustomerRequestValidator = new CreateCustomerRequestValidator();
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _createCustomerRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _customerRepository.ExistsByNameAsync(dto.CustomerName, ct);

            if (nameExists)
                throw new ValidationException("Customer Name already exists.");

            var customer = CustomerMapper.ToEntity(dto);

            var created = await _customerRepository.CreateAsync(customer, ct);

            return CustomerMapper.ToResponse(created);
        }
    }
}
