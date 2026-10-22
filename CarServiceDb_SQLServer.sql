-- =========================================================================
-- APEXAUTO DYNAMICS - CAR SERVICE MANAGEMENT SYSTEM
-- Microsoft SQL Server Database Schema & Initial Data Script
-- Compatible with SQL Server 2019, 2022, and Azure SQL Database
-- =========================================================================

-- Optional: Create Database if not exists (uncomment if running on master)
-- IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CarServiceDb')
-- BEGIN
--     CREATE DATABASE CarServiceDb;
-- END
-- GO
-- USE CarServiceDb;
-- GO

-- 1. Table: Vehicles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vehicles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [LicensePlate] NVARCHAR(20) NOT NULL,
        [Make] NVARCHAR(50) NOT NULL,
        [Model] NVARCHAR(50) NOT NULL,
        [Year] INT NOT NULL,
        [Color] NVARCHAR(30) NOT NULL CONSTRAINT [DF_Vehicles_Color] DEFAULT 'Black',
        [Vin] NVARCHAR(30) NULL,
        [CurrentMileage] INT NOT NULL CONSTRAINT [DF_Vehicles_Mileage] DEFAULT 0,
        [FuelType] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Vehicles_Fuel] DEFAULT 'Gasoline',
        [Transmission] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Vehicles_Trans] DEFAULT 'Automatic',
        [OwnerName] NVARCHAR(100) NOT NULL,
        [OwnerPhone] NVARCHAR(25) NOT NULL,
        [OwnerEmail] NVARCHAR(100) NULL,
        [OwnerAddress] NVARCHAR(200) NULL,
        [RegisteredAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Vehicles_RegisteredAt] DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Vehicles_LicensePlate] ON [dbo].[Vehicles] ([LicensePlate] ASC);
END
GO

-- 2. Table: Mechanics
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mechanics]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Mechanics] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Specialization] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(25) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [AssignedBay] NVARCHAR(50) NOT NULL,
        [IsAvailable] BIT NOT NULL CONSTRAINT [DF_Mechanics_Available] DEFAULT 1,
        [ExperienceYears] INT NOT NULL CONSTRAINT [DF_Mechanics_Exp] DEFAULT 3,
        CONSTRAINT [PK_Mechanics] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 3. Table: ServicePackages
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServicePackages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ServicePackages] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(250) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [BaseLaborPrice] DECIMAL(18,2) NOT NULL,
        [EstimatedHours] DECIMAL(18,2) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_ServicePackages_Active] DEFAULT 1,
        [IconClass] NVARCHAR(50) NOT NULL CONSTRAINT [DF_ServicePackages_Icon] DEFAULT 'bi-tools',
        CONSTRAINT [PK_ServicePackages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 4. Table: ServiceAppointments
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceAppointments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ServiceAppointments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [AppointmentCode] NVARCHAR(30) NOT NULL,
        [VehicleId] INT NOT NULL,
        [ServicePackageId] INT NULL,
        [ServiceTitle] NVARCHAR(150) NOT NULL,
        [ScheduledDate] DATETIME2(7) NOT NULL,
        [TimeSlot] NVARCHAR(50) NOT NULL,
        [MechanicId] INT NULL,
        [Bay] NVARCHAR(50) NOT NULL,
        [CustomerNotes] NVARCHAR(500) NULL,
        [EstimatedCost] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Appointments_Cost] DEFAULT 0.00,
        [Status] INT NOT NULL CONSTRAINT [DF_Appointments_Status] DEFAULT 0,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Appointments_CreatedAt] DEFAULT SYSUTCDATETIME(),
        [LinkedRepairJobId] INT NULL,
        CONSTRAINT [PK_ServiceAppointments] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ServiceAppointments_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ServiceAppointments_Packages] FOREIGN KEY ([ServicePackageId]) REFERENCES [dbo].[ServicePackages]([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_ServiceAppointments_Mechanics] FOREIGN KEY ([MechanicId]) REFERENCES [dbo].[Mechanics]([Id]) ON DELETE SET NULL
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_ServiceAppointments_Code] ON [dbo].[ServiceAppointments] ([AppointmentCode] ASC);
END
GO

