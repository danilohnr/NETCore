﻿using Microsoft.AspNetCore.Mvc;
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
    }
}
