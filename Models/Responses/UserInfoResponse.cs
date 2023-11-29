using ShineSyncControl.Models.DB;
namespace ShineSyncControl.Models.Responses
{
    public class UserInfoResponse : BaseResponse
    {
        public UserInfoResponse(User user) : base(true, null)
        {
            Data = new View(user);
        }

        class View
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }

            public View(User user)
            {
                Id = user.Id;
                FirstName = user.FirstName;
                LastName = user.LastName;
                Email = user.Email;
                Role = user.Role;
            }
        }
    }
}
