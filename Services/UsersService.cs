using System.Security.Claims;
using System.Text;
using System.Transactions;
using MyBlogWebApp.Data;
using MyBlogWebApp.Models;

namespace MyBlogWebApp.Services;

public class UsersService
{
    private MyWebAppDataContext _dataContext;

    public UsersService(MyWebAppDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public UserModel Create(UserModel userModel)
    {
        var newUser = new User
        {
            Name = userModel.Name,
            Email = userModel.Email,   
            Password = userModel.Password, // Стоит докинуть шифровку 
            Desctription = userModel.Desctription,
            Photo = userModel.Photo
        };
        
        // _dataContext.SaveChangesAsync();
        _dataContext.Users.Add(newUser);
        _dataContext.SaveChanges();
        
        userModel.Id = newUser.Id;
        
        return userModel;
    }
    
    public UserModel Update(User userToUpdate, UserModel userModel)
    {
        userToUpdate.Name = userModel.Name;
        userToUpdate.Email = userModel.Email;
        userToUpdate.Password = userModel.Password; // Стоит докинуть шифровку 
        userToUpdate.Desctription = userModel.Desctription;
        userToUpdate.Photo = userModel.Photo;
        
        _dataContext.SaveChanges();
        _dataContext.Users.Update(userToUpdate);
        
        return userModel;
    }
    
    
    public (string login, string password) GetUserLoginPassFromBasicAuth(HttpRequest request)
    {
        string userName = "";
        string userPass = "";
        string authHeader = request.Headers["Authorization"].ToString();

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            string encodedUserNamePass =  authHeader.Replace("Basic ", "");
            var encoding = Encoding.GetEncoding("iso-8859-1");
            
            string[] namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamePass)).Split(':');
            userName = namePassArray[0];
            userPass = namePassArray[1];
        } 
        
        return (userName, userPass);
    }

    public (ClaimsIdentity identity, int id)? GetIdentity(string email, string password)
    {
        User? currentUser = GetUserByLogin(email);

        if (currentUser == null || !VerifyHashedPassword(currentUser.Password, password)) return null;

        var claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
            // new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Type.ToString())
        };
        
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return (claimsIdentity, currentUser.Id);
    }

    public User? GetUserByLogin(string email)
    {
        // Логика получения пользователя из БД
        
        return _dataContext.Users.FirstOrDefault(u => u.Email == email);
    }

    public void DeleteUser(User user)
    {
        _dataContext.Users.Remove(user);
        _dataContext.SaveChanges();
    }

    public bool VerifyHashedPassword(string password, string hashedPassword)
    {
        // Здесь можно добавить логику проверки пароля
        return password ==  hashedPassword;
    }
}