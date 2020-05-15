using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class PaypalTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("city")]
        public string CustomerId { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("zipcode")]
        public string Zip { get; set; }
        [JsonProperty("username")]
        [Required]
        public string TransactionId { get; set; }
        public string StatusId { get; set; }
        public string UpdateDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal GrossTotal { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal FeesAmount { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal Amount { get; set; }
        public string TransactionToken { get; set; }
        public string ParentTransactionID { get; set; }
        [Column(TypeName ="datetime2")]
        public DateTime? PaymenDate { get; set; }

    }

}
