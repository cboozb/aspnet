using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AttributeRoutingSample.Models
{
    /// <summary>
    /// This implementation of <see cref="IContractRepository"/> uses a concurrent dictionary.
    /// </summary>
    public class ContactRepository : IContactRepository
    {
        private ConcurrentDictionary<int, Contact> _contacts = new ConcurrentDictionary<int, Contact>();
        private int currentId;

        public ContactRepository()
        {
            AddContact(new Contact { Email = "one@example.com", Name = "one", Phone = "111 111 1111" });
            AddContact(new Contact { Email = "two@example.com", Name = "two", Phone = "222 222 2222" });
            AddContact(new Contact { Email = "three@example.com", Name = "three", Phone = "333 333 3333" });
            AddContact(new Contact { Email = "four@example.com", Name = "four", Phone = "444 444 4444" });
        }

        public IEnumerable<Contact> GetAllContacts()
        {
            return _contacts.Select(kvp => kvp.Value).ToArray();
        }

        public Contact GetContact(int id)
        {
            Contact value;
            if (_contacts.TryGetValue(id, out value))
            {
                return value;
            }
            return null;
        }

        public Contact AddContact(Contact item)
        {
            item.Id = Interlocked.Increment(ref currentId);
            item.LastModified = DateTime.UtcNow;
            if (_contacts.TryAdd(item.Id, item))
            {
                return item;
            }
            throw new InvalidOperationException(string.Format("Item with ID {0} cannot be added", item.Id));
        }

        public bool RemoveContact(int id)
        {
            Contact value;
            return _contacts.TryRemove(id, out value);
        }

        public bool UpdateContact(int id, Contact item)
        {
            Contact oldValue;
            if (_contacts.TryGetValue(id, out oldValue))
            {
                item.Id = id;
                item.LastModified = DateTime.UtcNow;
                return _contacts.TryUpdate(id, item, oldValue);
            }
            return false;
        }
    }
}
