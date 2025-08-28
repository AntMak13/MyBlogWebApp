using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyBlogWebApp.Data;
using MyBlogWebApp.Models;
using MyBlogWebApp.Services;

namespace MyBlogWebApp.Controllers
{
    [Route("[Controller]")]
    [Authorize]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private UsersService _usersService;
        
        public AccountController(MyWebAppDataContext dataContext)
        {
            _usersService = new UsersService(dataContext);
        }
        
        [HttpGet]
        [AllowAnonymous] // Значит, что для того, чтобы дернуть метод авторизация не нужна 
        public IActionResult Get()
        {
            var currentUserEmail = HttpContext.User.Identity.Name;
            var currentUser = _usersService.GetUserByLogin(currentUserEmail);

            if (currentUser != null)
            {
                return BadRequest();
            } 
            
            return Ok(new UserModel
            {
                Id = currentUser.Id,
                Name = currentUser.Name,
                Email = currentUser.Email,
                Desctription = currentUser.Desctription,
                Photo = currentUser.Photo,
            });
            
        }

        [HttpPost]
        public IActionResult Create(UserModel user)
        {
            // add user to DB
            var newUser = _usersService.Create(user);
            
            return Ok(newUser);
            
            // _usersService.Create(user);
            // return Ok(user);
        }

        [HttpPatch]
        public IActionResult Update(UserModel user)
        {
            // check current user from request with user model 
            var currentUserEmail = HttpContext.User.Identity.Name;
            var currentUser = _usersService.GetUserByLogin(currentUserEmail);

            if (currentUser != null && currentUser?.Id != user.Id)
            {
                return BadRequest();
            }
            // update user in DB
            _usersService.Update(currentUser, user);
         
            return Ok(user);
        }

        [HttpDelete]
        public IActionResult Delete(int userId)
        {
            var currentUserEmail = HttpContext.User.Identity.Name;
            var currentUser = _usersService.GetUserByLogin(currentUserEmail);
            _usersService.DeleteUser(currentUser);
            
            // можно добавить обработчик ошибки при удалении пользователя
            
            return Ok();
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public IActionResult GetToken()
        {
            
            // 1. get user data from DB
            var userData = _usersService.GetUserLoginPassFromBasicAuth(Request);
            // 2. get identity
            (ClaimsIdentity claims, int id)? identity = _usersService.GetIdentity(userData.login, userData.password);
            if (identity is null ) return NotFound("Login or password is incorrect");
            // 3. create jwt token
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity?.claims.Claims,
                expires: now.AddMinutes(AuthOptions.LIFETIME),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            // 4. return token
            var tokenModel = new AuthToken(
                minutes: AuthOptions.LIFETIME,
                accessToken: encodedJwt,
                userName: userData.login,
                userId: identity.Value.id);

            return Ok(tokenModel);
        }
    }
    
}

