namespace MyBlogWebApp.Data;

/*
 * Поля одинаковые тут и в UserModel.cs 
 * Это (User.cs) используется для EntityFramework
 * А UserModel используется для передачи данных
 */

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string? Desctription { get; set; }
    
    public byte[]? Photo { get; set; }
}