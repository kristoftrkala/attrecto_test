﻿namespace Attrecto.Dtos.User
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
    }
}
