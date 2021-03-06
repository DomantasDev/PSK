﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PSK.Domain.Identity;
using PSK.Services;
using PSK.Services.Interfaces;

namespace PSK.FrontEnd.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly RoleManager<UserRole> _roleManager;

        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public EmployeeController(IEmployeeService employeeService, RoleManager<UserRole> roleManager,IMapper mapper, UserManager<Employee> userManager)
        {
            _employeeService = employeeService;
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Employees()
        {
            return View(await _employeeService.GetAll());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeDto employeeDto)
        {
            var selectedRoles = employeeDto.Roles.Where(r => r.IsSelected).Select(r => r.Role).ToList();
            await _employeeService.Create(_mapper.Map<Employee>(employeeDto), employeeDto.Password, selectedRoles);
            return Redirect("employees");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddNew()
        {
            return View(new EmployeeDto
            {
                Roles = _roleManager.Roles.Where(r => r.Name.ToLower() != "user").Select(r => new RoleSelection
                {
                    Role = r.Name,
                    IsSelected = false
                }).ToList()
            });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.Delete(id);
            return Redirect("/employee/employees");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await _employeeService.Get(id);

            if (employee == null)
                return NotFound();
            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            var currentRoles = await _userManager.GetRolesAsync(employee);

            employeeDto.Roles = _roleManager.Roles.Where(r => r.Name.ToLower() != "user").Select(r => new RoleSelection
            {
                Role = r.Name,
                IsSelected = currentRoles.Contains(r.Name)
            }).ToList();

            return View(employeeDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(EmployeeDto employeeDto)
        {
            var selectedRoles = employeeDto.Roles.Where(r => r.IsSelected).Select(r => r.Role).ToList();
            await _employeeService.Update(_mapper.Map<Employee>(employeeDto),  selectedRoles);
            return Redirect("employees");
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            return View(_mapper.Map<EmployeeDto>(await _employeeService.Get(id)));
        }
    }
}