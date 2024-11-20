using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{
    [Table("ArcgisSolution")]
    public class ArcgisSolution
    {

        [Key]
        [Column("idArcgisSolution")]
        public int idArcgisSolution { get; set; }


        [Column("TitredArcgisSolution")]
        public string TitredArcgisSolution { get; set; }

        [Column("DescriptionArcgisSolution")]
        public string DescriptionRole { get; set; }

        [Column("KeywordsArcgisSolution")]
        public string KeywordsArcgisSolution { get; set; }


        [ManyToMany(typeof(Client))]
        public virtual ICollection<Client> Clients { get; set; }
    }
}