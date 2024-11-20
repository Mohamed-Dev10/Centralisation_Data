using CentralisationV0.App_Start.IdentityConfig;
using CentralisationV0.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CentralisationV0.Controllers
{
    public class NotificationController : Controller
    {

        private NotificationService _notificationService = new NotificationService();
        private ApplicationUserManager _userManager;
        public NotificationController(NotificationService notificationService, ApplicationUserManager userManager)
        {
            _notificationService = notificationService;
            UserManager = userManager;
        }


        public NotificationController()
        {
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpGet]
        public JsonResult GetLatestNotifications()
        {
            try
            {
                var notifications = _notificationService.GetAllNotificationsWithUsers();
                var result = notifications.Select(n => new
                {
                    CreatedDate = n.CreatedDate,
                    Message = n.Message,
                    UserNames = n.UserNames
                }).ToList();

                return Json(new { success = true, notifications = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error and return a failure response
                return Json(new { success = false, message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        //[HttpPost]
        //public ActionResult NotifyUser(string userId, string userName)
        //{
        //    // Validate input parameters
        //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
        //    {
        //        return Json(new { success = false, message = "Invalid parameters." });
        //    }

        //    try
        //    {
        //        // Construct the message
        //        string message = $"L'utilisateur {userName} a vous demander un nouveau mot de passe.";

        //        // Call the CreateNotification method
        //        _notificationService.CreateNotification(message, userId, userName);
        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Return the detailed error message to the client
        //        return Json(new { success = false, message = $"An error occurred while creating a notification: {ex.Message}" });
        //    }
        //}


        [HttpPost]
        public JsonResult NotifyUser(string userId, string userName)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "Invalid parameters." });
            }

            try
            {
                // Create the notification
                var notification = _notificationService.CreateNotification($"L'utilisateur {userName} a vous demander un nouveau mot de passe.", userId, userName);

                // Retrieve notification details
                var notificationDetails = _notificationService.GetNotificationDetails(notification);

                return Json(new { success = true, newNotification = notificationDetails });
            }
            catch (Exception ex)
            {
                // Return the detailed error message to the client
                return Json(new { success = false, message = $"An error occurred while creating a notification: {ex.Message}" });
            }
        }




    }
}
