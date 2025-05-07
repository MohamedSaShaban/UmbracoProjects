using System.ComponentModel.DataAnnotations;

namespace TestProject1.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "الرسالة مطلوبة")]
        public string Message { get; set; } = null!;
    }
}
