using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentralisationdeDonnee.Models;




namespace CentralisationV0.Models.Entities
{
    public class Data
    {
        [Key]
        public int IdData { get; set; }

        [Required(ErrorMessage = "Le titre est requis.")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Display(Name = "Date de Réception")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AcquisitionDate { get; set; }

        [Display(Name = "Date de Publication")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Date Dernières Mise à Jour")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdatedDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Catégorie")]
        public string Category { get; set; }

        [StringLength(255)]
        [Display(Name = "Télecommunication")]
        public string Telecommunication { get; set; }
        [NotMapped] // Indique que cette propriété n'est pas mappée à la base de données
        public string ThemeName { get; set; }

        [Display(Name = "Thème")]
        public int ThemeId { get; set; }

        [ForeignKey("ThemeId")]
        public virtual Theme Theme { get; set; }

        [StringLength(255)]
        [Display(Name = "Couverture")]
        public string Coverage { get; set; }

        [Display(Name = "Résolution Spatial")]
        public double SpatialResolution { get; set; }

        [StringLength(500)]
        [Display(Name = "Résumé")]
        public string Summary { get; set; }

        [MaxLength(1000)]
        public string UrlData { get; set; }

        [StringLength(255)]
        public string Keywords { get; set; }

        public int DataSize { get; set; }

        // Navigation properties
        // Non-mapped property for IndustryName
        [NotMapped]
        public string IndustryName { get; set; }

        [ForeignKey(nameof(Industry))]
        public int IndustryId { get; set; }
        public virtual Industry Industry { get; set; }


        [ForeignKey(nameof(CoordinateSystem))]
        public int CoordinateSystemId { get; set; }
        public virtual CoordinateSystem CoordinateSystem { get; set; }


        [ForeignKey(nameof(DataType))]
        public int DataTypeId { get; set; }
        public virtual DataTyp DataType { get; set; }
        
        [SQLiteNetExtensions.Attributes.OneToMany(nameof(HistoriqueData.Data))]
        public virtual ICollection<HistoriqueData> Historiques { get; set; }


        [SQLiteNetExtensions.Attributes.ManyToMany(typeof(Location))]
        public virtual ICollection<Location> Locations { get; set; }


        [SQLiteNetExtensions.Attributes.ManyToMany(typeof(UseConstraint))]
        public virtual ICollection<UseConstraint> UseConstraints { get; set; }
        [SQLiteNetExtensions.Attributes.ManyToMany(typeof(DataBase))]
        public virtual ICollection<DataBase> DataBases { get; set; }


        [SQLiteNetExtensions.Attributes.ManyToMany(typeof(Collaboration))]
        public virtual ICollection<Collaboration> Collaborations { get; set; }

        public Data()
        {
            Collaborations = new List<Collaboration>();
        }
    }
}
