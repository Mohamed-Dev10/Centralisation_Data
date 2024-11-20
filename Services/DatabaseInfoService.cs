using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CentralisationV0.Models.Entities; // Assurez-vous que le namespace est correct
using CentralisationV0.Models; // Assurez-vous que le namespace est correct
using System.Data.Entity;


namespace CentralisationV0.Services
{

    public class DatabaseInfoService
    {
        private CentralisationContext db = new CentralisationContext();

        public List<DataBase> GetDatabases()
        {
            return db.DataBases.ToList();
        }
    }
}