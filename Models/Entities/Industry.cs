using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{

    [Table("industry")]
    public class Industry
    {
        [Key]
        [Column("idIndustry")]
        public int IdUseConstraint { get; set; }


        [Column("nomIndustry")]
        public string nom { get; set; }



        [Column("Keywords")]
        public string Keywords { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Theme))]
        public int ThemeId { get; set; }
        public virtual Theme Theme { get; set; }


      

       
    }
}