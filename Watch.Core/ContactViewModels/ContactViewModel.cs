using System.ComponentModel.DataAnnotations;

namespace Watch.Core.ContactViewModels
{
    public class ContactViewModel
    {

        public ContactMessageViewModel ContactMessage { get; set; } = new();

    }
        public class ContactMessageViewModel
        {
            public string Name { get; set; }
            public string? Subject { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Message { get; set; }
        }
    }

