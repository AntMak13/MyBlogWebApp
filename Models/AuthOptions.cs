using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyBlogWebApp.Models;

public class AuthOptions
{
    public const string ISSUER = "MyBlogWebApp";
    
    public const string AUDIENCE = "MyBlogWebApp";

    private const string KEY = "mysupersecret_secretkey!1234567890";

    public const int LIFETIME = 10;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}