using CentralisationV0.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationdeDonnee.Models
{
    [Table("datatype")]
    public class DataTyp
    {
        [Key]
        [Column("iddatatype")]
        public int ID { get; set; }
        
        [Column("format")] 
        public string format { get; set; }

        [Column("description")]
        public string description { get; set; }

        [SQLiteNetExtensions.Attributes.OneToMany(nameof(Data.Industry))]
        public virtual ICollection<Data> Datas { get; set; }

    }
}