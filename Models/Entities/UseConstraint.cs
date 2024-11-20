using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class UseConstraint
    {
        [Key]
        public int IdUseConstraint { get; set; }  // Définir la clé primaire (nom changé pour la cohérence)
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Data> Data { get; set; }
    }
}