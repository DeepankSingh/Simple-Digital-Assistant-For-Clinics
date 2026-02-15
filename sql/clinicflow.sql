-- Auth Service DB
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    Email NVARCHAR(320) NOT NULL,
    PasswordHash NVARCHAR(512) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy NVARCHAR(128) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedBy NVARCHAR(128) NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UX_Users_Tenant_Email ON Users(TenantId, Email) WHERE IsDeleted = 0;

CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UX_Roles_Tenant_Name ON Roles(TenantId, Name) WHERE IsDeleted = 0;

CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    TenantId NVARCHAR(64) NOT NULL,
    PRIMARY KEY(UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY(UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY(RoleId) REFERENCES Roles(Id)
);

CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    TenantId NVARCHAR(64) NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY(UserId) REFERENCES Users(Id)
);
CREATE INDEX IX_RefreshTokens_User_Expires ON RefreshTokens(UserId, ExpiresAt DESC);

-- Patient Service DB
CREATE TABLE Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Age INT NOT NULL,
    Gender NVARCHAR(20) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    MedicalHistoryNotes NVARCHAR(MAX) NULL,
    CreatedBy NVARCHAR(128) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UX_Patients_Tenant_Phone ON Patients(TenantId, Phone) WHERE IsDeleted = 0;
CREATE INDEX IX_Patients_Search ON Patients(TenantId, FullName, Phone) WHERE IsDeleted = 0;

-- Appointment Service DB
CREATE TABLE Appointments (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    DoctorUserId UNIQUEIDENTIFIER NOT NULL,
    StartAtUtc DATETIME2 NOT NULL,
    EndAtUtc DATETIME2 NOT NULL,
    Status INT NOT NULL,
    Notes NVARCHAR(500) NULL,
    CreatedBy NVARCHAR(128) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UX_Appointments_Doctor_Time
    ON Appointments(TenantId, DoctorUserId, StartAtUtc)
    WHERE IsDeleted = 0 AND Status <> 3;
CREATE INDEX IX_Appointments_Calendar ON Appointments(TenantId, DoctorUserId, StartAtUtc);

-- Prescription Service DB
CREATE TABLE Prescriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    AppointmentId UNIQUEIDENTIFIER NOT NULL,
    MedicinesJson NVARCHAR(MAX) NOT NULL,
    Dosage NVARCHAR(300) NOT NULL,
    Notes NVARCHAR(1000) NULL,
    CreatedDateUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(128) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE INDEX IX_Prescriptions_Tenant_Patient ON Prescriptions(TenantId, PatientId, CreatedDateUtc DESC);

-- Notification Service DB
CREATE TABLE ReminderLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TenantId NVARCHAR(64) NOT NULL,
    AppointmentId UNIQUEIDENTIFIER NOT NULL,
    Channel NVARCHAR(50) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    Message NVARCHAR(500) NULL,
    SentAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(128) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
CREATE INDEX IX_ReminderLogs_Tenant_SentAt ON ReminderLogs(TenantId, SentAtUtc DESC);
