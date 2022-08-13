namespace SharpStore.Models;

public class UserModels
{
    public class GetUserToken
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class SignUp
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}