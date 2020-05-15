using System;
using System.Collections.Generic;
using System.Linq;

namespace app.Models
{
    public enum UserType
   {
      provider = 1,
      subscriber
   }


    /*
    public class UserData
   {
      public static List<Provider> AllProviders
      {
         get
         {
            if(_allProviders.Count == 0)
            {
               using (var context = new UsersContext())
               {
                  _allProviders = context.Providers.ToList();
               }
            }
            return _allProviders;
         }
      }

      private static List<Provider> _allProviders = new List<Provider>();

      public static List<ProviderInfo> GetAllProviders()
      {
            return AllProviders.Select(c => new ProviderInfo
            {
               Bio = c.Bio,
               ConnectionId = null,
               InCall = false,
               LicenseNumber = c.IDLicenceNumber,
               StateRegistered = c.LicenseState,
               Title = c.Credentials,
               IsAvailable = false,
               Username = c.UserName,
               UserType = UserType.provider,
               PhoneNumber = c.PhoneNumber,
               Name = String.Format("{0} {1}", c.FirstName.TrimEnd(), c.LastName.TrimEnd()),
               Address = c.City,
               PhotoName = c.PhotoName
            }).ToList();
  
      }

      public static List<ProviderChat> GetChats(String providerUserName, String subscriberUserName)
      {
         using (UsersContext context = new UsersContext())
         {
            return context.Chats.Where(c => c.Provider.UserName == providerUserName && c.Subscriber.UserName == subscriberUserName).ToList();
         }
      }

      public static Subscriber GetTestSubscriber()
      {
         using (UsersContext context = new UsersContext())
         {
            return context.Subscribers.FirstOrDefault();
         }
      }

      public static Provider GetTestProvider()
      {
         using (UsersContext context = new UsersContext())
         {
            return context.Providers.FirstOrDefault();
         }
      }

   }

    */

}
