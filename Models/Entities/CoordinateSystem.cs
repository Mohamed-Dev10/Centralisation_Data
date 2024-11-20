using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class CoordinateSystem
    {
        [Key]
        public int IdCoordinateSystem { get; set; }  // Définir la clé primaire
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Data> Data { get; set; }
    }
}