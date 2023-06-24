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

        public List<IdentityRole> roles { get; set; }

        public async Task OnGet()
        {
            roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
        }

        public void OnPost() => RedirectToPage();
    }
}
