/* ----- TEST UNITAIRE ----- */
using PanneauSolaire.Models.UnitTest;

TestHeureCoupure.main(args);
//TestConsommationMoyenne.main(args);
//TestDetailPrevision.main(args);
Main.main(args);

/* ------------------------- */

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session using activation
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Enable session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Specific}/{action=Index}/{id?}");

app.Run();
