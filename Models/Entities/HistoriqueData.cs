using CentralisationV0.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("HistoriqueData")]
    public class HistoriqueData 
    {
        [Key]
        [Column("idHistoriqueData")]
        public int idHistoriqueData { get; set; }

        [Column("UrlData")]
        public string UrlData { get; set; }


        [Column("DateMise_a_Jours")]
        public DateTime DateMise_a_Jours { get; set; }


       
        public int idData { get; set; }
        public virtual Data Data { get; set; }
    }
}