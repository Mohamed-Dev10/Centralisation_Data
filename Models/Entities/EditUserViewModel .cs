using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

       
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "La profession est requise.")]
        [Display(Name = "Proffession")]
        public string Proffession { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateNaissance { get; set; }


        [Required(ErrorMessage = "Email est requis.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Role { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Mot de Passe actuel")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nouveau Mot de Passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau Mot de Passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation du mot de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }


}