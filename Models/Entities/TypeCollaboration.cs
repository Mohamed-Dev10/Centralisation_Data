using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("TypeCollaboration")]
    public class TypeCollaboration
    {
        [Key]
        [Column("idTypeCollaboration")]
        public int idTypeCollaboration { get; set; }

        [Column("NomType")]
        public string NomType { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Keywords")]
        public string Keywords { get; set; }

        [SQLiteNetExtensions.Attributes.OneToMany(nameof(Collaboration.TypeCollaboration))]
        public virtual ICollection<Collaboration> Collaborations { get; set; }
    }
}