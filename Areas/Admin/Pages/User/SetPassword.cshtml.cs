// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.User
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public SetPasswordModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser user { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("Không tìm thấy user");
            }
            user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tìm thấy user ID  = '{userId}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId, string returnUrl = null)
        {
            returnUrl ??= Url.Page("./Index");
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("Không tìm thấy user");
            }
            user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tìm thấy user ID  = '{userId}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _userManager.RemovePasswordAsync(user);
            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            StatusMessage = $"Đổi mật khẩu cho user {user.UserName} thành công.";

            return LocalRedirect(returnUrl);
        }
    }
}
