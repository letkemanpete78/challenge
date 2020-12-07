using System;
using System.Data.Entity;

namespace challenge.Models
{
    public class Contact
    {
        //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

        private String id;
        public Contact() {
            id = Guid.NewGuid().ToString();
        }
        public String newID() { 
            return Guid.NewGuid().ToString();
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
        public DbSet<Contact> Contacts {get;set;}
    }
}