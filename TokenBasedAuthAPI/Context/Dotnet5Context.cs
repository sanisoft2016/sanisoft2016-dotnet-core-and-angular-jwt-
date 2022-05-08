using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenBasedAuthAPI.Models;

namespace TokenBasedAuthAPI.Context
{
    public class Dotnet5Context : IdentityDbContext<ApplicationUser>
    {
        public Dotnet5Context(DbContextOptions<Dotnet5Context> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//
            //modelBuilder.Seed();
        }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<EmployeeModel> EmployeeModels { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
