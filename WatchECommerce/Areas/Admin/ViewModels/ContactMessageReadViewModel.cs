using Watch.Core.Entities;

namespace WatchECommerce.Areas.Admin.ViewModels
{
	public class ContactMessageReadViewModel
	{
        public List<ContactMessage> ContactMessages { get; set; }
        public ContactMessage ContactMessage { get; set; }
        public bool IsAllReadMessage { get; set; }
    }
}
