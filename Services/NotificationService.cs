using CentralisationV0.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralisationV0.Services
{
    public class NotificationService
    {

        private CentralisationContext _context = new CentralisationContext();

        public NotificationService()
        {
        }

        public NotificationService(CentralisationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        //working methode
        //public void CreateNotification(string message, string userId, string userName)
        //{
        //    try
        //    {
        //        // Check if the message is null
        //        if (string.IsNullOrEmpty(message))
        //        {
        //            message = $"L'utilisateur {userName} a vous demander un nouveau mot de passe.";
        //        }

        //        // Initialize Notification
        //        var notification = new Notification
        //        {
        //            message = message,
        //            CreatedDate = DateTime.Now
        //            // Initialize other necessary properties
        //        };

        //        // Ensure the Notification object is valid
        //        if (notification == null)
        //        {
        //            throw new InvalidOperationException("Notification object is not initialized correctly.");
        //        }

        //        _context.Notifications.Add(notification);
        //        _context.SaveChanges();

        //        // Initialize NotificationUser
        //        var notificationUser = new NotificationUser
        //        {
        //            IdNotification = notification.IdNotification,
        //            UserId = userId,
        //            IsRead = false,
        //            ReceivedDate = DateTime.Now
        //        };

        //        // Ensure the NotificationUser object is valid
        //        if (notificationUser == null)
        //        {
        //            throw new InvalidOperationException("NotificationUser object is not initialized correctly.");
        //        }

        //        _context.NotificationUsers.Add(notificationUser);
        //        _context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception message and stack trace
        //        string errorMessage = $"An error occurred while creating a notification: {ex.Message} \n Stack Trace: {ex.StackTrace}";
        //        // Log the errorMessage to a file, database, or other logging mechanism
        //        throw new Exception(errorMessage, ex);
        //    }
        //}



        public Notification CreateNotification(string message, string userId, string userName)
        {
            // Check if the message is null
            if (string.IsNullOrEmpty(message))
            {
                message = $"L'utilisateur {userName} a vous demander un nouveau mot de passe.";
            }

            // Initialize Notification
            var notification = new Notification
            {
                message = message,
                CreatedDate = DateTime.Now
                // Initialize other necessary properties
            };

            // Add the notification to the database
            _context.Notifications.Add(notification);
            _context.SaveChanges();

            // Initialize NotificationUser
            var notificationUser = new NotificationUser
            {
                IdNotification = notification.IdNotification,
                UserId = userId,
                IsRead = false,
                ReceivedDate = DateTime.Now
            };

            // Add the notification user to the database
            _context.NotificationUsers.Add(notificationUser);
            _context.SaveChanges();

            return notification;
        }


        public IEnumerable<Notification> GetNotificationsByUserId(string userId)
        {
            // Retrieve notifications based on userId from the NotificationUser table
            return _context.NotificationUsers
                .Where(nu => nu.UserId == userId) // Filter by userId
                .Select(nu => nu.Notification) // Select the related Notification entity
                .OrderByDescending(n => n.CreatedDate) // Order by the notification's creation date
                .ToList(); // Convert to a list
        }

        public IEnumerable<NotificationViewModel> GetAllNotificationsWithUsers()
        {
            var notificationsWithUsers = _context.NotificationUsers
                .Select(nu => new
                {
                    Notification = nu.Notification,
                    User = nu.User,
                    CreatedDate = nu.Notification.CreatedDate,
                    Message = nu.Notification.message,
                    UserName = nu.User.UserName
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            // Map to ViewModel
            var result = notificationsWithUsers
       .GroupBy(x => new { x.Notification.IdNotification, x.CreatedDate, x.Message })
       .Select(g => new NotificationViewModel
       {
           CreatedDate = g.Key.CreatedDate.ToString("f"),
           Message = g.Key.Message,
           UserNames = g.Select(x => x.UserName).ToList() // Join all user names into a single string
       })
       .ToList();

            return result;
        }
        public object GetNotificationDetails(Notification notification)
        {
            // Retrieve notification details including user names
            return new
            {
                CreatedDate = notification.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                Message = notification.message,
                UserNames = _context.NotificationUsers
                    .Where(nu => nu.IdNotification == notification.IdNotification)
                    .Select(nu => nu.User.UserName)
                    .ToList()
            };
        }





    }
}