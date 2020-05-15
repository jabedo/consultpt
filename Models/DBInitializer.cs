using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faker;
using Faker.Extensions;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using System.Net;

namespace app.Models
{
    public static class DBInitializer
    {
        public static  void Seed(UsersDBContext context)
        {
            if (context.Providers != null && context.Providers.Count() > 10)
            {
                return;
            }

            //Populate database with fake data
            List<Provider> fakeList = new List<Provider>();
            List<ProviderImage> fakeImageList = new List<ProviderImage>();
            WebClient webClient = new WebClient();
            for (int i = 0; i < 200; i++)
            {
                var bogusProvider = new Faker<Provider>()
                               .RuleFor(u => u.PhotoName_URL, f => f.Internet.Avatar()).Generate();
                Provider provider = new Provider
                {
                    Address = Address.StreetAddress(),
                    City = Address.City(),
                    State = Address.UsState(),
                    Credentials = Name.Prefix(),
                    IDLicenceNumber = Identification.SocialSecurityNumber(),
                    IDState = Address.UsState(),
                    IDTType = Identification.MedicareBeneficiaryIdentifier(),
                    LicenseDate = Identification.DateOfBirth(),
                    LicenseState = Address.UsState(),
                    DOB = Identification.DateOfBirth(),
                    Title = Company.Name(),
                    FirstName = Name.First(),
                    LastName = Name.Last(),
                    UserName = Internet.UserName(),
                    Bio = SplitString(Lorem.Words(100)),
                    PhoneNumber = Phone.Number(),
                    PhotoName_URL = bogusProvider.PhotoName_URL,
                    ZipCode = Address.ZipCode(),
                };
                //fakeList.Add(provider);
                context.Providers.Add(provider);
                context.SaveChanges();
                 ProviderImage image = new ProviderImage
                {
                    ImageData = webClient.DownloadData(provider.PhotoName_URL),
                    Provider = provider
                };
                //fakeImageList.Add(image);
                context.Images.Add(image);
                context.SaveChanges();
               
            }


            //    //  This method will be called after migrating to the latest version.

            ////  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            ////  to avoid creating duplicate seed data. E.g.

            //context.Providers.AddRange( 
            //    // ALWAYS Use Add-Migration Initial -IgnoreChanges if DataMigration!!!
            //      new Provider
            //      {
            //          Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.",
            //          LastName = "Quinn",
            //          FirstName = "Scott",
            //          IDState = "CO",
            //          IDLicenceNumber = "PTCO2015",
            //          Title = "Physical Therapist",
            //          PhoneNumber = "XXX-XXX-XXXX",
            //          UserName = "scottquinn@quinn.com",
            //          PhotoName = "scottquinn.png"
            //      },
            //      new Provider
            //      {
            //          Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident",
            //          LastName = "Therapist",
            //          FirstName = "Some",
            //          IDState = "CO",
            //          IDLicenceNumber = "PTCO2017",
            //          Title = "Physical Therapist",
            //          PhoneNumber = "XXX-XXX-XXXX",
            //          UserName = "sometherapist@quinn.com",
            //          PhotoName = "sometherapist.png"
            //      },
            //      new Provider
            //      {
            //          Bio = "Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident",
            //          LastName = "Therapist",
            //          FirstName = "Another",
            //          IDState = "CO",
            //          IDLicenceNumber = "PTCOO1",
            //          Title = "Dr Physical Therapist",
            //          PhoneNumber = "XXX-XXX-XXXX",
            //          UserName = "anothertherapist@quinn.com",
            //          PhotoName = "anothertherapist.png"
            //      }
            //      );
            //    context.SaveChanges();
            //    context.Subscribers.AddRange(
            //              new Subscriber
            //              {
            //                  UserName = "janeking@janeking.com",
            //                  Name = "Jane King"
            //              },
            //              new Subscriber
            //              {
            //                  UserName = "bob@bob.com",
            //                  Name = "Bob Fuller",
            //              },
            //              new Subscriber
            //              {
            //                  UserName = "lindaking@linda.com",
            //                  Name = "Linda Kallahan"
            //              },
            //              new Subscriber
            //              {
            //                  UserName = "janedoe@janedoe.com",
            //                  Name = "Jane Doe"
            //              }
            //              );
            //    context.SaveChanges();


        }

        private static string SplitString(IEnumerable<string> enumerable)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var str in enumerable)
            {
                builder.Append(str + " " );
            }
            return builder.ToString();
        }
    }

}
