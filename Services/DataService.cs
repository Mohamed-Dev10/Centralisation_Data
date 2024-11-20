using System.Collections.Generic;
using System.Linq;
using CentralisationV0.Models.Entities; // Assurez-vous que le namespace est correct
using CentralisationV0.Models; // Assurez-vous que le namespace est correct
using System.Data.Entity;
using CentralisationdeDonnee.Models; // Espace de noms où la classe Client est définie


namespace CentralisationV0.Services
{
    public interface IDataService
    {
        List<Data> GetData();
        List<Client> GetClients();

    }

    public class DataService : IDataService
    {
        public List<Data> GetData()
        {
            using (var context = new CentralisationContext())
            {
                // Utilisation de Include pour charger les thèmes associés
                return context.Datas.Include(d => d.Theme).ToList();
            }
        }

        public List<Client> GetClients()
        {
            using (var context = new CentralisationContext())
            {
                // Charger les contacts clients associés et les solutions ArcGIS avec Include
                return context.Clients
                              .Include(c => c.ContactClients) // Inclut les ContactClients associés
                              .Include(c => c.ArcgisSolutions) // Inclut les ArcgisSolutions associés
                              .ToList();
            }
        }
    }

}