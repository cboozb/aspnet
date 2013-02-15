using System.Collections.Generic;

namespace AttributeRoutingSample.Models
{
    public interface IContactRepository
    {
        IEnumerable<Contact> GetAllContacts();

        Contact GetContact(int id);

        Contact AddContact(Contact item);

        bool RemoveContact(int id);

        bool UpdateContact(int id, Contact item);
    }
}
