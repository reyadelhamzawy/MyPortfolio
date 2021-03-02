using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork<Owner> _owner;
        private readonly IUnitOfWork<PortfolioItem> _portfolio;

        public HomeController(IUnitOfWork<Owner> owner, IUnitOfWork<PortfolioItem> portfolio)
        {
            _owner = owner;
            _portfolio = portfolio;
        }
        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                Owner = _owner.Entity.GetAll().First(),
                PortfolioItems = _portfolio.Entity.GetAll().ToList()
            };
            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult ContactMe(HomeViewModel contactMe)
        {
            var mail = new MailMessage();
            var loginInfo = new NetworkCredential("Online.Car.Rental.System36@gmail.com", "01017364909");
            mail.From = new MailAddress(contactMe.contact.Email);
            mail.To.Add(new MailAddress("reyadnady36@gmail.com"));
            mail.Subject = contactMe.contact.Subject;
            mail.IsBodyHtml = true;
            string body = "Name : " + contactMe.contact.Name + "<br>" + "E-Mail : " + contactMe.contact.Email + "<br>" + "Subject : " + contactMe.contact.Subject + "<br>" + "Message : <b>" + contactMe.contact.Message + "</b>";
            mail.Body = body;

            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(mail);
            return RedirectToAction("Index");
        }
    }
}