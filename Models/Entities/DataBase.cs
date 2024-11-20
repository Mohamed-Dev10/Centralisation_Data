using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CentralisationV0.Models.Entities;


namespace CentralisationV0.Models.Entities
{

    [Table("database")]
    public class DataBase
    {
        [Key]
        [Column("idDataBase")]
        public int idDataBase { get; set; }


        [Column("DataBaseName")]
        public string DataBaseName { get; set; }

        [Column("Owner")]
        public string Owner { get; set; }

        
        [Column("createdDate")]
        public DateTime createdDate { get; set; }

            
        [Column("description")]
        public string description { get; set; }


        [Column("Keywords")]
        public string Keywords { get; set; }
        public virtual ICollection<Data> Datas { get; set; }






    }
}