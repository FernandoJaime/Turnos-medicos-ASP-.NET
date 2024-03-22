using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Turnos.Models;
using Turnos.Models.ViewModels;

namespace Turnos.Controllers
{
    public class ClienteController : Controller
    {
        private readonly GestionDeTurnosContext _context;

        public ClienteController(GestionDeTurnosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? obrasocial)
        {
            // Si recibe una obra social como parametro, filtra los clientes por esa obra social.
            if (obrasocial.HasValue && obrasocial.Value != 0)
            {
                var clientes = _context.Clientes.Where(c => c.IdObraSocialCliente == obrasocial.Value);
                ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial");
                return View(await clientes.ToListAsync());
            }
            else // Si no recibe parametro, devuelve todos.
            {
                var clientes = _context.Clientes.Include(c => c.IdObraSocialClienteNavigation);
                ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial");
                return View(await clientes.ToListAsync());
            }
        }

        public IActionResult Add()
        {
            ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ClienteViewModel cliente)
        {
            if (ModelState.IsValid) // Si las validaciones del ViewModel pasan.
            {
                try
                {
                    var newCliente = new Cliente // Creo un nuevo cliente y completo datos. 
                    {
                        CodigoCliente = cliente.CodigoCliente,
                        NombreCliente = cliente.NombreCliente,
                        ApellidoCliente = cliente.ApellidoCliente,
                        EmailCliente = cliente.EmailCliente,
                        IdObraSocialCliente = cliente.IdObraSocialCliente
                    };

                    // Verificar si el código del cliente ya existe en la base de datos
                    if (_context.Clientes.Any(c => c.CodigoCliente == cliente.CodigoCliente))
                    {
                        ModelState.AddModelError("", "El código de cliente ya existe. Por favor, elija otro código.");
                        // Retornar la vista actual con el modelo que tiene los errores
                        ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", cliente.IdObraSocialCliente);
                        return View(cliente);
                    }

                    // Verificar si el email del cliente ya existe en la base de datos
                    if (_context.Clientes.Any(c => c.EmailCliente == cliente.EmailCliente))
                    {
                        ModelState.AddModelError("", "Este email ya esta registrado en el sistema. Por favor, elija otro email.");
                        // Retornar la vista actual con el modelo que tiene los errores
                        ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", cliente.IdObraSocialCliente);
                        return View(cliente);
                    }

                    _context.Add(newCliente);

                    await _context.SaveChangesAsync(); // Guardo en la DB.

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al intentar agregar el cliente.");
                }
            }
            ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", cliente.IdObraSocialCliente);
            return View(cliente);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtengo el cliente a eliminar.
            Cliente client = _context.Clientes.Include(o => o.IdObraSocialClienteNavigation).FirstOrDefault(O => O.IdClientes == id);

            if (client == null)
            {
                return NotFound();
            }
            ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", client.IdObraSocialCliente);
            return View(client);
        }

        [HttpPost] // Utilizo el método POST para eliminar el cliente.
        public async Task<IActionResult> Delete(int id)
        {
            Cliente cliente = _context.Clientes.Include(b => b.IdObraSocialClienteNavigation).FirstOrDefault(b => b.IdClientes == id);
            
            if (cliente == null)
            {
                return NotFound();
            }

            // Si tiene turnos, los elimino.
            foreach (var item in _context.Turnos.Where(t => t.ClienteId == id))
            {
                _context.Turnos.Remove(item);
            }
            
            _context.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cliente cliente = _context.Clientes.Include(o => o.IdObraSocialClienteNavigation).FirstOrDefault(O => O.IdClientes == id);

            if (cliente == null)
            {
                return NotFound();
            }

            ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", cliente.IdObraSocialCliente);
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel viewmodel)
        {
            Cliente cliente = _context.Clientes.Include(o => o.IdObraSocialClienteNavigation).FirstOrDefault(O => O.IdClientes == id); 

            if (ModelState.IsValid)
            {
                try
                {
                    cliente.CodigoCliente = viewmodel.CodigoCliente;
                    cliente.NombreCliente = viewmodel.NombreCliente;
                    cliente.ApellidoCliente = viewmodel.ApellidoCliente;
                    cliente.EmailCliente = viewmodel.EmailCliente;
                    cliente.IdObraSocialCliente = viewmodel.IdObraSocialCliente;

                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction("Index");
            }
            ViewData["ObrasID"] = new SelectList(_context.Obrasocials, "IdObraSocial", "NombreObraSocial", viewmodel.IdObraSocialCliente);
            return View(cliente);
        }
    }
}
