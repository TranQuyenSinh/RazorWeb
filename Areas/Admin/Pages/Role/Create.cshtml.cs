using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newRole = new IdentityRole(Input.Name);

            var createResult = await _roleManager.CreateAsync(newRole);
            if (createResult.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo vai trò mới: {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                createResult.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
