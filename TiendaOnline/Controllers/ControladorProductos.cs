using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Services;

namespace TiendaOnline.Controllers
{
    public class ControladorProductos : Controller
    {   
        private readonly AppDBContext context;
        //En la acción Index() tenemos que leer la lista de productos de la base de datos
        //y tenemos que pasar la lista a la vista. Para leer los datos de la base de datos
        //necesitamos de AppDBContext que ya agregamos al contenedor de servicios.
        //Para solicitarlo del contenedor de servicios necesitamos crear un constructor.
        public ControladorProductos(AppDBContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {   //Una vez creado el constructor ControladorProductos y el campo context
            //podemos usar context para leer los productos de la base de datos
            var productos = context.Productos.OrderByDescending(p => p.Id).ToList();
            //Ahora pasamos la lista de productos a la vista
            return View(productos);
        }
    }
}
