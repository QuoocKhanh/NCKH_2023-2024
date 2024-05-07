using NCKH.Repository;
using Microsoft.AspNetCore.SignalR;
using NCKH.Models;
using Microsoft.AspNetCore.DataProtection;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Istudent, StudentDetail>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


// Add SignalR
builder.Services.AddSignalR();

/*builder.Services.AddDataProtection()
    .PersistKeysToDbContext<MyDbContext>();*/

/*builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "YourCookieName";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});*/
var app = builder.Build();

using (var context = new MyDbContext())
{
    context.Database.Migrate();
}

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(name: "areas", pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");


// Map SignalR Hubs
app.MapHub<ChatHub>("/chatHub");

app.Run();

// Define the ChatHub
public class ChatHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessageToGroup(string groupName, string message, string userID)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", groupName, message, userID);
    }
}