using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleId)
        {
            if (roleId == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string roleId)
        {
            if (roleId == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Không tìm thấy role");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Xóa thành công";
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
