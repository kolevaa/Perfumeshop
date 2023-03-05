using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Perfumeshop.Areas.Identity.Data;
using Perfumeshop.Models;

namespace Perfumeshop.Data
{
    public class PerfumeshopContext : IdentityDbContext<PerfumeshopUser>

    {
        public PerfumeshopContext (DbContextOptions<PerfumeshopContext> options)
            : base(options)
        {
        }

        public DbSet<Perfumeshop.Models.Perfume>? Perfume { get; set; }

        public DbSet<Perfumeshop.Models.Brand>? Brand { get; set; }

        public DbSet<Perfumeshop.Models.User>? User { get; set; }

        public DbSet<Perfumeshop.Models.Order>? Order { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>()
                .HasOne<User>(d => d.User)
                .WithMany(x => x.Perfumes)
                .HasForeignKey(d => d.UserId);
            builder.Entity<Order>()
                .HasOne<Perfume>(m => m.Perfume)
                .WithMany(x => x.Users)
                .HasForeignKey(m => m.PerfumeId);
            builder.Entity<Perfume>()
                .HasOne<Brand>(d => d.Brand)
                .WithMany(x => x.Perfumes)
                .HasForeignKey(m => m.BrandId);
        }

    }
}
