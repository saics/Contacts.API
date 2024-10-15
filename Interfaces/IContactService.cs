using System.Collections.Generic;
using System.Threading.Tasks;
using Contacts.API.Models;
using Contacts.API.Helpers;

namespace Contacts.API.Interfaces
{
    public interface IContactService
    {
        Task<(IEnumerable<Contact> Contacts, int TotalRecords)> GetContactsAsync(ContactQueryParameters parameters);
        Task<Contact> GetContactByIdAsync(int contactId);
        Task AddContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task DeleteContactAsync(int contactId);
    }
}
