-- SaludCitasPrueba: script DDL
IF DB_ID(N'SaludCitasPrueba') IS NULL CREATE DATABASE SaludCitasPrueba;
GO
USE SaludCitasPrueba;
GO
CREATE TABLE [dbo].[Especialidades] (
    [EspecialidadId] INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]         NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Especialidades] PRIMARY KEY CLUSTERED ([EspecialidadId] ASC),
    CONSTRAINT [UQ_Especialidades_Nombre] UNIQUE NONCLUSTERED ([Nombre] ASC)
);


CREATE TABLE [dbo].[Sedes] (
    [SedeId]    INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]    NVARCHAR (100) NOT NULL,
    [Direccion] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Sedes] PRIMARY KEY CLUSTERED ([SedeId] ASC),
    CONSTRAINT [UQ_Sedes_Nombre] UNIQUE NONCLUSTERED ([Nombre] ASC)
);


CREATE TABLE [dbo].[Usuarios] (
    [UsuarioId] INT            IDENTITY (1, 1) NOT NULL,
    [Correo]    NVARCHAR (150) NOT NULL,
    [ClaveHash] NVARCHAR (200) NOT NULL,
    [Rol]       NVARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([UsuarioId] ASC),
    CONSTRAINT [CK_Usuarios_Rol] CHECK ([Rol]=N'admin' OR [Rol]=N'paciente'),
    CONSTRAINT [UQ_Usuarios_Correo] UNIQUE NONCLUSTERED ([Correo] ASC)
);


CREATE TABLE [dbo].[Pacientes] (
    [PacienteId] INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioId]  INT            NOT NULL,
    [Nombres]    NVARCHAR (100) NOT NULL,
    [Apellidos]  NVARCHAR (100) NOT NULL,
    [Documento]  NVARCHAR (20)  NOT NULL,
    [Telefono]   NVARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([PacienteId] ASC),
    CONSTRAINT [FK_Pacientes_Usuarios] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([UsuarioId]),
    CONSTRAINT [UQ_Pacientes_Documento] UNIQUE NONCLUSTERED ([Documento] ASC),
    CONSTRAINT [UQ_Pacientes_Usuario] UNIQUE NONCLUSTERED ([UsuarioId] ASC)
);


CREATE TABLE [dbo].[Medicos] (
    [MedicoId]       INT            IDENTITY (1, 1) NOT NULL,
    [EspecialidadId] INT            NOT NULL,
    [Nombres]        NVARCHAR (100) NOT NULL,
    [Apellidos]      NVARCHAR (100) NOT NULL,
    [Activo]         BIT            CONSTRAINT [DF_Medicos_Activo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Medicos] PRIMARY KEY CLUSTERED ([MedicoId] ASC),
    CONSTRAINT [FK_Medicos_Especialidades] FOREIGN KEY ([EspecialidadId]) REFERENCES [dbo].[Especialidades] ([EspecialidadId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Medicos_Especialidad]
    ON [dbo].[Medicos]([EspecialidadId] ASC);


CREATE TABLE [dbo].[MedicoSedes] (
    [MedicoId] INT NOT NULL,
    [SedeId]   INT NOT NULL,
    CONSTRAINT [PK_MedicoSedes] PRIMARY KEY CLUSTERED ([MedicoId] ASC, [SedeId] ASC),
    CONSTRAINT [FK_MedicoSedes_Medico] FOREIGN KEY ([MedicoId]) REFERENCES [dbo].[Medicos] ([MedicoId]),
    CONSTRAINT [FK_MedicoSedes_Sede] FOREIGN KEY ([SedeId]) REFERENCES [dbo].[Sedes] ([SedeId])
);


CREATE TABLE [dbo].[Citas] (
    [CitaId]     INT           IDENTITY (1, 1) NOT NULL,
    [PacienteId] INT           NOT NULL,
    [MedicoId]   INT           NOT NULL,
    [SedeId]     INT           NOT NULL,
    [FechaHora]  DATETIME2 (0) NOT NULL,
    [Estado]     NVARCHAR (20) CONSTRAINT [DF_Citas_Estado] DEFAULT (N'programada') NOT NULL,
    CONSTRAINT [PK_Citas] PRIMARY KEY CLUSTERED ([CitaId] ASC),
    CONSTRAINT [CK_Citas_Estado] CHECK ([Estado]=N'atendida' OR [Estado]=N'cancelada' OR [Estado]=N'programada'),
    CONSTRAINT [FK_Citas_Medicos] FOREIGN KEY ([MedicoId]) REFERENCES [dbo].[Medicos] ([MedicoId]),
    CONSTRAINT [FK_Citas_Pacientes] FOREIGN KEY ([PacienteId]) REFERENCES [dbo].[Pacientes] ([PacienteId]),
    CONSTRAINT [FK_Citas_Sedes] FOREIGN KEY ([SedeId]) REFERENCES [dbo].[Sedes] ([SedeId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Citas_Medico_FechaHora]
    ON [dbo].[Citas]([MedicoId] ASC, [FechaHora] ASC) WHERE ([Estado]=N'programada');


GO
CREATE NONCLUSTERED INDEX [IX_Citas_Paciente]
    ON [dbo].[Citas]([PacienteId] ASC, [FechaHora] ASC);


