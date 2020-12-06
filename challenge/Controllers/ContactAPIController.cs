using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using challenge.Models;

namespace challenge.Controllers
{
    public class ContactAPIController : ApiController
    {
        private ContactDBConext db = new ContactDBConext();

        // GET api/ContactAPI
        public IEnumerable<Contact> GetContacts()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 
            return db.Contaccts.AsEnumerable();
        }

        // GET api/ContactAPI/5
        public Contact GetContact(string id)
        {
            Contact contact = db.Contaccts.Find(id);
            if (contact == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

            return contact;
        }

        // PUT api/ContactAPI/5
        public HttpResponseMessage PutContact(string id, Contact contact)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != contact.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(contact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ContactAPI
        public HttpResponseMessage PostContact(Contact contact)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Methods", "DELETE, PUT, GET, POST, OPTIONS");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Headers", "X-Requested-With, Accept, Access-Control-Allow-Origin, Content-Type");
            

            if (ModelState.IsValid)
            {
                db.Contaccts.Add(contact);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, contact);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = contact.ID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ContactAPI/5
        public HttpResponseMessage DeleteContact(string id)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Methods", "DELETE, PUT, GET, POST, OPTIONS");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Headers", "X-Requested-With, Accept, Access-Control-Allow-Origin, Content-Type");

            Contact contact = db.Contaccts.Find(id);
            if (contact == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Contaccts.Remove(contact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, contact);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}