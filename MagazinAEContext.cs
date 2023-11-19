using Microsoft.EntityFrameworkCore;
using MagazinAE.Models.Entities;

namespace MagazinAE
{
    public class MagazinAEContext : DbContext
    {
        public MagazinAEContext(DbContextOptions<MagazinAEContext> options)
            : base(options) 
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
