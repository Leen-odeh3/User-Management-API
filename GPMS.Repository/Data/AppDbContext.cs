using GPMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPMS.Repository.Data
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions options):base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);

        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
                (
                new IdentityRole() {Id="1", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() {Id = "2", Name = "Student", ConcurrencyStamp = "2", NormalizedName = "Student" },
                new IdentityRole() {Id = "3", Name = "HeadOfDepartment", ConcurrencyStamp = "3", NormalizedName = "HeadOfDepartment" },
                new IdentityRole() {Id = "4", Name = "Supervisor", ConcurrencyStamp = "4", NormalizedName = "Supervisor" }


                );
        }

        public DbSet<Project>  Projects { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
