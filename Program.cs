using Microsoft.EntityFrameworkCore;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyBlogContext>(options => {
    string connStr = builder.Configuration.GetConnectionString("MyBlogContext");
    options.UseSqlServer(connStr);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

/*
    Phát sinh các trang CRUD tự động
    dotnet aspnet-codegenerator razorpage -m Models.Article -dc Models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
*/