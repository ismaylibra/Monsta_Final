using System.ComponentModel.DataAnnotations;

namespace WatchECommerce.ViewModels
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        

        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmedPassword { get; set; }
        public IFormFile Image { get; set; }

    }
}
