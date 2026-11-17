# SaludCitas

Aplicación web de agendamiento de citas médicas.

- **Front:** Angular + Bootstrap
- **Back:** API REST en C# (.NET 10), arquitectura por capas, CQRS y Dapper
- **BD:** SQL Server

## Estructura

```
/Front   Aplicación Angular
/Back    API REST (solución SaludCitas.sln)
/BD      Scripts SQL
```

## Requisitos

- .NET SDK 9 o superior
- SQL Server (Express o Developer)
- Node.js 20 o superior y Angular CLI (`npm install -g @angular/cli`)

## 1. Base de datos

Ejecutar en este orden, en SQL Server Management Studio o Visual Studio:

1. `BD/SaludCitas.sql` (crea la base `SaludCitas` y las tablas)
2. `BD/SaludCitas-Datos.sql` (datos de prueba)

## 2. Backend

1. Abrir `Back/SaludCitas.sln` en Visual Studio.
2. Revisar la cadena de conexión en `Back/SaludCitas.Api/appsettings.json`
   (por defecto: `Server=localhost;Database=SaludCitas;Trusted_Connection=True;`).
   Si usa otra instancia, cambiar `localhost` (por ejemplo `localhost\SQLEXPRESS`).
3. Establecer `SaludCitas.Api` como proyecto de inicio y ejecutar (Ctrl + F5).
   La API queda en `https://localhost:7101`.

## 3. Frontend

```
cd Front
npm install
ng serve
```

Abrir `http://localhost:4200`.
Si la API usa otro puerto, cambiar `API_URL` en `Front/src/app/core/services/auth-service.ts`.

## Usuarios de prueba

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | admin@saludcitas.com | Admin123 |
| Paciente | paciente@correo.com | Paciente123 |

## Funcionalidades

- Registro e inicio de sesión (JWT, contraseñas cifradas)
- Paciente: ver horarios libres, agendar, ver sus citas y cancelar citas futuras
- Administrador: crear, editar y activar/desactivar médicos; crear, editar y eliminar especialidades y sedes
- Reglas validadas en el backend: sin citas duplicadas, horario de 8:00 a 17:00 en bloques de 30 minutos, solo cancelar citas futuras