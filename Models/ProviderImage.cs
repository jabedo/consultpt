using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class ProviderImage
   {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    
        public long ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider Provider { get; set; }

        public byte[] ImageData { get; set; }

    }

}
