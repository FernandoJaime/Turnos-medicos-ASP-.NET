using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnos.Models;
using Turnos.Models.ViewModels;

namespace Turnos.Controllers
{
    public class ObrasocialController : Controller
    {
        private readonly GestionDeTurnosContext _context;

        public ObrasocialController(GestionDeTurnosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? filtro)
        {
            switch (filtro)
            {
                case "Menor precio":
                    var obrasociales1 = _context.Obrasocials.Include(o => o.Clientes).OrderBy(o => o.PrecioObraSocial);
                    return View(await obrasociales1.ToListAsync());
                case "Mayor precio":
                    var obrasociales2 = _context.Obrasocials.Include(o => o.Clientes).OrderByDescending(o => o.PrecioObraSocial);
                    return View(await obrasociales2.ToListAsync());
                case "A-Z":
                    var obrasociales3 = _context.Obrasocials.Include(o => o.Clientes).OrderBy(o => o.NombreObraSocial);
                    return View(await obrasociales3.ToListAsync());
                case "Z-A":
                    var obrasociales4 = _context.Obrasocials.Include(o => o.Clientes).OrderByDescending(o => o.NombreObraSocial);
                    return View(await obrasociales4.ToListAsync());
                case "Mayor cantidad de clientes":
                    var obrasociales5 = _context.Obrasocials.Include(o => o.Clientes).OrderByDescending(o => o.Clientes.Count);
                    return View(await obrasociales5.ToListAsync());
                case "Menor cantidad de clientes":
                    var obrasociales6 = _context.Obrasocials.Include(o => o.Clientes).OrderBy(o => o.Clientes.Count);
                    return View(await obrasociales6.ToListAsync());
                default:
                    var obrasociales = _context.Obrasocials.Include(o => o.Clientes);
                    return View(await obrasociales.ToListAsync());
            }
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Obrasocial obra)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newObrasocial = new Obrasocial
                    {
                        CodigoObraSocial = obra.CodigoObraSocial,
                        NombreObraSocial = obra.NombreObraSocial,
                        PrecioObraSocial = obra.PrecioObraSocial
                    };

                    // Verificar si el código ya existe en la base de datos
                    if (_context.Obrasocials.Any(o => o.CodigoObraSocial == obra.CodigoObraSocial))
                    {
                        ModelState.AddModelError("", "El código ya existe. Por favor, elija otro código.");
                        
                        // Retornar la vista actual.
                        return View(obra);
                    }

                    _context.Add(newObrasocial);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar los datos. Intente nuevamente, y si el problema persiste, contacte al administrador del sistema.");
                }
            }
            return View(obra);
        }

        public async Task<IActionResult> Clientes(int id)
        {
            try
            {
                Obrasocial obra = await _context.Obrasocials.Include(o => o.Clientes).FirstOrDefaultAsync(o => o.IdObraSocial == id);
                if (obra.Clientes == null)
                {
                    ModelState.AddModelError("", "La obra social seleccionada no tiene clientes registrados.");
                }
                else
                {
                    var listaClientes = obra.Clientes.ToList();
                    ViewData["Obra"] = obra;
                    return View(listaClientes);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar obtener los clientes de la obra social.");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Obrasocial obra = _context.Obrasocials.Include(o => o.Clientes).FirstOrDefault(O => O.IdObraSocial == id);

            if (obra == null)
            {
                return NotFound();
            }
            return View(obra);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var obra = await _context.Obrasocials.Include(o => o.Clientes).FirstOrDefaultAsync(o => o.IdObraSocial == id);
                if (obra.Clientes.Count > 0)
                {
                    ModelState.AddModelError("", "No se puede eliminar la obra social porque hay clientes registrados en ella.");
                    return View(obra);
                }
                _context.Remove(obra);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar la obra social.");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Obrasocial obra = _context.Obrasocials.Include(o => o.Clientes).FirstOrDefault(O => O.IdObraSocial == id);

            if (obra == null)
            {
                return NotFound();
            }

            return View(obra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Obrasocial social)
        {
            Obrasocial obra = _context.Obrasocials.Include(o => o.Clientes).FirstOrDefault(O => O.IdObraSocial == id);

            if (ModelState.IsValid)
            {
                try
                {
                    obra.CodigoObraSocial = social.CodigoObraSocial;
                    obra.NombreObraSocial = social.NombreObraSocial;
                    obra.PrecioObraSocial = social.PrecioObraSocial;

                    _context.Update(obra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction("Index");
            }
            return View(obra);
        }
    }
}
