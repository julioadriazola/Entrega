using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Prueba5.Models
{

    public class RegisterModel
    {
        
        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "No es una dirección de Email válida")]
        [Required(ErrorMessage = "Debe especificar un Email")]
        [Remote("existeEmail", "Account", HttpMethod = "POST", ErrorMessage = "El email ya existe. Por favor elige otro.")]
        public string Email { get; set; }

        [StringLength(40, ErrorMessage = "El Usuario debe contener al menos {2} caracteres.", MinimumLength = 4)]
        [RegularExpression(@"(([A-Za-z]|[0-9]){0,40})",ErrorMessage="El Usuario debe contener solo letras y números.")]
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Debe especificar un Usuario")]
        [Remote("existeUsername", "Account", HttpMethod = "POST", ErrorMessage = "El usuario ya existe. Por favor elige otro.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Debe especificar un Password")]
        [Display(Name = "Contraseña")]
        [StringLength(40, ErrorMessage = "La contraseña debe contener al menos {2} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Debe ser igual a la Contraseña.")]
        public string ConfirmPassword { get; set; }

    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Debe ingresar un Usuario")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Debe ingresar su Contraseña")]
        [Display(Name="Contraseña")]
        public string Password { get; set; }

        [Display(Name="Conectarse automáticamente en cada visita")]
        public bool AutoLogin { get; set; }

    }




}