using GPMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

           builder.Entity<Department>().HasData(
      new Department { Id = 1, Name = "Computer Systems Engineering", HeadOfDepartment = "Dr. Thaer Samar"},
      new Department { Id = 2, Name = "Electrical Engineering", HeadOfDepartment = "Dr. Mahmoud Ismail" },
      new Department { Id = 3, Name = "Mechatronics Engineering", HeadOfDepartment = "Dr. Nabil Al-Tanna" },
      new Department { Id = 4, Name = "Mechanical Engineering", HeadOfDepartment = "Dr. Jafar Masri" },
      new Department { Id = 5, Name = "Telecommunications Engineering and Technology", HeadOfDepartment = "Mr. Mahmoud Sawalha"},
      new Department { Id = 6, Name = "Sustainable Energy Engineering", HeadOfDepartment = "Dr. Nabil Al-Tanna"},
      new Department { Id = 7, Name = "Civil Engineering and Surveying", HeadOfDepartment = "Mr. Bassel Salameh" },
      new Department { Id = 8, Name = "Architectural Engineering", HeadOfDepartment = "Dr. Shaher Ziyod" });
        }

        public DbSet<Project>  Projects { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
