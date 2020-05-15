using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Provider
   {
      public String Title { get; set; }
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public long Id { get; set; }
      [Required]
      [Display(Name = "Email")]
      public String UserName { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      //[AllowHtml]
      public string Bio { get; set; }
      public string Credentials { get; set; }
      public string Address { get; set; }
      public string City { get; set; }
      public string PhoneNumber { get; set; }
      public string State { get; set; }

      public string ZipCode { get; set; }
      public string IDTType { get; set; }
      public string IDLicenceNumber { get; set; }
      public string IDState { get; set; }

      [Column(TypeName = "datetime2")]
      public System.DateTime? DOB { get; set; }
      public string Sex { get; set; }

      public string LicenseState { get; set; }
      [Column(TypeName = "datetime2")]
      public System.DateTime? LicenseDate { get; set; }
      public String PhotoName_URL { get; set; }
   }

}
