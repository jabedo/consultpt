using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Subscriber
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public long Id { get; set; }
      [Required]
      public String UserName { get; set; }
      public String Name { get; set; }
      public String Address { get; set; }
      public String ZipCode { get; set; }
      public String PhoneNumber { get; set; }
      public String State { get; set; }

   }

}
