using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Contacts.API.Interfaces;
using Contacts.API.Models;
using Contacts.API.Helpers;
using Microsoft.Extensions.Configuration;

namespace Contacts.API.Services
{
    public class ContactService : IContactService
    {
        private readonly string _connectionString;

        public ContactService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<(IEnumerable<Contact> Contacts, int TotalRecords)> GetContactsAsync(ContactQueryParameters parameters)
        {
            var contacts = new List<Contact>();
            int totalRecords = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ManageContacts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "List");
                cmd.Parameters.AddWithValue("@PageNumber", parameters.PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", parameters.PageSize);
                cmd.Parameters.AddWithValue("@SortColumn", parameters.SortColumn ?? "FirstName");
                cmd.Parameters.AddWithValue("@SortOrder", parameters.SortOrder ?? "ASC");
                cmd.Parameters.AddWithValue("@SearchFirstName", (object)parameters.SearchFirstName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SearchLastName", (object)parameters.SearchLastName ?? DBNull.Value);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    // Read total records
                    if (await reader.ReadAsync())
                    {
                        totalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
                    }

                    // Move to the next result set (contacts)
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var contact = new Contact
                            {
                                ContactID = reader.GetInt32(reader.GetOrdinal("ContactID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                CreatedDateTime = reader.GetDateTime(reader.GetOrdinal("CreatedDateTime")),
                                LastModifiedDateTime = reader.GetDateTime(reader.GetOrdinal("LastModifiedDateTime"))
                            };
                            contacts.Add(contact);
                        }
                    }
                }
            }

            return (contacts, totalRecords);
        }

        public async Task<Contact> GetContactByIdAsync(int contactId)
        {
            Contact contact = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Contacts WHERE ContactID = @ContactID", conn))
            {
                cmd.Parameters.AddWithValue("@ContactID", contactId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        contact = new Contact
                        {
                            ContactID = reader.GetInt32(reader.GetOrdinal("ContactID")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                            CreatedDateTime = reader.GetDateTime(reader.GetOrdinal("CreatedDateTime")),
                            LastModifiedDateTime = reader.GetDateTime(reader.GetOrdinal("LastModifiedDateTime"))
                        };
                    }
                }
            }

            return contact;
        }

        public async Task AddContactAsync(Contact contact)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ManageContacts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "Add");
                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contact.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", (object)contact.Email ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ManageContacts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "Edit");
                cmd.Parameters.AddWithValue("@ContactID", contact.ContactID);
                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contact.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", (object)contact.Email ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteContactAsync(int contactId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ManageContacts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "Delete");
                cmd.Parameters.AddWithValue("@ContactID", contactId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
