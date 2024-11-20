using System;
using System.Collections.Generic;
using CentralisationV0.Models.Entities;
using System.Linq;
using System.Web;
using CentralisationdeDonnee.Models;

namespace CentralisationV0.Models.Entities
{
    public class IndexViewModel
    {
        public List<Collaboration> Collaborations { get; set; }

        public List<Data> DataList { get; set; }
        public List<Client> Clients { get; set; }
        public List<Theme> Themes { get; set; }  // Ajouté pour les thèmes
    }
}