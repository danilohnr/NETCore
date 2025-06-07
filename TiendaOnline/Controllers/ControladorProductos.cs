using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Models;
using TiendaOnline.Services;

namespace TiendaOnline.Controllers
{
    public class ControladorProductos : Controller
    {   
        private readonly AppDBContext context;
        private readonly IWebHostEnvironment environment;

        //En la acción Index() tenemos que leer la lista de productos de la base de datos
        //y tenemos que pasar la lista a la vista. Para leer los datos de la base de datos
        //necesitamos de AppDBContext que ya agregamos al contenedor de servicios.
        //Para solicitarlo del contenedor de servicios necesitamos crear un constructor.
        //Para guardar la imagen del producto se necesita un objeto de tipo IWebHostEnvironment
        public ControladorProductos(AppDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {   //Una vez creado el constructor ControladorProductos y el campo context
            //podemos usar context para leer los productos de la base de datos
            var productos = context.Productos.OrderByDescending(p => p.Id).ToList();
            //Ahora pasamos la lista de productos a la vista
            return View(productos);
        }
        //Vamos a agregar la acción para Crear producto nuevo
        public IActionResult Create()
        {
            return View();
        }
        //Vamos a agregar la acción para Crear producto cuando se presiona el botón Enviar
        //Vamos a incluir el parámetro del modelo ProductoDto
        //Para eso se debe indicar que se requiere el método HttpPost
        [HttpPost]
        public IActionResult Create(ProductoDto productoDto)
        {
            //Debemos validar el campo del archivo de imagen, ya que es opcional
            if (productoDto.ArchivoImagen == null)
            {
                ModelState.AddModelError("ArchivoImagen","El archivo de imagen es requerido.");
            }
            //Ahora veamos si tenemos errores de validación en ProductoDto
            //Si el estado del modelo no es válido, retornamos la vista Create con los datos del objeto productoDto enviado
            if (!ModelState.IsValid)
            {
                return View(productoDto);
            }
            //Para guardar el archivo de la imagen del producto, vamos a nombrarlo con la fecha de creación
            string nuevoNombreArchivo = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
            //Vamos a agregar la extensión del archivo al nombre del archivo
            nuevoNombreArchivo += Path.GetExtension(productoDto.ArchivoImagen!.FileName);
            //Vamos a obtener la la ruta (wwwroot/productos/) completa donde se guardará la imagen nueva
            string rutaCompletaImagen = environment.WebRootPath + "/productos/" + nuevoNombreArchivo;
            using (var flujoBytes = System.IO.File.Create(rutaCompletaImagen))
            {
                productoDto.ArchivoImagen.CopyTo(flujoBytes);
            }
            //Guardar el producto nuevo en la base de datos
            Producto producto = new Producto()
            {
                Nombre = productoDto.Nombre,
                Marca = productoDto.Marca,
                Categoria = productoDto.Categoria,
                Precio = productoDto.Precio,
                Descripcion = productoDto.Descripcion,  
                NombreArchivoImagen = nuevoNombreArchivo,
                CreadoEn = DateTime.Now,
            };
            context.Productos.Add(producto);    //Guardar producto en la tabla Productos
            context.SaveChanges();  //Hacer efectivo el cambio en la base de datos

            //Si todo está bien, redireccionamos al usuario a la lista de productos
            return RedirectToAction("Index", "ControladorProductos");
        }
        //Vamos a agregar la acción para Editar producto cuando se presiona el botón Editar
        //Se va a requerir el Id del producto, el Id se agregará a la URL
        public IActionResult Edit(int id)
        {
            //Primero se busca el producto a través de su Id
            //Si no se encuentra, el resultado devuelto es null
            var producto = context.Productos.Find(id);
            if (producto == null)
            {   //Si no se encuentra, redireccionamos al usuario a la lista de productos
                return RedirectToAction("Index","ControladorProductos");
            }
            //Si hallamos el producto, vamos a crear un objeto ProductoDto con los datos de producto
            var productoDto = new ProductoDto() //ProductoDto productoDto = new ProductoDto()
            {
                Nombre = producto.Nombre,
                Marca = producto.Marca,
                Categoria= producto.Categoria,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
            };
            //Debido a ProductoDto no contiene el Id, por lo que vamos a agregar el Id a un diccionario
            //llamado ViewData() para poder mostrarlo en la vista Edit
            ViewData["Id"] = producto.Id;
            ViewData["NombreArchivoImagen"] = producto.NombreArchivoImagen;
            ViewData["CreadoEn"] = producto.CreadoEn.ToString("dd/MM/yyyy");
            //Ahora debemos proveer la vista para este objeto
            return View(productoDto);
        }
    }
}
