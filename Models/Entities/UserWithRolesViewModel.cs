using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class UserWithRolesViewModel
    {
        public RegisterViewModel RegisterViewModel { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public Dictionary<string, string> UserRoles { get; set; }

        public ApplicationUser User { get; set; } // Ensure you have the ApplicationUser type
        public List<string> Roles { get; set; }
    }
}