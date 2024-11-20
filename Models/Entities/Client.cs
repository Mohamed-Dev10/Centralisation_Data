using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("Client")]
    public class Client
    {

        [Key]
        [Column("idClient")]
        public int idClient { get; set; }


        [Column("clientName")]
        public string clientName { get; set; }


        [Column("clientAddress")]
        public string clientAddress { get; set; }


        [Column("clientEmail")]
        public string clientEmail { get; set; }


        [Column("clientIndustry")]
        public string clientIndustry { get; set; }

        [Column("clientSize")]
        public int clientSize { get; set; }


        [Column("clientType")]
        public string clientType { get; set; }


        [Column("Keywords")]
        public string Keywords { get; set; }


        [SQLiteNetExtensions.Attributes.OneToMany(nameof(Collaborations))]
        public virtual ICollection<Collaboration> Collaborations { get; set; }



        [SQLiteNetExtensions.Attributes.OneToMany(nameof(ContactClient.Client))]
        public virtual ICollection<ContactClient> ContactClients { get; set; }


        [ManyToMany(typeof(ArcgisSolution))]
        public virtual ICollection<ArcgisSolution> ArcgisSolutions { get; set; }

    }
}