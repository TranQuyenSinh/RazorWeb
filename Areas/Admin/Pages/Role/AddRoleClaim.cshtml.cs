using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [Display(Name = "Tên đặc tính")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.", MinimumLength = 3)]
            public string ClaimType { get; set; }

            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.", MinimumLength = 3)]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null)
                return NotFound("Không tìm thấy vai trò");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null)
                return NotFound("Không tìm thấy vai trò");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var isExisted = (await _roleManager.GetClaimsAsync(Role)).Any(rc => rc.Type == Input.ClaimType && rc.Value == Input.ClaimValue);
            if (isExisted)
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã tồn tại");
                return Page();
            }

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(Role, newClaim);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return Page();
            }
            StatusMessage = "Thêm đặc tính mới thành công";
            return RedirectToPage("./Edit", new {roleId = roleId});
        }
    }
}
