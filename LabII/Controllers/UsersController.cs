﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LabII.DTOs;
using LabII.Models;
using LabII.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabII.Controllers
{
    // https://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
    //[Route("api/[controller]/[action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersService _userService;
        private IUserUserRolesService userUserRolesService;
        public UsersController(IUsersService userService, IUserUserRolesService userUserRolesService)
        {
            _userService = userService;
            this.userUserRolesService = userUserRolesService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginPostModel login)
        {
            var user = _userService.Authenticate(login.Username, login.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
         // [HttpPost]
        public IActionResult Register([FromBody]RegisterPostModel register)
        {
            var errors = _userService.Register(register);//, out User user);
            if (errors != null)
            {
                return BadRequest(errors);
            }
            return Ok();//(user);
        }

        [HttpGet]
        // [Authorize(Roles = "Admin,UserManager")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize(Roles = "Admin,UserManager")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public void Post([FromBody] UserPostModel user)
        {

            _userService.Create(user);
        }

        [Authorize(Roles = "Admin,UserManager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User currentLoggedUser = _userService.GetCurrentUser(HttpContext);
            string loggedRoleName = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            var regDate = currentLoggedUser.CreatedAt;
            var currentDate = DateTime.Now;
            var minDate = currentDate.Subtract(regDate).Days / (365 / 12);

            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToDelete = _userService.GetById(id);
                string userUserRoleToDelete = userUserRolesService.GetById(getUserToDelete.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToDelete.Equals("Admin"))
                {
                    return Forbid();
                }

            }

            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToDelete = _userService.GetById(id);
                string userUserRoleToDelete = userUserRolesService.GetById(getUserToDelete.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToDelete.Equals("UserManager") && minDate <= 6)

                    return Forbid();
            }

            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToDelete = _userService.GetById(id);
                string userUserRoleToDelete = userUserRolesService.GetById(getUserToDelete.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToDelete.Equals("UserManager") && minDate >= 6)
                {
                    var result1 = _userService.Delete(id);
                    return Ok(result1);
                }


            }

            var result = _userService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }


            return Ok(result);
        }



        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,UserManager")]
        public IActionResult Get(int id)
        {
            var found = _userService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }

            return Ok(found);
        }
        [Authorize(Roles = "Admin,UserManager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User user)
        {
            User currentLoggedUser = _userService.GetCurrentUser(HttpContext);
            string loggedRoleName = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            var regDate = currentLoggedUser.CreatedAt;
            var currentDate = DateTime.Now;
            var minDate = currentDate.Subtract(regDate).Days / (365 / 12);


            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToUp = _userService.GetById(id);

                if (getUserToUp == null)
                {
                    return NotFound();
                }

            }


            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToUp = _userService.GetById(id);
                string userUserRoleToUp = userUserRolesService.GetById(getUserToUp.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToUp.Equals("Admin"))

                    return Forbid();
            }

            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToUp = _userService.GetById(id);
                string userUserRoleToUp = userUserRolesService.GetById(getUserToUp.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToUp.Equals("UserManager") && minDate <= 6)

                    return Forbid();
            }


            if (loggedRoleName.Equals("UserManager"))
            {
                User getUserToUp = _userService.GetById(id);
                string userUserRoleToUp = userUserRolesService.GetById(getUserToUp.Id)
                       .FirstOrDefault(uurole => uurole.EndTime == null)
                       .UserRoleName;
                if (userUserRoleToUp.Equals("UserManager") && minDate >= 6)
                {
                    var result1 = _userService.Upsert(id, user);
                    return Ok(result1);
                }

            }


            var result = _userService.Upsert(id, user);
            return Ok(result);


        }

    }
}
    

