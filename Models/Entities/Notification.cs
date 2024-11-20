using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        public int IdNotification { get; set; }

        public string message { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<NotificationUser> NotificationUsers { get; set; }
    }
}