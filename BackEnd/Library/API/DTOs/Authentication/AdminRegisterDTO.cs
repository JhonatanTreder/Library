﻿using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Authentication
{
    public class AdminRegisterDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O e-mail deve estar em um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "A senha deve conter de 3 a 12 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo 'Tipo de usuário' é obrigatório.")]
        [EnumDataType(typeof(UserType), ErrorMessage = "O cargo deve estar em um formato válido.")]
        public UserType UserType { get; set; }
    }
}
