using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace App.Admin.User
{
    public class EditUserClaimModel : PageModel
    {
        private readonly MyBlogContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public EditUserClaimModel(MyBlogContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser EditingUser { get; set; }
        public IdentityUserClaim<string> Claim { get; set; }


        public void OnGet() => NotFound("Không được truy cập");

        public async Task<IActionResult> OnGetAddUserClaimAsync(string userId)
        {
            EditingUser = await _userManager.FindByIdAsync(userId);
            if (EditingUser == null)
                return NotFound("Không tìm thấy user");
            return Page();
        }

        public async Task<IActionResult> OnGetEditUserClaimAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy đặc tính");
            Claim = await _dbContext.UserClaims.FindAsync(claimId);
            if (Claim == null) return NotFound("Không tìm thấy đặc tính");

            EditingUser = await _userManager.FindByIdAsync(Claim.UserId);
            Input = new InputModel()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAddUserClaimAsync(string userId)
        {
            EditingUser = await _userManager.FindByIdAsync(userId);
            if (EditingUser == null)
                return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid)
                return Page();

            var userClaim = _dbContext.UserClaims.Where(x => x.UserId == userId);

            var isExisted = userClaim.Any(x => x.ClaimType == Input.ClaimType && x.ClaimValue == Input.ClaimValue);

            if (isExisted)
            {
                ModelState.AddModelError(string.Empty, "Đặc tính đã tồn tại");
                return Page();
            }

            await _userManager.AddClaimAsync(EditingUser, new Claim(Input.ClaimType, Input.ClaimValue));

            StatusMessage = $"Đã thêm đặc tính {Input.ClaimType} = {Input.ClaimValue} cho user {EditingUser.UserName}";
            return RedirectToPage("./AddRoleUser", new { userId = userId });
        }

        public async Task<IActionResult> OnPostEditUserClaimAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy đặc tính");
            Claim = await _dbContext.UserClaims.FindAsync(claimId);
            if (Claim == null) return NotFound("Không tìm thấy đặc tính");

            EditingUser = await _userManager.FindByIdAsync(Claim.UserId);

            if (!ModelState.IsValid)
                return Page();

            var userClaims = _dbContext.UserClaims.Where(x => x.UserId == EditingUser.Id);

            var isExisted = userClaims.Any(x => x.ClaimType == Input.ClaimType && x.ClaimValue == Input.ClaimValue && x.Id != Claim.Id);

            if (isExisted)
            {
                ModelState.AddModelError(string.Empty, "Đặc tính đã tồn tại");
                return Page();
            }

            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;
            await _dbContext.SaveChangesAsync();

            StatusMessage = $"Đã chỉnh sửa đặc tính {Input.ClaimType} = {Input.ClaimValue} cho user {EditingUser.UserName}";
            return RedirectToPage("./AddRoleUser", new { userId = EditingUser.Id });
        }

        public async Task<IActionResult> OnPostDeleteUserClaimAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy đặc tính");
            Claim = await _dbContext.UserClaims.FindAsync(claimId);
            if (Claim == null) return NotFound("Không tìm thấy đặc tính");
            
            EditingUser = await _userManager.FindByIdAsync(Claim.UserId);

            await _userManager.RemoveClaimAsync(EditingUser, new Claim(Claim.ClaimType, Claim.ClaimValue));

            StatusMessage = $"Đã xóa đặc tính {Input.ClaimType} = {Input.ClaimValue} của user {EditingUser.UserName}";
            return RedirectToPage("./AddRoleUser", new { userId = EditingUser.Id });
        }

    }
}
