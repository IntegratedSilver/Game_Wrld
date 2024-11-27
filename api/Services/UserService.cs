using BCrypt.Net;
using api.Models;
using api.Models.DTO;
using api.Services.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace api.Services;

public class UserService : ControllerBase
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;   
    }

    // Check if a user exists by username
    public bool DoesUserExist(string username)
    {
        return _context.UserInfo.SingleOrDefault(user => user.Username == username) != null;
    }

    public bool AddUser(CreateAccountDTO userToAdd)
    {
        bool result = false;
        if (!DoesUserExist(userToAdd.Username))
        {
            UserModel newUser = new UserModel
            {
                Username = userToAdd.Username,
                Email = userToAdd.Email, 
                Hash = BCrypt.Net.BCrypt.HashPassword(userToAdd.Password),
            };

            _context.Add(newUser);
            result = _context.SaveChanges() != 0;
        }
        return result;
    }


    public bool VerifyUserPassword(string enteredPassword, string storedHash)
    {
        if (enteredPassword == null) throw new ArgumentNullException(nameof(enteredPassword));

        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }


    public IActionResult Login(LoginDTO user)
    {
        IActionResult result = Unauthorized();
        var foundUser = _context.UserInfo.SingleOrDefault(u => u.Username == user.UserName);
        if (foundUser != null && VerifyUserPassword(user.Password, foundUser.Hash))
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForJWT"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: new List<Claim> { new Claim(ClaimTypes.Name, foundUser.Username) },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            result = Ok(new { Token = tokenString });
        }
        return result;
    }


    public IEnumerable<UserModel> GetAllUsers()
{
    return _context.UserInfo.ToList(); 
}
   public UserIdDTO GetUserIdDTOByUserName(string username)
{
    var user = _context.UserInfo.FirstOrDefault(u => u.Username == username);
    if (user == null) return null;

    return new UserIdDTO
    {
        UserId = user.Id,
        Username = user.Username
    };
}

}
