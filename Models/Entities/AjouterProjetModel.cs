using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class AjouterProjetModel
    {
        public string Titre { get; set; }
        public string ClientId { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }
        public string TypeCollaboration { get; set; }
        public string Status { get; set; }  // Ajout du champ Status
        public string[] SelectedDonnees { get; set; }
    }


}