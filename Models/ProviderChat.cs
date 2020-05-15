using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class ProviderChat
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public long Id { get; set; }
      [Column(TypeName = "datetime2")]
      public DateTime? Date { get; set; }
      public String Notes { get; set; }
      public long SubscriberId { get; set; }
      [ForeignKey("SubscriberId")]
      public Subscriber Subscriber { get; set; }
      public long ProviderId { get; set; }
      [ForeignKey("ProviderId")]
      public Provider Provider { get; set; }
      public String DateString { get; set; }

   }

}
