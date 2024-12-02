using Microsoft.AspNetCore.Identity;

namespace UniqIo.Models
{
    public class AppUser : IdentityUser
    {
        public string Fullname {  get; set; }
        public string? ProfileImage {  get; set; }
        public string? Adress {  get; set; }
    }
}
