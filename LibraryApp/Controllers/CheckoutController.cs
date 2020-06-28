using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApp.Data.Services;
using LibraryApp.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}