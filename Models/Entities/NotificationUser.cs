using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    [Table("NotificationUser")]
    public class NotificationUser
    {
        [Key]
        public int NotificationUserId { get; set; }
        public int IdNotification { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReceivedDate { get; set; }
        public Notification Notification { get; set; }
        public ApplicationUser User { get; set; }
    }
}