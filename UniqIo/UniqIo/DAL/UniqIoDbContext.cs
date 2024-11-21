using Microsoft.EntityFrameworkCore;
using UniqIo.Models;

namespace UniqIo.DAL;

public class UniqIoDbContext : DbContext
{
    public DbSet<Slider> Sliders { get; set; }

        public UniqIoDbContext(DbContextOptions opt) :base(opt)
        {
            
        }

}
