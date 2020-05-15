using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Payment
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public long Id { get; set; }
      public long SubscriberId { get; set; }
      [ForeignKey("SubscriberId")]
      public Subscriber Subscriber { get; set; }
      public long ProviderId { get; set; }
      [ForeignKey("ProviderId")]
      public Provider Provider { get; set; }
        [Column(TypeName ="decimal(10,2)")]
      public Decimal AmountPaid { get; set; }
      [Column(TypeName = "datetime2")]
      public DateTime Date { get; set; }
      [Required]
      public String Token { get; set; }

   }

}
