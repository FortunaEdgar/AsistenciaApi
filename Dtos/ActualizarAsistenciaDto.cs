namespace AsistenciaApi.Dtos
{
    public class ActualizarAsistenciaDto
    {
        public string CodigoEmpleado { get; set; } = string.Empty;
        public string NombreEmpleado { get; set; } = string.Empty;
        public string Departamento {  get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public TimeOnly? HoraEntrada { get; set; }
        public TimeOnly? HoraSalida { get; set; }
        public string Estado {  get; set; } = string.Empty;
        public string? Observacion { get; set; }
    }
}
