using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CentralisationV0.Models.Entities;
using CentralisationdeDonnee.Models;

namespace CentralisationV0.Models.Entities.ViewModels
{
    public class DataAndDatabaseViewModel
    {
        public List<Data> DataList { get; set; }
        public List<DataBase> DatabaseList { get; set; }
        public List<CoordinateSystem> CoordinateSystems { get; set; }
        public List<DataTyp> DataTypes { get; set; }
        public List<Theme> Themes { get; set; }
        public List<Industry> Industries { get; set; }
    }

    public class DataDetailsViewModel
    {
        public int IdData { get; set; }
        public string Title { get; set; }
        public string AcquisitionDate { get; set; }
        public string PublicationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ThemeName { get; set; }
        public string Coverage { get; set; }
        public double SpatialResolution { get; set; }
        public string Summary { get; set; }
        public List<FileViewModel> Files { get; set; }
    }

    public class FileViewModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
