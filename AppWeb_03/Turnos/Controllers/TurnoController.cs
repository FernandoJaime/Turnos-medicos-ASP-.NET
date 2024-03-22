using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnos.Models;
using Turnos.Models.ViewModels;

namespace Turnos.Controllers
{
    public class TurnoController : Controller
    {
        private readonly GestionDeTurnosContext _context;

        public TurnoController(GestionDeTurnosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? filtro)
        {
            switch (filtro)
            {
                case "Turno mas nuevo":
                    var turnos1 = _context.Turnos.OrderByDescending(t => t.FechaTurno).Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation);
                    return View(await turnos1.ToListAsync());
                case "Turno mas antiguo":
                    var turnos2 = _context.Turnos.OrderBy(t => t.FechaTurno).Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation);
                    return View(await turnos2.ToListAsync());
                default:
                    var turnos = _context.Turnos.Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation);
                    return View(await turnos.ToListAsync());
            }
        }

        // Metodos para obtener los clientes que tienen turnos y los profesionales que tienen turnos.
        public async Task<IActionResult> Clientes()
        {
            try
            {
                var clientes = _context.Clientes.Include(c => c.Turnos);
                var listaClientes = clientes.Where(c => c.Turnos.Any(t => t.FechaTurno >= DateTime.Now)).ToList();

                if (listaClientes.Count == 0)
                {
                    ModelState.AddModelError("", "No hay clientes que tengan turnos pendientes registrados.");
                    return View(listaClientes); // Aquí deberías devolver la vista sin la lista de clientes ya que no hay clientes pendientes.
                }
                else
                {
                    return View(listaClientes.ToList());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar obtener los clientes de los turnos.");
                return View();
            }
        }

        public async Task<IActionResult> Profesionales()
        {
            try
            {
                var profesionales = _context.Profesionales.Include(p => p.Turnos);
                var listaProfesionales = profesionales.Where(c => c.Turnos.Any(t => t.FechaTurno >= DateTime.Now)).ToList();

                if (listaProfesionales.Count == 0)
                {
                    ModelState.AddModelError("", "No hay profesionales que tengan turnos pendientes registrados.");
                    return View(listaProfesionales);
                }
                else
                {
                    return View(listaProfesionales.ToList());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar obtener los profesionales de los turnos.");
                return View();
            }
        }

        public IActionResult Add()
        {
            ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional");
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TurnoViewModel turno)
        {
            if (ModelState.IsValid) // Si las validaciones del ViewModel pasan.
            {
                try
                {
                    var newTurno = new Turno // Creo un nuevo turno y completo datos. 
                    {
                        CodigoTurno = turno.CodigoTurno,
                        RazonDeTurno = turno.RazonDeTurno,
                        FechaTurno = turno.FechaTurno,
                        ProfesionalId = turno.ProfesionalId,
                        ClienteId = turno.ClienteId
                    };

                    // Verificar si el código del cliente ya existe en la base de datos
                    if (_context.Turnos.Any(c => c.CodigoTurno == turno.CodigoTurno))
                    {
                        ModelState.AddModelError("", "El código del turno ya existe. Por favor, elija otro código.");
                        // Retornar la vista actual con el modelo que tiene los errores
                        ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", turno.ProfesionalId);
                        ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", turno.ClienteId);
                        return View(turno);
                    }

                    // Verificar la fecha del turno
                    if (turno.FechaTurno < DateTime.Now)
                    {
                        ModelState.AddModelError("", "La fecha del turno no puede ser menor a la fecha actual.");
                        ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", turno.ProfesionalId);
                        ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", turno.ClienteId);
                        return View(turno);
                    }

                    _context.Add(newTurno);

                    await _context.SaveChangesAsync(); // Guardo en la DB.

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al intentar agregar el turno.");
                }
            }
            ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", turno.ProfesionalId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", turno.ClienteId);
            return View(turno);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtengo el turno a eliminar.
            Turno turno = _context.Turnos.Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation).FirstOrDefault(t => t.IdTurno == id);

            if (turno == null)
            {
                return NotFound();
            }
            ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", turno.ProfesionalId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", turno.ClienteId);
            return View(turno);
        }

        [HttpPost] // Utilizo el método POST para eliminar el cliente.
        public async Task<IActionResult> Delete(int id)
        {
            Turno turno = _context.Turnos.Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation).FirstOrDefault(t => t.IdTurno == id);

            if (turno == null)
            {
                return NotFound();
            }

            _context.Remove(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Turno turno = _context.Turnos.Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation).FirstOrDefault(t => t.IdTurno == id);

            if (turno == null)
            {
                return NotFound();
            }

            ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", turno.ProfesionalId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", turno.ClienteId);
            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TurnoViewModel viewmodel)
        {
            Turno turno = _context.Turnos.Include(c => c.Profesional).Include(c => c.Cliente).Include(c => c.Cliente.IdObraSocialClienteNavigation).FirstOrDefault(t => t.IdTurno == id);

            if (ModelState.IsValid)
            {
                try
                {
                    turno.CodigoTurno = viewmodel.CodigoTurno;
                    turno.RazonDeTurno = viewmodel.RazonDeTurno;
                    turno.FechaTurno = viewmodel.FechaTurno;
                    turno.ClienteId = viewmodel.ClienteId;
                    turno.ProfesionalId = viewmodel.ProfesionalId;

                    // Verificar la fecha del turno
                    if (viewmodel.FechaTurno < DateTime.Now)
                    {
                        ModelState.AddModelError("", "La fecha del turno no puede ser menor a la fecha actual.");
                        ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", viewmodel.ProfesionalId);
                        ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", viewmodel.ClienteId);
                        return View(viewmodel);
                    }

                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Ocurrió un error al intentar editar el turno.");
                }
                return RedirectToAction("Index");
            }
            ViewData["Profesionales"] = new SelectList(_context.Profesionales, "IdProfesional", "ApellidoProfesional", viewmodel.ProfesionalId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdClientes", "ApellidoCliente", viewmodel.ClienteId);
            return View(turno);
        }
    }
}
