using System;
using System.Data.Entity;

namespace challenge.Models
{
    public class Contact
    {
        //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

        private String id;
        Contact() {
            id = Guid.NewGuid().ToString();
        }
        public String ID {
            get
            {
                return id;
            }
            set
            {
                if ((value == null) || (String.Compare(value,"") != 0 ))
                { 
                    id = Guid.NewGuid().ToString();
                }
                id = value;
            }
        }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
    }

    public class ContactDBConext : DbContext {
        public DbSet<Contact> Contaccts {get;set;}
    }
}