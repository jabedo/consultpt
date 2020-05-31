using Bogus.DataSets;
using System;

namespace app.Models
{
    public class ProviderInfo : ClientUser
    {

        public String Bio { get; set; }
        public String Title { get; set; }
        public String PhoneNumber { get; set; }
        public String StateRegistered { get; set; }
        public String LicenseNumber { get; set; }
        public String PhotoName { get; set; }
        public String Address { get; set; }
        public string Avatar { get; set; }
        public string Words { get; set; }
        public long ProviderID { get; internal set; }
    }

}
