USE [master]
GO
/****** Object:  Database [QhapacNan]    Script Date: 4/05/2022 02:56:49 ******/
CREATE DATABASE [QhapacNan]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'QhapacÑan', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\QhapacÑan.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'QhapacÑan_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\QhapacÑan_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [QhapacNan] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [QhapacNan].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [QhapacNan] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [QhapacNan] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [QhapacNan] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [QhapacNan] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [QhapacNan] SET ARITHABORT OFF 
GO
ALTER DATABASE [QhapacNan] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [QhapacNan] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [QhapacNan] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [QhapacNan] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [QhapacNan] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [QhapacNan] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [QhapacNan] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [QhapacNan] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [QhapacNan] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [QhapacNan] SET  DISABLE_BROKER 
GO
ALTER DATABASE [QhapacNan] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [QhapacNan] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [QhapacNan] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [QhapacNan] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [QhapacNan] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [QhapacNan] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [QhapacNan] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [QhapacNan] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [QhapacNan] SET  MULTI_USER 
GO
ALTER DATABASE [QhapacNan] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [QhapacNan] SET DB_CHAINING OFF 
GO
ALTER DATABASE [QhapacNan] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [QhapacNan] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [QhapacNan] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [QhapacNan] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [QhapacNan] SET QUERY_STORE = OFF
GO
USE [QhapacNan]
GO
/****** Object:  Table [dbo].[Hora]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hora](
	[Id] [int] NOT NULL,
	[Hora_Inicio] [int] NULL,
	[Hora_Fin] [int] NULL,
 CONSTRAINT [PK_Hora] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reserva]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reserva](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DNI_User] [nvarchar](8) NULL,
	[Fecha_Pre] [date] NULL,
	[Fecha_Post] [date] NULL,
	[Estado] [bit] NULL,
	[Precio] [decimal](18, 2) NULL,
	[Hora_Fecha] [datetime] NULL,
 CONSTRAINT [PK_Reserva] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReservaHora]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservaHora](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_Reserva] [int] NULL,
	[Id_Hora] [int] NULL,
	[Estado] [bit] NULL,
 CONSTRAINT [PK_ReservaHora] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReservaServicio]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservaServicio](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_Reserva] [int] NULL,
	[Id_Servicio] [int] NULL,
 CONSTRAINT [PK_ReservaServicio] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] NOT NULL,
	[Nombre] [nvarchar](5) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Servicios]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Servicios](
	[Id] [int] NOT NULL,
	[Nombre] [nvarchar](10) NULL,
	[Precio] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Servicios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 4/05/2022 02:56:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[DNI] [nvarchar](8) NOT NULL,
	[Ap_Paterno] [nvarchar](20) NULL,
	[Ap_Materno] [nvarchar](20) NULL,
	[Nombres] [nvarchar](40) NULL,
	[Nacimiento] [date] NULL,
	[Correo] [nvarchar](100) NULL,
	[Celular] [nvarchar](9) NULL,
	[RUC] [nvarchar](10) NULL,
	[Contrasenia] [nvarchar](100) NULL,
	[Recovery] [nvarchar](100) NULL,
	[Id_Rol] [int] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[DNI] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (1, 8, 9)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (2, 9, 10)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (3, 10, 11)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (4, 11, 12)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (5, 12, 13)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (6, 13, 14)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (7, 14, 15)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (8, 15, 16)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (9, 16, 17)
INSERT [dbo].[Hora] ([Id], [Hora_Inicio], [Hora_Fin]) VALUES (10, 17, 18)
GO
SET IDENTITY_INSERT [dbo].[Reserva] ON 

INSERT [dbo].[Reserva] ([Id], [DNI_User], [Fecha_Pre], [Fecha_Post], [Estado], [Precio], [Hora_Fecha]) VALUES (53, N'71245503', CAST(N'2022-05-03' AS Date), CAST(N'2022-05-27' AS Date), 0, CAST(40.00 AS Decimal(18, 2)), CAST(N'2022-05-03T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Reserva] OFF
GO
SET IDENTITY_INSERT [dbo].[ReservaHora] ON 

INSERT [dbo].[ReservaHora] ([Id], [Id_Reserva], [Id_Hora], [Estado]) VALUES (82, 53, 2, 0)
INSERT [dbo].[ReservaHora] ([Id], [Id_Reserva], [Id_Hora], [Estado]) VALUES (83, 53, 5, 0)
SET IDENTITY_INSERT [dbo].[ReservaHora] OFF
GO
SET IDENTITY_INSERT [dbo].[ReservaServicio] ON 

INSERT [dbo].[ReservaServicio] ([Id], [Id_Reserva], [Id_Servicio]) VALUES (70, 53, 1)
INSERT [dbo].[ReservaServicio] ([Id], [Id_Reserva], [Id_Servicio]) VALUES (71, 53, 2)
SET IDENTITY_INSERT [dbo].[ReservaServicio] OFF
GO
INSERT [dbo].[Roles] ([Id], [Nombre]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([Id], [Nombre]) VALUES (2, N'UserC')
GO
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (1, N'Futbol 7', CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (2, N'Voley', CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (3, N'Fulbito', CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (4, N'Basquet', CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (5, N'Tenis', CAST(10.00 AS Decimal(18, 2)))
INSERT [dbo].[Servicios] ([Id], [Nombre], [Precio]) VALUES (6, N'Fronton', CAST(10.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Usuario] ([DNI], [Ap_Paterno], [Ap_Materno], [Nombres], [Nacimiento], [Correo], [Celular], [RUC], [Contrasenia], [Recovery], [Id_Rol]) VALUES (N'70686996', N'Vasquez', N'Cabanillas', N'Miguel Anthony', CAST(N'1999-12-01' AS Date), N'miguel@gmail.com', N'976148585', NULL, N'kfWpY1A+vTXu8YLDsBJ95f4WnQDRn03lxOrVE7ziF5/OAxrYwxs9sdxKF8QWvQwYKDzOwqTsL6LNYxJlZANS4A==', NULL, 2)
INSERT [dbo].[Usuario] ([DNI], [Ap_Paterno], [Ap_Materno], [Nombres], [Nacimiento], [Correo], [Celular], [RUC], [Contrasenia], [Recovery], [Id_Rol]) VALUES (N'71245503', N'Toledo', N'Sanchez', N'Edwin', CAST(N'2000-11-05' AS Date), N'andybipoets@gmail.com', N'932182685', N'1071245503', N'kfWpY1A+vTXu8YLDsBJ95f4WnQDRn03lxOrVE7ziF5/OAxrYwxs9sdxKF8QWvQwYKDzOwqTsL6LNYxJlZANS4A==', NULL, 1)
INSERT [dbo].[Usuario] ([DNI], [Ap_Paterno], [Ap_Materno], [Nombres], [Nacimiento], [Correo], [Celular], [RUC], [Contrasenia], [Recovery], [Id_Rol]) VALUES (N'71245504', N'Toledo', N'Sanchez', N'Maicol', CAST(N'2002-08-07' AS Date), N'maico@gmail.com', N'952871425', NULL, N'kfWpY1A+vTXu8YLDsBJ95f4WnQDRn03lxOrVE7ziF5/OAxrYwxs9sdxKF8QWvQwYKDzOwqTsL6LNYxJlZANS4A==', NULL, 3)
GO
USE [master]
GO
ALTER DATABASE [QhapacNan] SET  READ_WRITE 
GO
