using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace UpdatingProjects.Models
{
    public partial class UpdatingDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public UpdatingDbContext(DbContextOptions<UpdatingDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public virtual DbSet<ProjectsDb> ProjectsDbs { get; set; }
        public DbSet<UserModel> LoginData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config["DbConnectionString"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
