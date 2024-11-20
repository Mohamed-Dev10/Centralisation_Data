using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("ContactClient")]
    public class ContactClient
    {


        [Key]
        [Column("idContactClient")]
        public int idContactClient { get; set; }


        [Column("ContactName")]
        public string clientName { get; set; }


        [Column("clientAddress")]
        public string clientAddress { get; set; }


        [Column("clientEmail")]
        public string clientEmail { get; set; }


        [Column("clientIndustry")]
        public string clientIndustry { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Client))]
        public int? idClient { get; set; }
        public virtual Client Client { get; set; }
    }
}