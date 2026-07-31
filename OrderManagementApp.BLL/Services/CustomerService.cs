using FluentValidation;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.BLL.Validators;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Common.Exceptions;
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
        private readonly UpdateCustomerRequestValidator _updateCustomerRequestValidator;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _createCustomerRequestValidator = new CreateCustomerRequestValidator();
            _updateCustomerRequestValidator = new UpdateCustomerRequestValidator();
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _createCustomerRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _customerRepository.ExistsByNameAsync(dto.CustomerName, ct: ct);

            if (nameExists)
                throw new ValidationException("Customer Name already exists.");

            var customer = CustomerMapper.ToEntity(dto);

            var created = await _customerRepository.CreateAsync(customer, ct);

            return CustomerMapper.ToResponse(created);
        }

        public async Task<List<CustomerResponse>> GetAllAsync(CancellationToken ct = default)
        {
            var customers = await _customerRepository.GetAllAsync(ct);

            return CustomerMapper.ToResponseList(customers);
        }

        public async Task<CustomerResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, ct);

            if (customer is null)
                throw new NotFoundException("Customer", "id", id);

            return CustomerMapper.ToResponse(customer);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _updateCustomerRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _customerRepository.ExistsByNameAsync(dto.CustomerName, dto.Id, ct);

            if (nameExists)
                throw new ValidationException("Customer Name already exists.");

            var customer = CustomerMapper.ToEntity(dto);

            var updated = await _customerRepository.UpdateAsync(id, customer, ct);

            if (!updated)
                throw new NotFoundException("Customer", "id", id);

            return updated;
        }
    }
}
