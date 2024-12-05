using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniqIo.Models;

namespace UniqIo.DAL;

public class UniqIoDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    public DbSet<ProductComment> ProductComments { get; set; }

    public UniqIoDbContext(DbContextOptions opt) :base(opt)
        {
            
        }

}
