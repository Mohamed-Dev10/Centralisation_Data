using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class NotificationViewModel
    {

        public List<string> UserNames { get; set; }
        public string CreatedDate { get; set; }
        public string Message { get; set; }

    }
}