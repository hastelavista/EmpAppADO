
using DATA;
using DATA.Repo;
//using DATA.Repo;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllersWithViews();
//Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
});


builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<IEmpRepo, EmpRepo>();

var app = builder.Build();



app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Emp}/{action=List}/{id?}")
    .WithStaticAssets();


app.Run();
