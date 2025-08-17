namespace PostsBlogApi.DTOs
{
    public class RegisterDto
    {
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
    }

    public class LoginDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
