
using EmpAppADO;
using EmpAppADO.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<HttpServiceHelper>();
builder.Services.AddScoped<APICallService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();



app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EmpView}/{action=List}/{id?}")
    .WithStaticAssets();


app.Run();
