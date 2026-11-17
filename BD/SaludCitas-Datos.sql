USE SaludCitas;
GO

SET IDENTITY_INSERT dbo.Especialidades ON;
INSERT INTO dbo.Especialidades (EspecialidadId, Nombre) VALUES (5, N'Cardiología');
INSERT INTO dbo.Especialidades (EspecialidadId, Nombre) VALUES (4, N'Dermatología');
INSERT INTO dbo.Especialidades (EspecialidadId, Nombre) VALUES (1, N'Medicina General');
INSERT INTO dbo.Especialidades (EspecialidadId, Nombre) VALUES (3, N'Odontología');
INSERT INTO dbo.Especialidades (EspecialidadId, Nombre) VALUES (2, N'Pediatría');
SET IDENTITY_INSERT dbo.Especialidades OFF;
GO

SET IDENTITY_INSERT dbo.Sedes ON;
INSERT INTO dbo.Sedes (SedeId, Nombre, Direccion) VALUES (1, N'Sede Centro', N'Carrera 50 # 52-10');
INSERT INTO dbo.Sedes (SedeId, Nombre, Direccion) VALUES (2, N'Sede Laureles', N'Circular 4 # 70-25');
INSERT INTO dbo.Sedes (SedeId, Nombre, Direccion) VALUES (3, N'Sede Poblado', N'Calle 10 # 35-40');
SET IDENTITY_INSERT dbo.Sedes OFF;
GO

SET IDENTITY_INSERT dbo.Usuarios ON;
INSERT INTO dbo.Usuarios (UsuarioId, Correo, ClaveHash, Rol) VALUES (1, N'admin@saludcitas.com', N'$2b$11$Il.qZ6eJThH4/7HFgqaI2OLfDjDntb.hAh6PCzHEpveWym7km.CIO', N'admin');
INSERT INTO dbo.Usuarios (UsuarioId, Correo, ClaveHash, Rol) VALUES (2, N'paciente@correo.com', N'$2b$11$3HUHkOf2K77e0hVyhqjDS.9c1xjOwuknci3nQ6f5Dl7Laq9ogUpyO', N'paciente');
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

SET IDENTITY_INSERT dbo.Pacientes ON;
INSERT INTO dbo.Pacientes (PacienteId, UsuarioId, Nombres, Apellidos, Documento, Telefono) VALUES (1, 2, N'María', N'López', N'1020304050', N'3001234567');
SET IDENTITY_INSERT dbo.Pacientes OFF;
GO

SET IDENTITY_INSERT dbo.Medicos ON;
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (1, 1, N'Carlos', N'Ramírez', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (2, 1, N'Laura', N'Gómez', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (3, 2, N'Andrea', N'Restrepo', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (4, 2, N'Julián', N'Cárdenas', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (5, 3, N'Marcela', N'Ospina', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (6, 4, N'Santiago', N'Vélez', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (7, 5, N'Patricia', N'Montoya', 1);
INSERT INTO dbo.Medicos (MedicoId, EspecialidadId, Nombres, Apellidos, Activo) VALUES (8, 5, N'Felipe', N'Arango', 0);
SET IDENTITY_INSERT dbo.Medicos OFF;
GO

INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (1, 1);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (1, 2);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (2, 3);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (3, 1);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (3, 3);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (4, 2);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (5, 1);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (6, 2);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (6, 3);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (7, 3);
INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (8, 1);
GO