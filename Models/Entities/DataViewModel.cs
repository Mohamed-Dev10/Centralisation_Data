using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.ComponentModel.DataAnnotations;

namespace CentralisationV0.Models.Entities
{
    public class DataViewModel
    {
        public int IdData { get; set; }

        [Required(ErrorMessage = "Le titre est requis.")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Display(Name = "Date de Réception")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AcquisitionDate { get; set; }

        [Display(Name = "Date de Publication")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Date Dernières Mise à Jour")]
        [DataType(DataType.Date)]
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

        [Display(Name = "Thème")]
        public string ThemeName { get; set; }

        [StringLength(255)]
        [Display(Name = "Couverture")]
        public string Coverage { get; set; }

        [Display(Name = "Résolution Spatial")]
        public double SpatialResolution { get; set; }

        [StringLength(500)]
        [Display(Name = "Résumé")]
        public string Summary { get; set; }

        [StringLength(255)]
        public string UrlData { get; set; }

        [StringLength(255)]
        public string Keywords { get; set; }

        public int DataSize { get; set; }
    }
}