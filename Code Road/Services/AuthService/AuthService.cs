﻿using Code_Road.Dto.Account;
using Code_Road.Dto.Account.Enum;
using Code_Road.Helpers;
using Code_Road.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Code_Road.Services.PostService.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _Jwt;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> Jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _Jwt = Jwt.Value;
        }

        // Register
        public async Task<AuthDto> RegisterAsync(SignUpDto model)
        {
            StateDto state = new StateDto();
            // If email is exist
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                state.Flag = false;
                return new AuthDto() { Status = state };
            }
            // If userName is exit
            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                state.Flag = false;
                state.Message = "This UserName Is Exist!!";
                return new AuthDto() { Status = state };
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                state.Flag = false;
                state.Message = errors;
                return new AuthDto { Status = state };
            }

            await _userManager.AddToRoleAsync(user, Roles.User.ToString());

            // Create Token
            var jwtSecurityToken = await CreateJwtToken(user);

            state.Flag = true;
            state.Message = "Signup Succeeded";
            return new AuthDto
            {
                Status = state,
                UserName = user.UserName,
                Email = user.Email,
                TokenExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { Roles.User.ToString() },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };

        }

        // Generate Token
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return JwtSecurityToken;
        }

        // Check User Login
        public async Task<AuthDto> LoginAsync(LoginDto model)
        {
            StateDto state = new StateDto();
            var authModel = new AuthDto();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                state.Flag = false;
                state.Message = "Email or Password is incorrect!";
                authModel.Status = state;
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            state.Flag = true;
            state.Message = "Login Successfully";

            authModel.Status = state;
            authModel.Email = user.Email;
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.TokenExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            return authModel;
        }

        // Add User To Role
        public async Task<StateDto> AddUserToRoleAsync(AddUserToRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            // if user not found
            if (user is null)
                return new StateDto() { Flag = false, Message = "User Not Found" };
            // if role not found
            if (await _roleManager.RoleExistsAsync(model.Role) == false)
                return new StateDto() { Flag = false, Message = $"{model.Role} Role Not Found" };

            // if user is assigned to this role
            if (await _userManager.IsInRoleAsync(user, model.Role) == true)
                return new StateDto() { Flag = false, Message = "User is already assigned to this role" };

            await _userManager.AddToRoleAsync(user, model.Role);
            return new StateDto() { Flag = true, Message = "User Added To Role Successfully" };
        }

        // get user name from id
        public async Task<string> GetUserName(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is not null)
                return user.FirstName + " " + user.LastName;
            return string.Empty;
        }

        // Update Password
        public async Task<StateDto> UpdatePassword(UpdatePasswordDto model)
        {
            StateDto status = new StateDto();
            var user = await _userManager.FindByEmailAsync(model.Gmail);
            if (user is not null)
            {
                bool isTrue = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (isTrue)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        status.Flag = true;
                        status.Message = "Password Updated Successfully";
                        return status;
                    }
                    else
                    {
                        status.Flag = false;
                        status.Message = result.Errors.ToString();
                        return status;
                    }
                }
            }
            status.Flag = false;
            status.Message = "Old Password or Email Incorrect";
            return status;
        }


        // admin Delete User ==> you should delete anything related to this user from all tables
        public async Task<StateDto> DeleteUser(DeleteUserDto model)
        {
            StateDto status = new StateDto();
            var admin = await _userManager.FindByEmailAsync(model.AdminGmail);
            var user = await _userManager.FindByEmailAsync(model.UserGmail);

            if (admin is not null && user is not null)
            {
                bool isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");
                if (isAdmin)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        result = await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            status.Flag = true;
                            status.Message = "User Deleted Successfully";
                            return status;
                        }
                    }
                    status.Flag = false;
                    status.Message = result.Errors.ToString();
                    return status;
                }
                status.Flag = false;
                status.Message = "You Don't have Permission To Delete User";
                return status;
            }
            status.Flag = false;
            status.Message = "Admin or User Email Incorrect";
            return status;
        }
    }
}
