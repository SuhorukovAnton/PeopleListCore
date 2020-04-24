using Microsoft.EntityFrameworkCore;

namespace PeopleListCore.Models
{
    public class PeopleContext : DbContext
    {
        public DbSet<People> People { get; set; }
        public PeopleContext(DbContextOptions<PeopleContext> options)
            : base(options)
        {
            Database.EnsureCreated();   
        }
    }
}
