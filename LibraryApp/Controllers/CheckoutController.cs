using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using LibraryApp.Data.Services;
using LibraryApp.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IBraintreeService _braintreeService;

        public CheckoutController(IBookService bookService, IBraintreeService braintreeService)
        {
            _bookService = bookService;
            _braintreeService = braintreeService;
        }

        public IActionResult Purchase(Guid id)
        {

            var book = _bookService.GetById(id);
            if (book == null) return NotFound();

            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            var data = new BookPurchaseVM
            {
                Id = book.Id,
                Description = book.Description,
                Author = book.Author,
                Thumbnail = book.Thumbnail,
                Title = book.Title,
                Price = book.Price,
                Nonce = ""
            };

            return View(data);
        }

        public IActionResult Create(BookPurchaseVM model)
        {
            var gateway = _braintreeService.GetGateway();
            var book = _bookService.GetById(model.Id);

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(book.Price),
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }
        }

        public IActionResult BraintreePlans()
        {
            var gateway = _braintreeService.GetGateway();
            var plans = gateway.Plan.All();

            return View(plans);
        }

        public IActionResult SubscribeToPlan(string id)
        {
            var gateway = _braintreeService.GetGateway();

            var subscriptionRequest = new SubscriptionRequest()
            {
                // My notes: This a hard coded payment method token (credit card token) of
                // a sandbox test customer manually created Braintree's
                // dashboard: Vault > Customers > +New Customer.
                // In real world you might want to create a new customer (if not existing yet)
                // on the fly from a checkout flow and use the generated payment-method-token.
                PaymentMethodToken = "my-payment-method-token-value-xyz",
                PlanId = id,
            };

            Result<Subscription> result = gateway.Subscription.Create(subscriptionRequest);

            if (result.IsSuccess())
            {
                return View("Subscribed");
            }
            else
            {
                return View("NotSubscribed");
            }
        }
    }
}