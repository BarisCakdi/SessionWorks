using System.ComponentModel.DataAnnotations;

namespace SessionWorks.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordRepeat { get; set; }
        public int RolId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? RedirectUrl { get; set; }
    }

    public class Roll
    {
        public string Id { get; set; }
        public string RollName { get; set; }
    }

    public class UserAndRoll
    {
        public List<User> Users { get; set; }
        public List<Roll> Rolls { get; set; }
        public int? SelectedRoll { get; set; }
    }
}
