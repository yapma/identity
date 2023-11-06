using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleRoleBase.Models;

namespace SimpleRoleBase.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // data seed
            this.SeedRoles(builder);
            this.SeedUsers(builder);
            this.SeedUserRoles(builder);

            base.OnModelCreating(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            User user = new User()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                Email = "admin@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                FirstName = "Admin FirstName",
                LastName = "Admin LastName",
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

            builder.Entity<User>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = Enums.Roles.Owner.ToString(), ConcurrencyStamp = "1", NormalizedName = Enums.Roles.Owner.ToString() },
                new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = Enums.Roles.Admin.ToString(), ConcurrencyStamp = "2", NormalizedName = Enums.Roles.Admin.ToString() },
                new IdentityRole() { Id = "c7b013f0-5101-4317-abd8-c211f91b7320", Name = Enums.Roles.Customer.ToString(), ConcurrencyStamp = "3", NormalizedName = Enums.Roles.Customer.ToString() }
                );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
                );
        }

    }
}
