using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class ClientEditViewModelcs
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientEmail { get; set; }
        public string ClientIndustry { get; set; }
        public string ClientType { get; set; }
        public List<int> ContactClients { get; set; }
        public List<int> ArcgisSolutions { get; set; }
    }
}