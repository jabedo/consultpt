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
        public static void Seed(UsersDBContext context)
        {
            if (context.Providers.Count() < 100)
            {
                //Populate database with fake data
                List<Provider> fakeList = new List<Provider>();
                List<ProviderImage> fakeImageList = new List<ProviderImage>();
                WebClient webClient = new WebClient();
                for (int i = 0; i < 100; i++)
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
                        UserName = Internet.Email(),
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
            }

            //Add subscriber test data
            if (context.Subscribers.Count() < 50)
            {
                var fakeSubscribers = new List<Subscriber>();
                for (int i = 0; i < 50; i++)
                {
                    Subscriber subscriber = new Subscriber
                    {
                        Address = Address.StreetAddress(),
                        PhoneNumber = Phone.Number(),
                        ZipCode = Address.ZipCode(),
                        Name = string.Format("{0} {1}", Name.First(), Name.Last()),
                        State = Address.UsState(),
                        UserName = Internet.Email(),
                    };
                    fakeSubscribers.Add(subscriber);
                }

                context.Subscribers.AddRange(fakeSubscribers);
                context.SaveChanges();
                  
                }

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
