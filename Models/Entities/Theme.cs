using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentralisationV0.Models.Entities;
using Newtonsoft.Json;

namespace CentralisationdeDonnee.Models
{
    [Table("theme")]
    public class Theme
    {
        [Key]
        [Column("IdTheme")]
        public int IdTheme { get; set; }  // Renommé pour correspondre à la colonne dans la base de données

        [Column("nom")]
        public string nom { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("Keywords")]
        public string Keywords { get; set; }

        [OneToMany(nameof(Industry.Theme))]
        public virtual ICollection<Industry> Industries { get; set; }

        [JsonIgnore] // Ignorer la sérialisation de cette propriété
        public virtual ICollection<Data> Datas { get; set; }
    }
}
