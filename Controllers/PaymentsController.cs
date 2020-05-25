using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using app.Hubs;
using app.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Braintree;
using Microsoft.AspNetCore.Http;

namespace app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
      private readonly ILogger<PaymentsController> _logger;
        private readonly UsersDBContext _dbContext;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly IBraintreeGateway _braintreeGateway;

        public PaymentsController(IHubContext<NotificationHub, INotificationHub> notificationHub, IConfiguration configuration,
           UsersDBContext dbContext, ILogger<PaymentsController> logger)
        {
            this._hubContext = notificationHub;
            this._configuration = configuration;
            this._dbContext = dbContext;
            this._logger = logger;
            this._braintreeGateway = new BraintreeGateway(Braintree.Environment.SANDBOX, 
                        _configuration["braintree:merchantid"],
                        _configuration["braintree:publickey"], 
                        _configuration["braintree:privatekey"]);
        }
        [HttpPost("token")]
        [Authorize]
        public JsonResult Get()
        {
            Random r = new Random();
            var x = r.Next(0, 1000000);
            string s = x.ToString("000000");

            return new JsonResult(new
            {
                token = _braintreeGateway.ClientToken.Generate(),
                amount = _configuration["braintree:amount"],
                clientId = s
            }) ;
        }

        [HttpPost("process")]
        [Authorize]
        public JsonResult Process(string nonce, string name)
        {
            var amount = _configuration["braintree:amount"];
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal( amount),
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = _braintreeGateway.Transaction.Sale(request);
            if (result.IsSuccess() || result.Transaction != null)
            {
                Transaction transaction = result.Target;

                //PostPaymentToDB(transaction, name);

                return new JsonResult(new {id = transaction.Id });
                //return Ok(transaction.Id);
            }

            return new JsonResult(new { id = "Payment Attempt Not Successfull" });

            //else
            //{
            //    return BadRequest("Payment Attempt Not Successfull");
            //}

        }

        private  void PostPaymentToDB(Transaction transaction, string providerName)
        {

            var paypalModel = transaction.PayPalDetails;
            PaypalTransaction paypalTransaction = new PaypalTransaction
            {
                Email = paypalModel.PayeeEmail,
                ParentTransactionID = transaction.AuthorizedTransactionId,
                Amount = transaction.Amount.GetValueOrDefault(),
                FeesAmount = transaction.ServiceFeeAmount.GetValueOrDefault(),
                FirstName = paypalModel.PayerFirstName,
                LastName = paypalModel.PayerLastName,
                CustomerId = transaction.BillingAddress.CustomerId,
                PaymenDate = transaction.CreatedAt.GetValueOrDefault(),
                TransactionToken = paypalModel.Token,
                UpdateDate = DateTime.UtcNow.ToShortDateString(),

            };
#if DEBUG
            return;
#else
    _dbContext.PaypalTransactions.Add(paypalTransaction);
            var name = HttpContext.User?.Identity?.Name;
            var subscriber = _dbContext.Subscribers.Where(c => c.UserName == name).FirstOrDefault();
            var provider = _dbContext.Providers.Where(c => c.UserName == providerName).FirstOrDefault();
            if (subscriber != null && provider != null)
            {
                Payment p = new Payment
                {
                    SubscriberId = subscriber.Id,
                    ProviderId = provider.Id,
                    AmountPaid = transaction.Amount.GetValueOrDefault(),
                    Date = DateTime.UtcNow,
                    Token = paypalModel.Token
                };
                _dbContext.Payments.Add(p);
            }
            await _dbContext.SaveChangesAsync();
#endif

        }

      
    }
}
