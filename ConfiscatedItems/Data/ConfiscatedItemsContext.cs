using ConfiscatedItems.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfiscatedItems.Data
{
    public class ConfiscatedItemsContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=Localhost;Initial Catalog=ConfiscatedItems;Integrated Security=True");
        }

    }
}
