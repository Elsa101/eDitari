namespace eDitari.Dtos
{
    public class RegisterStaffDto
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;  // plain password input
    }
}
