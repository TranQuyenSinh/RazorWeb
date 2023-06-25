using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

namespace App.Admin.Role
{
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
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

        public IdentityRoleClaim<string> Claim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimId)
        {
            if (claimId == null)
                return NotFound("Không tìm thấy claim");
            Claim = await _dbContext.RoleClaims.FindAsync(claimId);
            if (Claim == null)
                return NotFound("Không tìm thấy claim");

            Input = new InputModel()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimId)
        {
            if (claimId == null)
                return NotFound("Không tìm thấy claim");
            Claim = await _dbContext.RoleClaims.FindAsync(claimId);
            if (Claim == null)
                return NotFound("Không tìm thấy claim");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var isExisted = _dbContext.RoleClaims
                            .Where(rc => rc.RoleId == Claim.RoleId && rc.Id != claimId)
                            .Any(rc => rc.ClaimType == Input.ClaimType && rc.ClaimValue == Input.ClaimValue);
            if (isExisted)
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã tồn tại");
                return Page();
            }


            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            var result = await _dbContext.SaveChangesAsync();

            StatusMessage = "Chỉnh sửa đặc tính thành công";
            return RedirectToPage("./Edit", new { roleId = Claim.RoleId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimId)
        {
           if (claimId == null)
                return NotFound("Không tìm thấy claim");
            Claim = await _dbContext.RoleClaims.FindAsync(claimId);
            
            if (Claim == null)
                return NotFound("Không tìm thấy claim");

            var role = await _roleManager.FindByIdAsync(Claim.RoleId);
            await _roleManager.RemoveClaimAsync(role, new Claim(Claim.ClaimType, Claim.ClaimValue));

            StatusMessage = "Đã xóa đặc tính thành công";
            return RedirectToPage("./Edit", new {roleId = Claim.RoleId});
        }
    }
}
