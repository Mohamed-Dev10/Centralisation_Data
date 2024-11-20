using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "La date de naissance est requise.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de Naissance")]
        public DateTime DateNaissance { get; set; }

        [Required(ErrorMessage = "L'adresse est requise.")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required(ErrorMessage = "La profession est requise.")]
        [Display(Name = "Profession")]
        public string Proffession { get; set; }

        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Adresse email invalide.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de Passe")]
        public string Password { get; set; }

        
    }

}