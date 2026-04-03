namespace Documentor.Core.Models.User
{
    public class CreateUserModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
    }
}