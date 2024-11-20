using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CentralisationV0.Models.Entities;
using CentralisationdeDonnee.Models;

namespace CentralisationV0.Models.Entities
{
    public class IndexAndDetailsViewModel
    {
        public List<Collaboration> Collaborations { get; set; }
        public List<Data> DataList { get; set; }
        public List<ContactClient> ContactClients { get; set; }
        public List<ArcgisSolution> ArcgisSolutions { get; set; }
        public List<Client> Clients { get; set; }
        public List<CoordinateSystem> CoordinateSystems { get; set; }
        public List<DataBase> DatabaseList { get; set; }
        public List<Theme> Themes { get; set; }  // Ajouté pour les thèmes

        public IndexViewModel IndexViewModel { get; set; }
        public CollaborationDetailsViewModel CollaborationDetails { get; set; } = new CollaborationDetailsViewModel();

        public class CollaborationDetailsViewModel
        {
            public int Id { get; set; }
            public string Titre { get; set; }
            public string Description { get; set; }
            public string ClientName { get; set; }
            public string TypeCollaborationName { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string Status { get; set; }
            public int Duration { get; set; }
            
            public List<FileViewModel> Files { get; set; } = new List<FileViewModel>();
        }

        public class FileViewModel
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
        public class ClientAndArcGISViewModel
        {
           
            public List<ContactClient> ContactClients { get; set; }
            public List<ArcgisSolution> ArcgisSolutions { get; set; }
        }
    }

}