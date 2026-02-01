using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Infrastracture.Persistence
{
    public class PetWorldDbContext : DbContext
    {
        public PetWorldDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        protected PetWorldDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Question).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Answer).IsRequired().HasMaxLength(5000);
                entity.Property(e => e.Iterations).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
