using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class AjouterDataModel
    {
        public string Title { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }  // Peut être null
        public string Description { get; set; }
        public string Category { get; set; }
        public int ThemeId { get; set; }  // Thème sélectionné par ID
        public string IndustryName { get; set; }  // Industrie sélectionnée par ID
        public string Coverage { get; set; }  // Couverture géographique
        public int SpatialResolution { get; set; }
        public int CoordinateSystemId { get; set; }  // Système de coordonnées sélectionné par ID
        public int DataTypeId { get; set; }  // Type de donnée sélectionné par ID
        public string Summary { get; set; }
        public string FileName { get; set; }  // Nom du fichier uploadé

        // Modifier la propriété File pour accepter plusieurs fichiers
        public List<HttpPostedFileBase> Files { get; set; }  // Fichiers uploadés

        public string[] SelectedDataBases { get; set; }  // Bases de données sélectionnées par IDs
    }
}
