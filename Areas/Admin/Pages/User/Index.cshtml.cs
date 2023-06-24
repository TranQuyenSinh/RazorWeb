using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Models;

namespace App.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string ReturnUrl { get; set; }

        public class UserAndRoles : AppUser
        {
            public string RoleNames { get; set; }
        }

        public List<UserAndRoles> users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public int TotalUser { get; set; }

        /* ================ Phân trang ================ */
        public const int ITEMS_PER_PAGE = 10;
        public int countPages { get; set; }
        [BindProperty(SupportsGet = true, Name = "p")]
        [FromQuery]
        public int currentPage { get; set; }

        public async Task OnGet()
        {
            ReturnUrl = UriHelper.GetEncodedPathAndQuery(this.Request);
            TotalUser = await _userManager.Users.CountAsync();
            countPages = (int)Math.Ceiling((double)TotalUser / ITEMS_PER_PAGE);

            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPages)
                currentPage = countPages;

            var qr = (from a in _userManager.Users
                      orderby a.UserName
                      select a)
                     .Skip((currentPage - 1) * ITEMS_PER_PAGE)
                     .Take(ITEMS_PER_PAGE)
                     .Select(user => new UserAndRoles()
                     {
                         Id = user.Id,
                         UserName = user.UserName
                     });

            users = await qr.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
