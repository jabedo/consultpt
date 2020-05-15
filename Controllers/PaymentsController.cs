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

        //private static ConcurrentBag<Question> questions = new ConcurrentBag<Question> {
        //  new Question {
        //        Id = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
        //        CreatedBy = "terry.pratchett@lspace.com",
        //        Title = "Welcome",
        //        Body = "Welcome to the _mini Stack Overflow_ rip-off!\n" +
        //               "This application was built as an example on how **SignalR** and **Vue** can play together\n" +
        //               " - [Original article in the DotNetCurry magazine](https://www.dotnetcurry.com/aspnet-core/1480/aspnet-core-vuejs-signalr-app)\n" +
        //               " - [GitHub source of this app](https://github.com/DaniJG/so-signalr)",
        //        Answers = new List<Answer>{ new Answer { Body = "Sample answer", CreatedBy = "pierre.lemaitre@gmail.com" }}
        //    },
        //  new Question {
        //        Id = Guid.Parse("eb20d554-80be-429c-8418-5a72245bcaf3"),
        //        CreatedBy = "terry.pratchett@lspace.com",
        //        Title = "Welcome Back!",
        //        Body = "The second iteration enhanced the app adding authentication.\n" +
        //               "It includes examples for both **cookie** and **jwt** based authentication integrated with Vue and SignalR.\n" +
        //               "While this will be the subject of a new DotNetCurry article, you can Start by checking out these links:\n" +
        //               " - [SignalR authentication docs](https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2)\n" +
        //               " - [Example with multiple authentication schemes](https://github.com/aspnet/AspNetCore/tree/release/2.2/src/Security/samples/PathSchemeSelection)\n" +
        //               " - [JWT examples with ASP.NET Core](https://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api)\n" +
        //               " - [Securing APIs in ASP.NET Core](https://www.blinkingcaret.com/2018/07/18/secure-an-asp-net-core-web-api-using-cookies/)",
        //        Answers = new List<Answer>()
        //    },
        //};


        //[HttpPost()]
        //[Authorize]
        //public async Task<Question> AddQuestion([FromBody]Question question)
        //{
        //    question.Id = Guid.NewGuid();
        //    question.CreatedBy = this.User.Identity.Name;
        //    question.Deleted = false;
        //    question.Answers = new List<Answer>();
        //    questions.Add(question);
        //    await this._hubContext.Clients.All.UpdateUserList(new List<JsonUser>());
        //    return question;
        //}

        //[HttpPost("{id}/answer")]
        //[Authorize]
        //public async Task<ActionResult> AddAnswerAsync(Guid id, [FromBody]Answer answer)
        //{
        //    var question = questions.SingleOrDefault(t => t.Id == id && !t.Deleted);
        //    if (question == null) return NotFound();

        //    answer.Id = Guid.NewGuid();
        //    answer.QuestionId = id;
        //    answer.CreatedBy = this.User.Identity.Name;
        //    answer.Deleted = false;
        //    question.Answers.Add(answer);

        //    // Notify anyone connected to the group for this answer
        //    await this._hubContext.Clients.Group(id.ToString()).IncomingCall("");
        //    // Notify every client
        //    await this._hubContext.Clients.All.UpdateUserList(question.Id, question.Answers.Count);

        //    return new JsonResult(answer);
        //}

        //[HttpPatch("{id}/upvote")]
        //[Authorize]
        //public async Task<ActionResult> UpvoteQuestionAsync(Guid id)
        //{
        //    var question = questions.SingleOrDefault(t => t.Id == id && !t.Deleted);
        //    if (question == null) return NotFound();

        //    // Warning, this isnt really atomic!
        //    question.Score++;

        //    // Notify every client
        //    await this.hubContext.Clients.All.QuestionScoreChange(question.Id, question.Score);

        //    return new JsonResult(question);
        //}

        //[HttpPatch("{id}/downvote")]
        //[Authorize]
        //public async Task<ActionResult> DownvoteQuestionAsync(Guid id)
        //{
        //    var question = questions.SingleOrDefault(t => t.Id == id && !t.Deleted);
        //    if (question == null) return NotFound();

        //    // Warning, this isnt really atomic
        //    question.Score--;

        //    // Notify every client
        //    await this.hubContext.Clients.All.QuestionScoreChange(question.Id, question.Score);

        //    return new JsonResult(question);
        //}
    }
}
