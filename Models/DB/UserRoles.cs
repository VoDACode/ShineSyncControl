namespace ShineSyncControl.Models.DB
{
    public class UserRoles
    {
        public string Role { get; set; }

        public const string User = "User";
        public const string Admin = "Admin";
        public const string Registrar = "Registrar";

        public UserRoles() { }

        public UserRoles(string role)
        {
            Role = role;
        }

        public static implicit operator string(UserRoles role)
        {
            return role.Role;
        }
    }
}
