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
using Microsoft.EntityFrameworkCore;
using Models;

namespace App.Admin.User
{
    public class AddRoleUser : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _dbContext;
        public AddRoleUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, MyBlogContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser user { get; set; }

        [BindProperty]
        [Display(Name = "Vai trò")]
        // Danh sách Role của user
        public string[] RoleNames { get; set; }

        // Danh sách role trong db
        public SelectList allRoles { get; set; }

        // các claim từ tất cả role của user
        public List<IdentityRoleClaim<string>> ClaimsInRole { get; set; }

        // các claim của user
        public List<IdentityUserClaim<string>> ClaimsOfUser { get; set; }

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
            await GetClaims(userId);

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

        async Task GetClaims(string userId)
        {
            var rolesOfUser = (from ur in _dbContext.UserRoles
                               join r in _dbContext.Roles on ur.RoleId equals r.Id
                               where ur.UserId == userId
                               select r);
            var claimsInRole = from r in rolesOfUser
                               join c in _dbContext.RoleClaims on r.Id equals c.RoleId
                               select c;
            ClaimsInRole = await claimsInRole.ToListAsync();

            var claimsOfUser = from uc in _dbContext.UserClaims
                               where uc.UserId == userId
                               select uc;
            ClaimsOfUser = await claimsOfUser.ToListAsync();
        }
    }
}
