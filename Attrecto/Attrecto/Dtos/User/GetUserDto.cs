namespace Attrecto.Dtos.User
{
    public class GetUserDto
    {
        public int IdUser { get; set; }
        public int FkRole { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
    }
}
