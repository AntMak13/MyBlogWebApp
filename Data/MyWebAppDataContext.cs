using Microsoft.EntityFrameworkCore;

namespace MyBlogWebApp.Data;

public class MyWebAppDataContext : DbContext
{
    // Создаем "наборы" для хранения данных, т.е. таблицы, которые будут у нас в БД
    
    public DbSet<User> Users { get; set; }
    
    // Конфигуратор
    public MyWebAppDataContext(DbContextOptions<MyWebAppDataContext> contextOption) :  base(contextOption)
    {
        Database.EnsureCreated();
    }
}