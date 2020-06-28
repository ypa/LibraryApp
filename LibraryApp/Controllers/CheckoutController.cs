using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApp.Data.Services;
using LibraryApp.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace LibraryApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBookService _bookService;

        public CheckoutController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Purchase(Guid id)
        {

            var book = _bookService.GetById(id);
            ViewBag.PurchaseAmount = book.Price;
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost]
        public IActionResult Create(string stripeToken, Guid id)
        {
            var book = _bookService.GetById(id);

            var chargeOptions = new ChargeCreateOptions()
            {
                Amount = (long)(Convert.ToDouble(book.Price) * 100),
                Currency = "usd",
                Source = stripeToken,
                Metadata = new Dictionary<string, string>()
                {
                    { "BookId", book.Id.ToString() },
                    { "BookTitle", book.Title },
                    { "BookAuthor", book.Author }
                }
            };

            var service = new ChargeService();
            Charge charge = service.Create(chargeOptions);

            if (charge.Status == "succeeded")
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }

        }

        public IActionResult LoadAllPlans()
        {
            var service = new PlanService();
            var allPlans = service.List().ToList();

            return View(allPlans);
        }

        public IActionResult SubscribeToPlan(string id)
        {
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                // My notes: this is a hard-coded Stripe Customer ID from
                // which customer manually created in Stripe dashboard.
                // In real world, the customer will be created on the fly
                // from the checkout flow and use the returning Stripe Customer ID.
                Customer = "cus_HYFVKstgVPIJhs",
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = id
                    }
                }
            };

            var service = new SubscriptionService();

            Subscription subscription = service.Create(subscriptionOptions);

            if (subscription.Created != null)
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