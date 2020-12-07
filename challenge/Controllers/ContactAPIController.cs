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
using System.Data.SqlClient;
using AttributeRouting.Web.Mvc;

namespace challenge.Controllers
{
    public class ContactAPIController : ApiController
    {
        private ContactDBConext db = new ContactDBConext();
        
        // GET api/ContactAPI
        public IEnumerable<Contact> GetContacts()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            
            return db.Contacts.AsEnumerable();
        }

        // GET api/ContactAPI/5
        public Contact GetContact(string id)
        {
            if (id.ToLower() == "insertfromexcel") {
                insertFromExcel();
                return new Contact();
            }
            Contact contact = db.Contacts.Find(id);
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
                db.Contacts.Add(contact);
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

            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Contacts.Remove(contact);

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

        public HttpResponseMessage insertFromExcel()
        {
            string excelConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + System.Web.HttpContext.Current.Server.MapPath("~/app_data/mock_data.xlsx") + "';Extended Properties=\"Excel 12.0 Xml;HDR=NO;\"";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(excelConStr))
            {
                conn.Open();
                System.Data.OleDb.OleDbCommand command = new System.Data.OleDb.OleDbCommand("Select * from [data$]", conn);
                System.Data.OleDb.OleDbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["ContactDBConext"].ConnectionString;  
                    SqlConnection con = new SqlConnection(strcon);
                    con.Open();

                    while (reader.Read())
                    {
                        Contact contact = new Contact();
                        contact.FirstName = reader[0].ToString();
                        contact.LastName = reader[1].ToString();
                        contact.Email = reader[2].ToString();
                        db.Contacts.Add(contact);
                        db.SaveChanges();
                    }
                    con.Close();
                    RemoveHeaderRow();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void RemoveHeaderRow()
        {
            Contact contact = db.Contacts.Where(s => s.FirstName == "first_name" && s.LastName == "last_name" && s.Email == "email").FirstOrDefault<Contact>();

            if (contact != null)
            {
                db.Contacts.Remove(contact);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Error: ", ex.Message);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}