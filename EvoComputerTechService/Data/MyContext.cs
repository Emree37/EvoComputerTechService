using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {

        }

        public DbSet<Issue> Issues { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<IssueProducts> IssueProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(8, 2);

            builder.Entity<IssueProducts>()
                .Property(x => x.Price)
                .HasPrecision(8, 2);

            builder.Entity<IssueProducts>()
                .HasKey(x => new { x.IssueId, x.ProductId }); //Composite Key
            
        }
    }
}
