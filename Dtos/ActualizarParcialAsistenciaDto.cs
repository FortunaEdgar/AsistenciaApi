namespace AsistenciaApi.Dtos
{
    public class ActualizarParcialAsistenciaDto
    {
        public string? CodigoEmpleado { get; set; }
        public string? NombreEmpleado { get; set; }
        public string? Departamento { get; set; }
        public DateOnly? Fecha { get; set; }
        public TimeOnly? HoraEntrada { get; set; }
        public TimeOnly? HoraSalida { get; set; }
        public string? Estado { get; set; }
        public string? Observacin {  get; set; }
    }
}
