using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    // public class UsersContext : DbContext
    //{
    //   public UsersContext()
    //       : base("usersConnectionString")
    //   {
    //      //use to see initial data to db
    //      //commented out to use data migrations instead
    //      //Database.SetInitializer(new UserDBInitializer());
    //   }

    //     public DbSet<Provider> Providers { get; set; }
    //     public DbSet<Subscriber> Subscribers { get; set; }
    //     public DbSet<ProviderChat> Chats { get; set; }
    //     public DbSet<Payment> Payments { get; set; }
    //     public DbSet<PaypalTransaction> Transactions { get; set; }
    //     public DbSet<Order> Orders { get; set; }
    // }



    //public class UsersDatabaseInitializer : DropCreateDatabaseAlways<UsersContext>
    //{
    //   //protected override void Seed(DataAccess.Models.UsersContext context)
    //   //{

    //   //   context.Providers.AddOrUpdate(x => x.Id,
    //   //          new Provider()
    //   //          {
    //   //             Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.",
    //   //             LastName = "Quinn",
    //   //             FirstName = "Scott",
    //   //             IDState = "CO",
    //   //             IDLicenceNumber = "PTCO2015",
    //   //             Title = "Physical Therapist",
    //   //             PhoneNumber = "XXX-XXX-XXXX",
    //   //             UserName = "scottquinn@quinn.com",
    //   //             PhotoName = "scottquinn.png"
    //   //          },
    //   //       new Provider()
    //   //       {
    //   //          Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident",
    //   //          LastName = "Therapist",
    //   //          FirstName = "Some",
    //   //          IDState = "CO",
    //   //          IDLicenceNumber = "PTCO2017",
    //   //          Title = "Physical Therapist",
    //   //          PhoneNumber = "XXX-XXX-XXXX",
    //   //          UserName = "sometherapist@quinn.com",
    //   //          PhotoName = "sometherapist.png"
    //   //       },
    //   //       new Provider()
    //   //       {
    //   //          Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident",
    //   //          LastName = "Therapist",
    //   //          FirstName = "Another",
    //   //          IDState = "CO",
    //   //          IDLicenceNumber = "PTCOO1",
    //   //          Title = "Dr Physical Therapist",
    //   //          PhoneNumber = "XXX-XXX-XXXX",
    //   //          UserName = "anothertherapist@quinn.com",
    //   //          PhotoName = "anothertherapist.png"
    //   //       }
    //   //       );
    //   //   context.Subscribers.AddOrUpdate(c => c.Id,
    //   //           new Subscriber { UserName = "janeking@janeking.com", Name = "Jane King" },
    //   //             new Subscriber { UserName = "bob@bob.com", Name = "Bob Fuller", },
    //   //             new Subscriber { UserName = "lindaking@linda.com", Name = "Linda Kallahan" },
    //   //             new Subscriber { UserName = "janedoe@janedoe.com", Name = "Jane Doe" }
    //   //             );

    //   //   context.Payments.AddOrUpdate(c => c.Id,
    //   //new Payment() { Amount = 50, Date = DateTime.Now, ProviderId = 1, SubscriberId = 1 }
    //   //);


    //   //   //context.Chats.AddOrUpdate(c => c.Id,
    //   //   //         new ProviderChat { Date = DateTime.Now, ProviderId = 1, SubscriberId = 1, Notes = "High ankle sprain", DateString = "Today" },
    //   //   //           new ProviderChat { Date = DateTime.Now, ProviderId = 1, SubscriberId = 1, Notes = "Back sprain", DateString = "Today" },
    //   //   //           new ProviderChat { Date = DateTime.Now, ProviderId = 1, SubscriberId = 1, Notes = "Knee pain", DateString = "Today" },
    //   //   //           new ProviderChat { Date = DateTime.Now, ProviderId = 1, SubscriberId = 1, Notes = "Back Pain", DateString = "Today" },

    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-1), ProviderId = 1, SubscriberId = 1, Notes = "High ankle sprain", DateString = "Yesterday" },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-1), ProviderId = 1, SubscriberId = 1, Notes = "Back sprain", DateString = "Yesterday" },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-1), ProviderId = 1, SubscriberId = 1, Notes = "Knee pain", DateString = "Yesterday" },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-1), ProviderId = 1, SubscriberId = 1, Notes = "Back Pain", DateString = "Yesterday" },


    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-2), ProviderId = 1, SubscriberId = 1, Notes = "High ankle sprain", DateString = DateTime.Now.AddDays(-2).ToLongDateString() },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-2), ProviderId = 1, SubscriberId = 1, Notes = "Back sprain", DateString = DateTime.Now.AddDays(-2).ToLongDateString() },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-2), ProviderId = 1, SubscriberId = 1, Notes = "Knee pain", DateString = DateTime.Now.AddDays(-2).ToLongDateString() },
    //   //   //           new ProviderChat { Date = DateTime.Now.AddDays(-2), ProviderId = 1, SubscriberId = 1, Notes = "Back Pain", DateString = DateTime.Now.AddDays(-2).ToLongDateString() }

    //   //   //           );


    //      //base.Seed(context);
    //   //}

    // }
    public class ProviderCallee
   {
      [Display(Name ="Name")]
      public String Name { get; set; }
      public String ConnectionID { get; set; }
      public String PhotoName { get; set; }

      public Decimal Amount { get; set; }
   }

}
