using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.Role
{
    /*
        Vì các trang trong Pages/Role có thể cần lặp lại đoạn code inject roleManager nhiều lần
        Nên tạo 1 class:PageModel inject các dịch vụ cần thiết, rồi cho các trang trong Pages/Role kế thừa class mới tạo này
    */
    [Authorize(Policy = "AllowManageRole")] 
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly AppDbContext _dbContext;

        [TempData]
        public string StatusMessage { get; set; }

        public RolePageModel(RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _roleManager = roleManager;
            _dbContext = context;
        }
    }
}