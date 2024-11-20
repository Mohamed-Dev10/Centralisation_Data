using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class EditUtilisateurViewModel
    {


        public string UserId { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Display(Name = "Date de Naissance")]
        [DataType(DataType.Date)]
        public DateTime? DateNaissance { get; set; }

        [Display(Name = "Profession")]
        public string Proffession { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }



        [DataType(DataType.Password)]
        [Display(Name = "Nouveau Mot de Passe")]
        public string Password { get; set; }


        public List<NotificationViewModel> Notifications { get; set; }
    }
}