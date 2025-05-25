using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Request;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Получить неполные данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("short/{id:guid}", Name = "GetEmployeeShort")]
        [ProducesResponseType(typeof(EmployeeShortResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeShortResponse>> GetShortEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeShortResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                FullName = employee.FullName,
            };

            return Ok(employeeModel);
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="employeeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateEmployeeAsync(CreateEmployeeRequest employeeRequest)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Email = employeeRequest.Email,
                Roles = new List<Role>(),
                FirstName = employeeRequest.FirstName,
                LastName = employeeRequest.LastName,
                AppliedPromocodesCount = employeeRequest.AppliedPromocodesCount
            };
            var createdEmployee = await _employeeRepository.CreateAsync(employee);
            if (createdEmployee == null) return Problem("Не удалось создать сотрудника");
            var employeeShortResponse = new EmployeeShortResponse()
            {
                Id = createdEmployee.Id,
                Email = createdEmployee.Email,
                FullName = createdEmployee.FullName,
            };

            return CreatedAtRoute("GetEmployeeShort", new { id = createdEmployee.Id }, employeeShortResponse);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, UpdateEmployeeRequest request)
        {
            var employeeExist = await _employeeRepository.GetByIdAsync(id);

            if (employeeExist == null)
                return NotFound();

            employeeExist.Email = request.Email;
            employeeExist.FirstName = request.FirstName;
            employeeExist.LastName = request.LastName;
            employeeExist.AppliedPromocodesCount = request.AppliedPromocodesCount;

            _employeeRepository.Update(employeeExist);
            return Ok(id);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _employeeRepository.Delete(id);
            return NoContent();

        }
    }
}