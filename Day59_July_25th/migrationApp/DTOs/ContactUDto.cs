using System.ComponentModel.DataAnnotations;

namespace ChienVHShopOnline.Dtos
{
    public class ContactUDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Content { get; set; }
    }

    public class CreateContactUDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
