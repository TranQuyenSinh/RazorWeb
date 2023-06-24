using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

namespace App.Admin.Role
{
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class RoleModel : IdentityRole
        {
            // thêm 1 danh sách các claim của role
            public string[] Claims { get; set; }
        }

        public List<RoleModel> roles { get; set; }

        public async Task OnGet()
        {
            var r = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
            roles = new List<RoleModel>();
            
            foreach (var role in r)
            {
                var claimOfRole = await _roleManager.GetClaimsAsync(role);
                var claimsString = claimOfRole.Select(c => $"{c.Type} = {c.Value}");
                var roleModel = new RoleModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Claims = claimsString.ToArray()
                };
                roles.Add(roleModel);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
