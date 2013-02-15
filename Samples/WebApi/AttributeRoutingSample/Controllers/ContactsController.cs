using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AttributeRoutingSample.Models;

namespace AttributeRoutingSample.Controllers
{
    /// <summary>
    /// This sample controller holds contacts using a simple repository pattern.
    /// </summary>
    public class ContactController : ApiController
    {
        private static readonly IContactRepository _contacts = new ContactRepository();

        [GET("contacts")]
        public IEnumerable<Contact> Get()
        {
            return _contacts.GetAllContacts();
        }

        [GET("contact/{id}")]
        public Contact Get(int id)
        {
            Contact contact = _contacts.GetContact(id);
            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return contact;
        }

        [GET("contactOrDefault/{id=1}")]
        public Contact GetWithDefault(int id)
        {
            Contact contact = _contacts.GetContact(id);
            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return contact;
        }

        [GET("contactRange/{id:range(1,3)}")]
        public Contact GetWithRange(int id)
        {
            Contact contact = _contacts.GetContact(id);
            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return contact;
        }

        [POST("contact")]
        public Contact Post(Contact contact)
        {
            return _contacts.AddContact(contact);
        }

        [PUT("contact/{id}")]
        public void Put(int id, Contact newContact)
        {
            Contact contact = _contacts.GetContact(id);
            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _contacts.UpdateContact(id, newContact);
        }
    }
}
