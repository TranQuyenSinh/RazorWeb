using System.Net;
using App.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;

var builder = WebApplication.CreateBuilder(args);
/* ================ Send Mail Service ================ */
// Lấy và đăng ký cấu hình send mail
builder.Services.AddOptions();
var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSetting>(mailsettings);
// Đăng ký dịch vụ IEmailSender với đối tượng cụ thể là SendMailService để Identity gửi email xác thực
builder.Services.AddSingleton<IEmailSender, SendMailService>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyBlogContext>(options =>
{
    string connStr = builder.Configuration.GetConnectionString("MyBlogContext");
    options.UseSqlServer(connStr);
});
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại, xác thực rồi mới cho login)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;  // Yêu cầu xác thực email trước khi login, xem trang register để rõ hơn

});
// Đăng ký Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyBlogContext>()
                .AddDefaultTokenProviders();

/* Sử dụng giao diện mặc định của Identity.UI*/
// builder.Services.AddDefaultIdentity<AppUser>()
//                 .AddEntityFrameworkStores<MyBlogContext>()
//                 .AddDefaultTokenProviders();

/* ================ Authorization option ================ */
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/login/";
    option.LogoutPath = "/logout/";
    option.AccessDeniedPath = "/not-allow.html";
});

/* ================ Thêm các Authentication provider ================ */
// Từ google, facebook
builder.Services.AddAuthentication()
                .AddGoogle(option =>
                {
                    var ggConfig = builder.Configuration.GetSection("Authentication:Google");
                    option.ClientId = ggConfig["ClientId"];
                    option.ClientSecret = ggConfig["ClientSecret"];
                    // http://localhost:5253/signin-google => Callbackpath mặc định
                    option.CallbackPath = "/dang-nhap-tu-google";
                })
                .AddFacebook(option =>
                {
                    var fbConfig = builder.Configuration.GetSection("Authentication:Facebook");
                    option.AppId = fbConfig["ClientId"];
                    option.AppSecret = fbConfig["ClientSecret"];
                    // còn error khi login
                    // phải xóa &scope=email trong url để fix
                });
/* ================ Tùy biến thông báo lỗi của Identity ================ */
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

/* ================ Thêm vào đoạn này để fix bug khi dùng external login ================ */
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.MapRazorPages();

app.Run();

/*
    Phát sinh các trang CRUD tự động
    dotnet aspnet-codegenerator razorpage -m Models.Article -dc Models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries

    Identity:
    dotnet add package Microsoft.AspNetCore.Identity
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet add package Microsoft.AspNetCore.Identity.UI
    dotnet add package Microsoft.AspNetCore.Authentication
    dotnet add package Microsoft.AspNetCore.Http.Abstractions
    dotnet add package Microsoft.AspNetCore.Authentication.Cookies
    dotnet add package Microsoft.AspNetCore.Authentication.Facebook
    dotnet add package Microsoft.AspNetCore.Authentication.Google
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
    dotnet add package Microsoft.AspNetCore.Authentication.oAuth
    dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect
    dotnet add package Microsoft.AspNetCore.Authentication.Twitter

    - Authentication: Xác thực danh tính => Login/Logout
    - Authorization: Xác thực quyền truy cập => Phân quyền
    - Quản lý user: Sign Up, User, Role ...

    /Identity/Account/Login
    /Identity/Account/Manage

    SignInManager<AppUser> s; => Kiểm tra có user login không, thông tin user login
    UserManager<AppUser> u;  => Quản lý App user

    Để ứng dụng gửi email xác thực tài khoản cần đăng ký dịch vụ IEmailSender vào container

    Để phát sinh code của những trang razor của Identity để tùy biến
    dotnet aspnet-codegenerator Identity -dc Models.MyBlogContext
    ** nhớ bỏ .AddDefaultTokenProviders() nếu ko sẽ bị lỗi

    // Thiết lập sử dụng những trang Identity custom thay vì mặc định 
    builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();

    2. Authorization: Xác thực quyền truy cập
    - Role-based authorization: Xác thực quyền theo vai trò (role)
    - 1 user có 1 hoặc nhiều role

    Các trang quản lý role: Index, Create, Edit, Delete
    => dotnet new page -n Index -o Areas/Admin/Pages/Role -p:n App.Admin.Role

    [Authorize](phải đăng nhập) => pagemodel, controller, action của controller

    

*/