using System.ComponentModel.DataAnnotations;

namespace WatchECommerce.ViewModels
{
    public class ContactViewModel
    {

        public ContactMessageViewModel ContactMessage { get; set; } = new();

    }
        public class ContactMessageViewModel
        {
            public string Name { get; set; }
            public string? ImageUrl { get; set; }
            public string? Subject { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Message { get; set; }
        }
    }

