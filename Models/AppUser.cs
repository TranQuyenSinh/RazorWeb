using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class AppUser:IdentityUser
    {
        // thêm những trường bổ sung ngoài những trường mặc định
        [Column(TypeName= "nvarchar")]
        [StringLength(400)]
        public string? HomeAddress { get; set; }
    }
}