// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace App.Admin.User
{
    public class AddRoleUser : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser user { get; set; }

        [BindProperty]
        [Display(Name = "Vai trò")]
        public string[] RoleNames { get; set; }

        public SelectList allRoles { get; set; }

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

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            /* ================ Nạp các role trong db ================ */
            List<string> roleNames = _roleManager.Roles.Select(role => role.Name).ToList();
            allRoles = new SelectList(roleNames);

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
            var oldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            // những role trong list role cũ mà không có trong list role mới => role cần xóa
            var deleteRoles = oldRoleNames.Where(role => !RoleNames.Contains(role));

            // những role có trong list role mới mà ko có trong list role cũ => role cần thêm
            var addRoles = RoleNames.Where(role => !oldRoleNames.Contains(role));

            /* ================ Nạp các role trong db ================ */
            List<string> roleNames = _roleManager.Roles.Select(role => role.Name).ToList();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                foreach (var error in resultDelete.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultAdd.Succeeded)
            {
                foreach (var error in resultAdd.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            StatusMessage = $"Cập nhật vai trò cho {user.UserName} thành công.";

            return LocalRedirect(returnUrl);
        }
    }
}
