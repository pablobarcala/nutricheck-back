﻿namespace NutriCheck.Backend.Dtos
{
    public class RegistroUserDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; }
    }
}
