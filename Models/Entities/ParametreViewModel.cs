using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class ParametreViewModel
    {
        public RegisterViewModel RegisterViewModel { get; set; }
        public List<UserWithRolesViewModel> UsersWithRoles { get; set; }
        public List<NotificationViewModel> Notifications { get; set; }
        public List<ApplicationUser> Users { get; set; } // Add this line
        public Dictionary<string, string> UserRoles { get; set; }
       
    }
}