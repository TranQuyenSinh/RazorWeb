using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

namespace App.Admin.Role
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [Display(Name = "Tên vai trò")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.", MinimumLength = 3)]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole Role { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public async Task<IActionResult> OnGet(string roleId)
        {
            if (roleId == null) return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleId);
            Claims = await _dbContext.RoleClaims.Where(rc => rc.RoleId == roleId).ToListAsync();
            if (Role != null)
            {
                Input = new InputModel()
                {
                    Name = Role.Name
                };
                return Page();
            }

            return NotFound("Không tìm thấy role");
        }

        public async Task<IActionResult> OnPost(string roleId)
        {
            if (roleId == null) return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(roleId);
            Claims = await _dbContext.RoleClaims.Where(rc => rc.RoleId == roleId).ToListAsync();

            if (Role == null) return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Role.Name = Input.Name;

            var result = await _roleManager.UpdateAsync(Role);
            if (result.Succeeded)
            {
                StatusMessage = $"Cập nhật thành công";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
