using Microsoft.EntityFrameworkCore;
using TiendaOnline.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor de la WebApp.
builder.Services.AddControllersWithViews();
//El tipo de Contexto de BD es nuestra clase AppDBContext
//Y le vamos a agregar una función lamdba para la configuración
//Requiere como parámetro la variable options de tipo DBContextOptions
//Declarada en el método constructor de la clase
builder.Services.AddDbContext<AppDBContext>(options => 
{
    //Aquí agregamos la cadena de conexión que agregamos en el archivo appsettings.json
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //Luego se agrega como opción la cadena de conexión a AppDBContext
    options.UseSqlServer(connectionString);
    //Todo lo anterior es para la App use la cadena de conexión para conectarse con SQL Server
    //Y lo hicimos dentro del contenedor de Servicios, cualquier otra clase que necesitemos podrá usarla 
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
