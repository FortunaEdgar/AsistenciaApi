# AsistenciaApi

API REST para la gestión de registros de asistencia de empleados, desarrollada con **ASP.NET Core 8 (Minimal APIs)** como proyecto de la asignatura *Lenguaje de Programación III*.

Permite registrar, consultar, filtrar, actualizar y eliminar la asistencia diaria de los empleados (hora de entrada, hora de salida, estado y observaciones). Los datos se almacenan **en memoria**, por lo que se reinician cada vez que la aplicación se detiene.

## Características

- CRUD completo sobre `/api/asistencias` (GET, POST, PUT, PATCH, DELETE).
- Filtros por código de empleado y estado.
- Validaciones de negocio:
  - Código, nombre, departamento y fecha son obligatorios.
  - El estado debe ser `Presente`, `Tardanza`, `Ausente` o `Justificado`.
  - La hora de salida debe ser posterior a la hora de entrada.
  - Un empleado no puede tener dos registros en la misma fecha.
- Uso de DTOs para separar los datos de entrada del modelo.

## Herramientas utilizadas

| Herramienta | Uso |
|---|---|
| C# / .NET 8 SDK | Lenguaje y plataforma de desarrollo |
| ASP.NET Core Minimal APIs | Definición de los endpoints HTTP |
| Visual Studio 2022 | IDE de desarrollo |
| Visual Studio Code + Thunder Client | Pruebas de los endpoints |
| Git y GitHub | Control de versiones y alojamiento del código |

## Estructura del proyecto

```
AsistenciaApi/
├── Dtos/
│   ├── CrearAsistenciaDto.cs              # Datos para crear un registro (POST)
│   ├── ActualizarAsistenciaDto.cs         # Datos para reemplazar un registro (PUT)
│   └── ActualizarParcialAsistenciaDto.cs  # Datos opcionales para actualizar en parte (PATCH)
├── Models/
│   └── RegistroAsistencia.cs              # Modelo del registro de asistencia
├── Properties/
│   └── launchSettings.json                # Perfiles y puertos de ejecución
├── Program.cs                             # Configuración, endpoints y validaciones
├── AsistenciaApi.csproj
└── AsistenciaApi.sln
```

## Modelo de datos

```json
{
  "id": 1,
  "codigoEmpleado": "EMP001",
  "nombreEmpleado": "Ana Perez",
  "departamento": "Contabilidad",
  "fecha": "2026-10-09",
  "horaEntrada": "08:00:00",
  "horaSalida": "17:00:00",
  "estado": "Presente",
  "observacion": "Llego a tiempo"
}
```

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/asistencias` | Lista todos los registros. Acepta los filtros opcionales `codigoEmpleado` y `estado`. |
| GET | `/api/asistencias/{id}` | Obtiene un registro por su id. |
| POST | `/api/asistencias` | Crea un nuevo registro. Responde `201 Created`. |
| PUT | `/api/asistencias/{id}` | Reemplaza todos los datos de un registro. |
| PATCH | `/api/asistencias/{id}` | Actualiza solo los campos enviados. |
| DELETE | `/api/asistencias/{id}` | Elimina un registro. |

Respuestas de error: `400 Bad Request` cuando falla una validación y `404 Not Found` cuando el registro no existe o un filtro no devuelve resultados. Ambas incluyen un objeto `{ "mensaje": "..." }`.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- Git
- Visual Studio Code con la extensión [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client) (para las pruebas)
- (Opcional) Visual Studio 2022

## Descarga y ejecución

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/FortunaEdgar/AsistenciaApi.git
   cd AsistenciaApi
   ```

2. Restaurar dependencias y compilar:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Ejecutar la API:

   ```bash
   dotnet run --launch-profile http
   ```

   La API queda disponible en `http://localhost:5115`.

   También se puede abrir `AsistenciaApi.sln` en Visual Studio y ejecutar con **F5**.

## Pruebas

Las pruebas de la API se realizan con la extensión **Thunder Client** de Visual Studio Code.

1. Instalar la extensión **Thunder Client** desde el Marketplace de VS Code.
2. Ejecutar la API (`dotnet run --launch-profile http`).
3. Abrir Thunder Client desde la barra lateral de VS Code y pulsar **New Request**.
4. Seleccionar el método HTTP, escribir la URL y, para POST, PUT y PATCH, ir a la pestaña **Body → JSON** y pegar el cuerpo de la petición.
5. Pulsar **Send** y revisar el código de estado y la respuesta.

### Peticiones de ejemplo

| # | Método | URL | Body (JSON) | Respuesta esperada |
|---|---|---|---|---|
| 1 | POST | `http://localhost:5115/api/asistencias` | Ver ejemplo abajo | `201 Created` |
| 2 | GET | `http://localhost:5115/api/asistencias` | — | `200 OK` |
| 3 | GET | `http://localhost:5115/api/asistencias/1` | — | `200 OK` |
| 4 | GET | `http://localhost:5115/api/asistencias?codigoEmpleado=EMP001&estado=Presente` | — | `200 OK` |
| 5 | PUT | `http://localhost:5115/api/asistencias/1` | Registro completo | `200 OK` |
| 6 | PATCH | `http://localhost:5115/api/asistencias/1` | `{ "estado": "Tardanza" }` | `200 OK` |
| 7 | DELETE | `http://localhost:5115/api/asistencias/1` | — | `200 OK` |

Body de ejemplo para POST y PUT:

```json
{
  "codigoEmpleado": "EMP001",
  "nombreEmpleado": "Ana Perez",
  "departamento": "Contabilidad",
  "fecha": "2026-10-09",
  "horaEntrada": "08:00:00",
  "horaSalida": "17:00:00",
  "estado": "Presente",
  "observacion": "Llego a tiempo"
}
```

### Casos de validación sugeridos

- Enviar un `estado` distinto a los permitidos → `400`.
- Enviar `horaSalida` anterior a `horaEntrada` → `400`.
- Crear dos registros del mismo empleado en la misma fecha → `400`.
- Consultar un id inexistente → `404`.

## Autor

Edgar Fortuna — Lenguaje de Programación III
