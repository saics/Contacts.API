namespace Contacts.API.Helpers
{
    public class ContactQueryParameters
    {
        private const int maxPageSize = 100;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string SortColumn { get; set; } = "FirstName";
        public string SortOrder { get; set; } = "ASC";
        public string? SearchFirstName { get; set; }
        public string? SearchLastName { get; set; }
    }
}
