-- Crear la base de datos SIGEBI
CREATE DATABASE SIGEBI;
GO

USE SIGEBI;
GO

-- Tabla de Roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE  -- Ejemplo: Usuario, Bibliotecario, Administrador
);

-- Tabla de Usuarios
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Tabla de Autores
CREATE TABLE Authors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(150) NOT NULL
);

-- Tabla de Categorías
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- Tabla de Libros
CREATE TABLE Books (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    AuthorId INT NOT NULL,
    CategoryId INT NOT NULL,
    ISBN NVARCHAR(20) NULL,
    PublishYear INT NULL,
    State NVARCHAR(50) NOT NULL DEFAULT 'Disponible', -- Disponible, Prestado, Reservado, Dañado, etc.
    Location NVARCHAR(100) NULL, -- Ubicación física
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AuthorId) REFERENCES Authors(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Tabla de Préstamos
CREATE TABLE Loans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    UserId INT NOT NULL,
    LoanDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL, -- Fecha límite de devolución
    ReturnDate DATETIME NULL, -- Fecha en que se devolvió
    Returned BIT DEFAULT 0,
    -- PenaltyId quitado para evitar referencia circular
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de Penalizaciones (referencia a Loans)
CREATE TABLE Penalties (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LoanId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Reason NVARCHAR(255) NOT NULL,
    Paid BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (LoanId) REFERENCES Loans(Id)
);

-- Tabla de Notificaciones
CREATE TABLE Notifications (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- Ejemplo: Email, Mensajería Interna
    Message NVARCHAR(500) NOT NULL,
    SentAt DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de Reportes Generados
CREATE TABLE Reports (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ReportType NVARCHAR(50) NOT NULL, -- Ejemplo: PDF, Excel
    Title NVARCHAR(150) NOT NULL,
    GeneratedBy INT NOT NULL, -- UserId
    GeneratedAt DATETIME DEFAULT GETDATE(),
    FilePath NVARCHAR(255) NOT NULL,
    FOREIGN KEY (GeneratedBy) REFERENCES Users(Id)
);

-- Historial de Préstamos
CREATE TABLE LoanHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LoanId INT NOT NULL,
    ChangedAt DATETIME NOT NULL,
    OldReturned BIT,
    NewReturned BIT,
    FOREIGN KEY (LoanId) REFERENCES Loans(Id)
);

-- Tabla para integración externa (por ejemplo, credenciales institucionales)
CREATE TABLE ExternalIntegrations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ExternalSystem NVARCHAR(100) NOT NULL,
    ExternalId NVARCHAR(100) NOT NULL,
    LinkedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Configuración avanzada del sistema (parámetros globales)
CREATE TABLE AppSettings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(255) NOT NULL,
    Description NVARCHAR(255) NULL
);

-- Auditoría de acciones administrativas
CREATE TABLE AuditLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Action NVARCHAR(100) NOT NULL,
    Details NVARCHAR(500) NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Reservas de libros (opcional, para apartar libros antes de préstamo)
CREATE TABLE Reservations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    UserId INT NOT NULL,
    ReservedAt DATETIME NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla para comentarios o reseñas de libros (opcional)
CREATE TABLE BookReviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    UserId INT NOT NULL,
    Review NVARCHAR(1000) NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Multiautor por libro (relación muchos a muchos)
CREATE TABLE BookAuthors (
    BookId INT NOT NULL,
    AuthorId INT NOT NULL,
    PRIMARY KEY (BookId, AuthorId),
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
);

-- Tabla para logs técnicos y errores
CREATE TABLE SystemLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Level NVARCHAR(20) NOT NULL, -- Info, Warning, Error
    Message NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);