-- 5. Table: RepairJobCards
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RepairJobCards]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RepairJobCards] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [JobCardNumber] NVARCHAR(30) NOT NULL,
        [VehicleId] INT NOT NULL,
        [ServiceAppointmentId] INT NULL,
        [LeadMechanicId] INT NULL,
        [Bay] NVARCHAR(50) NOT NULL,
        [OdometerIn] INT NOT NULL,
        [DiagnosticNotes] NVARCHAR(1000) NULL,
        [WorkPerformedSummary] NVARCHAR(1000) NULL,
        [Status] INT NOT NULL CONSTRAINT [DF_RepairJobs_Status] DEFAULT 0,
        [StartedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_RepairJobs_StartedAt] DEFAULT SYSUTCDATETIME(),
        [EstimatedCompletionAt] DATETIME2(7) NULL,
        [CompletedAt] DATETIME2(7) NULL,
        [CheckEngineOil] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Oil] DEFAULT 1,
        [CheckBrakeSystem] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Brake] DEFAULT 1,
        [CheckBatteryHealth] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Bat] DEFAULT 1,
        [CheckTirePressureTread] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Tire] DEFAULT 1,
        [CheckCoolantRadiator] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Cool] DEFAULT 1,
        [CheckSuspensionSteering] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Susp] DEFAULT 1,
        [CheckLightsElectronics] BIT NOT NULL CONSTRAINT [DF_RepairJobs_Light] DEFAULT 1,
        [OverallInspectionGrade] NVARCHAR(30) NOT NULL CONSTRAINT [DF_RepairJobs_Grade] DEFAULT 'Pass - Good Condition',
        CONSTRAINT [PK_RepairJobCards] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RepairJobCards_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RepairJobCards_Appointments] FOREIGN KEY ([ServiceAppointmentId]) REFERENCES [dbo].[ServiceAppointments]([Id]),
        CONSTRAINT [FK_RepairJobCards_Mechanics] FOREIGN KEY ([LeadMechanicId]) REFERENCES [dbo].[Mechanics]([Id])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_RepairJobCards_Number] ON [dbo].[RepairJobCards] ([JobCardNumber] ASC);
END
GO

-- 6. Table: RepairLaborItems
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RepairLaborItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RepairLaborItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RepairJobCardId] INT NOT NULL,
        [Description] NVARCHAR(200) NOT NULL,
        [Hours] DECIMAL(18,2) NOT NULL,
        [HourlyRate] DECIMAL(18,2) NOT NULL,
        CONSTRAINT [PK_RepairLaborItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RepairLaborItems_JobCard] FOREIGN KEY ([RepairJobCardId]) REFERENCES [dbo].[RepairJobCards]([Id]) ON DELETE CASCADE
    );
END
GO

-- 7. Table: RepairPartItems
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RepairPartItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RepairPartItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RepairJobCardId] INT NOT NULL,
        [PartName] NVARCHAR(150) NOT NULL,
        [PartNumber] NVARCHAR(50) NULL,
        [Quantity] INT NOT NULL CONSTRAINT [DF_RepairParts_Qty] DEFAULT 1,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        CONSTRAINT [PK_RepairPartItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RepairPartItems_JobCard] FOREIGN KEY ([RepairJobCardId]) REFERENCES [dbo].[RepairJobCards]([Id]) ON DELETE CASCADE
    );
END
GO

-- 8. Table: Invoices
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Invoices] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [InvoiceNumber] NVARCHAR(30) NOT NULL,
        [RepairJobCardId] INT NOT NULL,
        [VehicleId] INT NOT NULL,
        [InvoiceDate] DATETIME2(7) NOT NULL CONSTRAINT [DF_Invoices_Date] DEFAULT SYSUTCDATETIME(),
        [DueDate] DATETIME2(7) NOT NULL,
        [LaborTotal] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Labor] DEFAULT 0.00,
        [PartsTotal] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Parts] DEFAULT 0.00,
        [SubTotal] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Subtotal] DEFAULT 0.00,
        [DiscountPercentage] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_DiscPct] DEFAULT 0.00,
        [DiscountAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Disc] DEFAULT 0.00,
        [TaxRate] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_TaxRate] DEFAULT 10.00,
        [TaxAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Tax] DEFAULT 0.00,
        [GrandTotal] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Total] DEFAULT 0.00,
        [AmountPaid] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Paid] DEFAULT 0.00,
        [BalanceDue] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_Balance] DEFAULT 0.00,
        [Status] INT NOT NULL CONSTRAINT [DF_Invoices_Status] DEFAULT 0,
        [PaymentMethod] NVARCHAR(50) NULL,
        [TransactionReference] NVARCHAR(100) NULL,
        [PaidAt] DATETIME2(7) NULL,
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Invoices_JobCard] FOREIGN KEY ([RepairJobCardId]) REFERENCES [dbo].[RepairJobCards]([Id]),
        CONSTRAINT [FK_Invoices_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Invoices_Number] ON [dbo].[Invoices] ([InvoiceNumber] ASC);
END
GO

PRINT 'CarServiceDb SQL Server schema successfully initialized.';
