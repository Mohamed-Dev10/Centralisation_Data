using CentralisationV0.Models.Entities;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("Collaboration")]
    public class Collaboration
    {
        [Key]
        [Column("idCollaborateur")]
        public int idCollaborateur { get; set; }

        [Column("Titre")]
        public string Titre { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("Status")]
        public string Status { get; set; }

        [Column("Duration")]
        public int Duration { get; set; }


        [Column("MarketNumber")]
        public int MarketNumber { get; set; }


        [Column("Keywords")]
        public int Keywords { get; set; }
        [Column("LivrablesPaths")]
        public string LivrablesPaths { get; set; } // Stocke les chemins des fichiers


        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Client))]
        public int idClient { get; set; }
        public virtual Client Client { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(TypeCollaboration))]
        public int idTypeCollaboration { get; set; }
        public virtual TypeCollaboration TypeCollaboration { get; set; }

        [ManyToMany(typeof(Data))]
        public virtual ICollection<Data> Datas { get; set; }


        [ManyToMany(typeof(ApplicationUser))]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public Collaboration()
        {
            Datas = new List<Data>();
        }

    }
}