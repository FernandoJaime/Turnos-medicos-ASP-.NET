using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnos.Models;

namespace Turnos.Controllers
{
    public class ProfesionaleController : Controller
    {
        private readonly GestionDeTurnosContext _context;

        public ProfesionaleController(GestionDeTurnosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filtro)
        {
            if (filtro == "Mas antiguos")
            {
                var profesionales1 = _context.Profesionales.OrderBy(p => p.FechaDeRecibimiento.Year);
                return View(await profesionales1.ToListAsync());
            }
            else if (filtro == "Mas recientes")
            {
                var profesionales2 = _context.Profesionales.OrderByDescending(p => p.FechaDeRecibimiento.Year);
                return View(await profesionales2.ToListAsync());
            }
            else
            {
                var profesionales = _context.Profesionales;
                return View(await profesionales.ToListAsync());
            }
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Profesionale profesional)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var nuevoProfesional = new Profesionale
                    {
                        CodigoProfesional = profesional.CodigoProfesional,
                        NombreProfesional = profesional.NombreProfesional,
                        ApellidoProfesional = profesional.ApellidoProfesional,
                        FechaDeRecibimiento = profesional.FechaDeRecibimiento
                    };

                    if (_context.Profesionales.Any(p => p.CodigoProfesional == profesional.CodigoProfesional))
                    {
                        ModelState.AddModelError("", "El código del profesional ya existe.");
                        return View(profesional);
                    }

                    if (profesional.FechaDeRecibimiento > DateTime.Now)
                    {
                        ModelState.AddModelError("", "La fecha de recibimiento no puede ser mayor a la fecha actual.");
                        return View(profesional);
                    }
                    else if (profesional.FechaDeRecibimiento < DateTime.Now.AddYears(-50))
                    {
                        ModelState.AddModelError("", "La fecha de recibimiento no puede ser de hace más de 50 años.");
                        return View(profesional);
                    }

                    _context.Add(nuevoProfesional);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar los datos. Intente nuevamente, y si el problema persiste, contacte al administrador del sistema.");
                }
            }
            return View(profesional);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtengo el profesional a eliminar.
            Profesionale profe = _context.Profesionales.Include(p => p.Turnos).FirstOrDefault(p => p.IdProfesional == id);

            if (profe == null)
            {
                return NotFound();
            }

            return View(profe);
        }

        [HttpPost] // Utilizo el método POST para eliminar el profesional.
        public async Task<IActionResult> Delete(int id)
        {
            var profe = await _context.Profesionales.Include(p => p.Turnos).FirstOrDefaultAsync(p => p.IdProfesional == id);

            try
            {
                // Si tiene turnos a cargo, no lo elimino.
                foreach (var item in _context.Turnos.Where(t => t.ProfesionalId == id))
                {
                    if (item.FechaTurno > DateTime.Now)
                    {
                        ModelState.AddModelError("", "Para que este profesional se pueda dar de baja, primero debe terminar con sus turnos programados.");
                        return View(profe);
                    }
                    _context.Turnos.Remove(item);
                }
                _context.Remove(profe);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar el profesional.");
                return View(profe);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profesionale profe = _context.Profesionales.Include(p => p.Turnos).FirstOrDefault(p => p.IdProfesional == id);

            if (profe == null)
            {
                return NotFound();
            }

            return View(profe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Profesionale profesional)
        {
            Profesionale profe = _context.Profesionales.Include(p => p.Turnos).FirstOrDefault(p => p.IdProfesional == id);

            if (ModelState.IsValid)
            {
                try
                {
                    profe.CodigoProfesional = profesional.CodigoProfesional;
                    profe.NombreProfesional = profesional.NombreProfesional;
                    profe.ApellidoProfesional = profesional.ApellidoProfesional;
                    profe.FechaDeRecibimiento = profesional.FechaDeRecibimiento;

                    _context.Update(profe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction("Index");
            }
            return View(profe);
        }
    }
}
