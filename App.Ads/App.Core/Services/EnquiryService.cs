using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using App.Core.Models;
using App.Core.Data;

namespace App.Core.Services
{
    public interface IEnquiryService
    {
        void SendEnquiry(SendEnquiryModel enquiry);
        void Save(Enquiry enquiry);
    }

    public class EnquiryService : IEnquiryService
    {
        private AdsDBEntities db;
        private readonly IEmailService emailService;

        public EnquiryService(IEmailService emailService)
        {
            db = new AdsDBEntities();
            this.emailService = emailService;
        }

        void IEnquiryService.Save(Enquiry enquiry)
        {
            enquiry.CreateDate = DateTime.Now;
            
            db.Enquiries.Add(enquiry);
            db.SaveChanges();
        }

        void IEnquiryService.SendEnquiry(SendEnquiryModel enquiry)
        {
            var viewData = new ViewDataDictionary { Model = enquiry };

            this.emailService.SendEmailWithTemplate(
                enquiry.Recipient.UserName,
                "Someone is interested in your listing - " + enquiry.ListingTitle,
                "Enquiry",
                viewData);
        }

    }
}
