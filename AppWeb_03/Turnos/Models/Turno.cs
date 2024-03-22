using System;
using System.Collections.Generic;

namespace Turnos.Models;

public partial class Turno
{
    public int IdTurno { get; set; }

    public string CodigoTurno { get; set; } = null!;

    public string RazonDeTurno { get; set; } = null!;

    public DateTime FechaTurno { get; set; }

    public int ProfesionalId { get; set; }

    public int ClienteId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Profesionale Profesional { get; set; } = null!;
}
