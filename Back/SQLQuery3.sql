USE SaludCitasPrueba;
GO
SELECT 'Especialidades' AS Tabla, COUNT(*) AS Filas FROM dbo.Especialidades
UNION ALL SELECT 'Sedes', COUNT(*) FROM dbo.Sedes
UNION ALL SELECT 'Usuarios', COUNT(*) FROM dbo.Usuarios
UNION ALL SELECT 'Pacientes', COUNT(*) FROM dbo.Pacientes
UNION ALL SELECT 'Medicos', COUNT(*) FROM dbo.Medicos
UNION ALL SELECT 'MedicoSedes', COUNT(*) FROM dbo.MedicoSedes;

GO