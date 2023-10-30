using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Tools
{
    public static class DbInit
    {
        private static bool _isInitialized = false;

        public static void Initialize(DbApp context)
        {
            if (_isInitialized)
            {
                return;
            }
            context.Database.EnsureCreated();
            _isInitialized = true;

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<UserRoles>
                {
                    new UserRoles(UserRoles.User),
                    new UserRoles(UserRoles.Registrar),
                    new UserRoles(UserRoles.Admin)
                });
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(new List<User>()
                {
                    new User
                    {
                        Email = "registrar@example.com",
                        Password = PasswordHasher.Hash("registrar"),
                        FirstName = "registrar",
                        LastName = "registrar",
                        IsActivated = true,
                        Role = UserRoles.Registrar
                    },
                    new User
                    {
                        Email = "admin@example.com",
                        Password = PasswordHasher.Hash("admin"),
                        FirstName = "admin",
                        LastName = "admin",
                        IsActivated = true,
                        Role = UserRoles.Admin
                    }
                });
                context.SaveChanges();
            }
        }
    }
}
