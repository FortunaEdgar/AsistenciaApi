using AsistenciaApi.Dtos;
using AsistenciaApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var asistencias = new List<RegistroAsistencia>();
var siguienteId = 1;

var estadosPermitidos = new[] { "Presente", "Tardanza", "Ausente", "Justificado" };

var grupo = app.MapGroup("/api/asistencias");

grupo.MapGet("/", (string? codigoEmpleado, string? estado) =>
{
    var resultado = asistencias.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(codigoEmpleado))
        resultado = resultado.Where(a => a.CodigoEmpleado.Equals(codigoEmpleado, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrWhiteSpace(estado))
        resultado = resultado.Where(a => a.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));

    var lista = resultado.ToList();
    var hayFiltro = !string.IsNullOrWhiteSpace(codigoEmpleado) || !string.IsNullOrWhiteSpace(estado);

    if (hayFiltro && lista.Count == 0)
        return Results.NotFound(new { mensaje = "No hay registros con esos criterios." });

    return Results.Ok(lista);
});

grupo.MapGet("/{id:int}", (int id) =>
{
    var registro = asistencias.FirstOrDefault(a => a.Id == id);

    if (registro is null)
        return Results.NotFound(new { mensaje = $"No existe el registro {id}. " });

    return Results.Ok(registro);
});

grupo.MapPost("/", (CrearAsistenciaDto dto) =>
{
    var error = Validar(dto.CodigoEmpleado, dto.NombreEmpleado, dto.Departamento, dto.Fecha, dto.HoraEntrada, dto.HoraSalida, dto.Estado);
    if (error is not null) return Results.BadRequest(new { mensaje = error });
    var registro = new RegistroAsistencia
    {
        Id = siguienteId++,
        CodigoEmpleado = dto.CodigoEmpleado,
        NombreEmpleado = dto.NombreEmpleado,
        Departamento = dto.Departamento,
        Fecha = dto.Fecha,
        HoraEntrada = dto.HoraEntrada,
        HoraSalida = dto.HoraSalida,
        Estado = dto.Estado,
        Observacion = dto.Observacion
    };

    asistencias.Add(registro);

    return Results.Created($"/api/asistencias/{registro.Id}", registro);
});

grupo.MapPut("/{id:int}", (int id, ActualizarAsistenciaDto dto) =>
{
    var registro = asistencias.FirstOrDefault(a => a.Id == id);
    if (registro is null)
        return Results.NotFound(new { mensaje = $"No existe el registro {id}." });

    var error = Validar(dto.CodigoEmpleado, dto.NombreEmpleado, dto.Departamento, dto.Fecha, dto.HoraEntrada, dto.HoraSalida, dto.Estado, id);
    if (error is not null) return Results.BadRequest(new { mensaje = error });

    registro.CodigoEmpleado = dto.CodigoEmpleado;
    registro.NombreEmpleado = dto.NombreEmpleado;
    registro.Departamento = dto.Departamento;
    registro.Fecha = dto.Fecha;
    registro.HoraEntrada = dto.HoraEntrada;
    registro.HoraSalida = dto.HoraSalida;
    registro.Estado = dto.Estado;
    registro.Observacion = dto.Observacion;

    return Results.Ok(registro);
});

grupo.MapPatch("/{id:int}", (int id, ActualizarParcialAsistenciaDto dto) =>
{
    var registro = asistencias.FirstOrDefault(a => a.Id == id);
    if (registro is null)
        return Results.NotFound(new { mensaje = $"No existe el registro {id}." });

    var codigo = dto.CodigoEmpleado ?? registro.CodigoEmpleado;
    var nombre = dto.NombreEmpleado ?? registro.NombreEmpleado;
    var departamento = dto.Departamento ?? registro.Departamento;
    var fecha = dto.Fecha ?? registro.Fecha;
    var entrada = dto.HoraEntrada ?? registro.HoraEntrada;
    var salida = dto.HoraSalida ?? registro.HoraSalida;
    var estado = dto.Estado ?? registro.Estado;

    var error = Validar(codigo, nombre, departamento, fecha, entrada, salida, estado, id);
    if ((error is not null))
        return Results.BadRequest(new { mensaje = error });

    registro.CodigoEmpleado = codigo;
    registro.NombreEmpleado = nombre;
    registro.Departamento = departamento;
    registro.Fecha = fecha;
    registro.HoraEntrada = entrada;
    registro.HoraSalida = salida;
    registro.Estado = estado;
    registro.Observacion = dto.Observacin ?? registro.Observacion;

    return Results.Ok(registro);
});

grupo.MapDelete("/{id:int}", (int id) =>
{
    var registro = asistencias.FirstOrDefault(a => a.Id == id);
    if (registro is null)
        return Results.NotFound(new { mensaje = $"No existe el registtro {id}." });

    asistencias.Remove(registro);

    return Results.Ok(new { mensaje = $"Registro {id} eliminado correctamente." });
});

app.Run();

string? Validar(string? codigo, string? nombre, string? departamento, DateOnly fecha, TimeOnly? entrada, TimeOnly? salida, string? estado, int idActual = 0)
{
    if (string.IsNullOrWhiteSpace(codigo)) return "El codigo del empleado es requerido.";
    if (string.IsNullOrWhiteSpace(nombre)) return "El nombre del empleado es requerido.";
    if (string.IsNullOrWhiteSpace(departamento)) return "El departamento es requerido.";
    if (fecha == default) return "La fecha es requerida.";

    if (!estadosPermitidos.Contains(estado, StringComparer.OrdinalIgnoreCase)) return "El estado debe ser Presente, Tardanza, Ausente o Justificado.";

    if (entrada is not null && salida is not null && salida <= entrada) return "La hora de salida debe ser posterior a la de entrada.";

    var duplicado = asistencias.Any(a => a.Id != idActual && a.Fecha == fecha && a.CodigoEmpleado.Equals(codigo, StringComparison.OrdinalIgnoreCase));
    if (duplicado) return "Ese empleado ya tiene un registro en esa fecha.";

    return null;
}

