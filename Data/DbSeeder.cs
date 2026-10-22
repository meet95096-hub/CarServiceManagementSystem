using web.net.Models;

namespace web.net.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if data already exists
        if (context.Vehicles.Any())
        {
            return;
        }

        // 1. Seed Mechanics
        var mechanics = new List<Mechanic>
        {
            new Mechanic
            {
                FullName = "Marcus Vance",
                Specialization = "Master Engine & Powertrain Specialist",
                Phone = "+1 (555) 234-5678",
                Email = "m.vance@apexautoworks.com",
                AssignedBay = "Bay 1 - Diagnostics & Engine",
                ExperienceYears = 12,
                IsAvailable = true
            },
            new Mechanic
            {
                FullName = "Elena Rostova",
                Specialization = "High-Performance Brakes & Suspension",
                Phone = "+1 (555) 345-6789",
                Email = "e.rostova@apexautoworks.com",
                AssignedBay = "Bay 2 - Brake & Chassis",
                ExperienceYears = 9,
                IsAvailable = true
            },
            new Mechanic
            {
                FullName = "David Chen",
                Specialization = "Hybrid, EV & Automotive Electronics",
                Phone = "+1 (555) 456-7890",
                Email = "d.chen@apexautoworks.com",
                AssignedBay = "Bay 3 - Electrical & HVAC",
                ExperienceYears = 8,
                IsAvailable = true
            },
            new Mechanic
            {
                FullName = "Sarah Jenkins",
                Specialization = "Scheduled Maintenance & Quick Lube",
                Phone = "+1 (555) 567-8901",
                Email = "s.jenkins@apexautoworks.com",
                AssignedBay = "Bay 4 - Express Lube & Tires",
                ExperienceYears = 6,
                IsAvailable = true
            }
        };
        await context.Mechanics.AddRangeAsync(mechanics);
        await context.SaveChangesAsync();

        // 2. Seed Service Packages
        var packages = new List<ServicePackage>
        {
            new ServicePackage
            {
                Name = "Comprehensive Periodic Service (30,000 / 60,000 km)",
                Category = "Maintenance",
                Description = "Full synthetic oil & filter change, 45-point health check, spark plug check, coolant top-up, brake cleaning.",
                BaseLaborPrice = 180.00m,
                EstimatedHours = 2.5m,
                IconClass = "bi-speedometer2"
            },
            new ServicePackage
            {
                Name = "Express Synthetic Oil & Filter Service",
                Category = "Maintenance",
                Description = "High-grade OEM spec synthetic oil, premium filter, tire pressure audit, fluid levels inspection.",
                BaseLaborPrice = 65.00m,
                EstimatedHours = 0.75m,
                IconClass = "bi-droplet-half"
            },
            new ServicePackage
            {
                Name = "Performance Brake Rotor & Pad Replacement",
                Category = "Brakes",
                Description = "Front and rear brake disc inspection, rotor resurfacing/replacement, ceramic pads fitting, brake fluid flush.",
                BaseLaborPrice = 160.00m,
                EstimatedHours = 2.0m,
                IconClass = "bi-disc"
            },
            new ServicePackage
            {
                Name = "Complete OBD-II Diagnostics & ECU Scan",
                Category = "Diagnostics",
                Description = "Full digital sensor telemetry test, freeze-frame code review, battery load test, alternator check.",
                BaseLaborPrice = 95.00m,
                EstimatedHours = 1.0m,
                IconClass = "bi-cpu"
            },
            new ServicePackage
            {
                Name = "Climate Control & AC Evaporator Service",
                Category = "HVAC",
                Description = "Refrigerant recovery, leak check, compressor oil recharge, anti-bacterial cabin filter replacement.",
                BaseLaborPrice = 120.00m,
                EstimatedHours = 1.5m,
                IconClass = "bi-snow"
            },
            new ServicePackage
            {
                Name = "Laser Wheel Alignment & Precision Balancing",
                Category = "Tires & Steering",
                Description = "4-wheel 3D digital alignment, computerized dynamic tire balancing, steering rack inspection.",
                BaseLaborPrice = 90.00m,
                EstimatedHours = 1.0m,
                IconClass = "bi-gear-wide-connected"
            }
        };
        await context.ServicePackages.AddRangeAsync(packages);
        await context.SaveChangesAsync();

        // 3. Seed Vehicles
        var vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                LicensePlate = "CAL-7892",
                Make = "BMW",
                Model = "330i M Sport",
                Year = 2022,
                Color = "Alpine White",
                Vin = "WBA5R1C50NFP84210",
                CurrentMileage = 34500,
                FuelType = "Gasoline",
                Transmission = "8-Speed Automatic",
                OwnerName = "Alexander Wright",
                OwnerPhone = "+1 (555) 789-0123",
                OwnerEmail = "alex.wright@example.com",
                OwnerAddress = "742 Evergreen Terrace, Springfield, CA",
                RegisteredAt = DateTime.UtcNow.AddDays(-120)
            },
            new Vehicle
            {
                LicensePlate = "TEX-4019",
                Make = "Toyota",
                Model = "RAV4 Hybrid XLE",
                Year = 2021,
                Color = "Magnetic Gray Metallic",
                Vin = "2T3P1RFV5MW392810",
                CurrentMileage = 48200,
                FuelType = "Hybrid",
                Transmission = "eCVT",
                OwnerName = "Samantha Miller",
                OwnerPhone = "+1 (555) 890-1234",
                OwnerEmail = "sam.miller@techcorp.io",
                OwnerAddress = "1200 Innovation Blvd, Austin, TX",
                RegisteredAt = DateTime.UtcNow.AddDays(-90)
            },
            new Vehicle
            {
                LicensePlate = "FLA-9284",
                Make = "Ford",
                Model = "Mustang GT Premium",
                Year = 2023,
                Color = "Race Red",
                Vin = "1FA6P8CF9P5104829",
                CurrentMileage = 18900,
                FuelType = "Gasoline",
                Transmission = "6-Speed Manual",
                OwnerName = "Carlos Mendez",
                OwnerPhone = "+1 (555) 901-2345",
                OwnerEmail = "carlos.m@speedway.com",
                OwnerAddress = "45 Ocean Drive, Miami, FL",
                RegisteredAt = DateTime.UtcNow.AddDays(-60)
            },
            new Vehicle
            {
                LicensePlate = "NYK-3341",
                Make = "Audi",
                Model = "A6 2.0T Quattro",
                Year = 2020,
                Color = "Mythos Black Metallic",
                Vin = "WAUZZZF27LN049281",
                CurrentMileage = 62400,
                FuelType = "Gasoline",
                Transmission = "7-Speed S tronic",
                OwnerName = "Victoria Hughes",
                OwnerPhone = "+1 (555) 012-3456",
                OwnerEmail = "v.hughes@manhattanlaw.org",
                OwnerAddress = "880 5th Avenue, New York, NY",
                RegisteredAt = DateTime.UtcNow.AddDays(-45)
            },
            new Vehicle
            {
                LicensePlate = "WA-6628",
                Make = "Tesla",
                Model = "Model 3 Long Range",
                Year = 2023,
                Color = "Deep Blue Metallic",
                Vin = "5YJ3E1EB9PF948271",
                CurrentMileage = 21000,
                FuelType = "Electric",
                Transmission = "Single Speed",
                OwnerName = "Ethan Cooper",
                OwnerPhone = "+1 (555) 123-9876",
                OwnerEmail = "ethan.cooper@cloudlab.net",
                OwnerAddress = "404 Bellevue Way, Seattle, WA",
                RegisteredAt = DateTime.UtcNow.AddDays(-30)
            },
            new Vehicle
            {
                LicensePlate = "IL-5582",
                Make = "Honda",
                Model = "Civic Touring",
                Year = 2022,
                Color = "Platinum White Pearl",
                Vin = "1HGCR2F83NA094829",
                CurrentMileage = 29400,
                FuelType = "Gasoline",
                Transmission = "CVT",
                OwnerName = "Rachel Patel",
                OwnerPhone = "+1 (555) 234-8765",
                OwnerEmail = "rachel.patel@horizon.edu",
                OwnerAddress = "310 Michigan Ave, Chicago, IL",
                RegisteredAt = DateTime.UtcNow.AddDays(-15)
            }
        };
        await context.Vehicles.AddRangeAsync(vehicles);
        await context.SaveChangesAsync();

        // 4. Seed Service Appointments
        var appt1 = new ServiceAppointment
        {
            AppointmentCode = "APT-2026-101",
            VehicleId = vehicles[0].Id,
            ServicePackageId = packages[0].Id,
            ServiceTitle = "Scheduled 30,000 km Service + Brake Inspection",
            ScheduledDate = DateTime.Today,
            TimeSlot = "09:00 AM - 11:00 AM",
            MechanicId = mechanics[0].Id,
            Bay = "Bay 1 - Diagnostics & Engine",
            CustomerNotes = "Noticed a slight vibration during high speed braking. Please check front rotors.",
            EstimatedCost = 420.00m,
            Status = AppointmentStatus.InService,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var appt2 = new ServiceAppointment
        {
            AppointmentCode = "APT-2026-102",
            VehicleId = vehicles[1].Id,
            ServicePackageId = packages[1].Id,
            ServiceTitle = "Express Synthetic Oil Service & Tire Rotation",
            ScheduledDate = DateTime.Today,
            TimeSlot = "11:30 AM - 01:30 PM",
            MechanicId = mechanics[3].Id,
            Bay = "Bay 4 - Express Lube & Tires",
            CustomerNotes = "Regular 5,000 mile scheduled maintenance.",
            EstimatedCost = 110.00m,
            Status = AppointmentStatus.Confirmed,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var appt3 = new ServiceAppointment
        {
            AppointmentCode = "APT-2026-103",
            VehicleId = vehicles[2].Id,
            ServicePackageId = packages[2].Id,
            ServiceTitle = "Brembo Brake Upgrade & Fluid Flush",
            ScheduledDate = DateTime.Today.AddDays(1),
            TimeSlot = "02:00 PM - 04:00 PM",
            MechanicId = mechanics[1].Id,
            Bay = "Bay 2 - Brake & Chassis",
            CustomerNotes = "Customer bringing car for track inspection prep.",
            EstimatedCost = 580.00m,
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var appt4 = new ServiceAppointment
        {
            AppointmentCode = "APT-2026-104",
            VehicleId = vehicles[3].Id,
            ServicePackageId = packages[4].Id,
            ServiceTitle = "AC System Diagnosis & Cabin Filter",
            ScheduledDate = DateTime.Today.AddDays(-5),
            TimeSlot = "09:00 AM - 11:00 AM",
            MechanicId = mechanics[2].Id,
            Bay = "Bay 3 - Electrical & HVAC",
            CustomerNotes = "AC blowing warm air on passenger side vents.",
            EstimatedCost = 350.00m,
            Status = AppointmentStatus.Completed,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        await context.ServiceAppointments.AddRangeAsync(appt1, appt2, appt3, appt4);
        await context.SaveChangesAsync();

        // 5. Seed Active & Completed Repair Job Cards
        // Job 1: In Progress (BMW 330i)
        var job1 = new RepairJobCard
        {
            JobCardNumber = "JOB-2026-8801",
            VehicleId = vehicles[0].Id,
            ServiceAppointmentId = appt1.Id,
            LeadMechanicId = mechanics[0].Id,
            Bay = "Bay 1 - Diagnostics & Engine",
            OdometerIn = 34500,
            DiagnosticNotes = "Confirmed brake rotor runout of 0.04mm causing vibration. Front ceramic pads worn down to 3mm. Engine oil dark but no sludge.",
            WorkPerformedSummary = "Replaced front slotted brake discs and ceramic performance pads. Completed full synthetic oil & OEM filter change.",
            Status = RepairStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddHours(-3),
            EstimatedCompletionAt = DateTime.UtcNow.AddHours(2),
            CheckEngineOil = true,
            CheckBrakeSystem = true,
            CheckBatteryHealth = true,
            CheckTirePressureTread = true,
            CheckCoolantRadiator = true,
            CheckSuspensionSteering = true,
            CheckLightsElectronics = true,
            OverallInspectionGrade = "Attention Needed - Brake pads worn"
        };
        await context.RepairJobCards.AddAsync(job1);
        await context.SaveChangesAsync();

        appt1.LinkedRepairJobId = job1.Id;
        await context.SaveChangesAsync();

        // Add Labor and Parts for Job 1
        var job1Labor = new List<RepairLaborItem>
        {
            new RepairLaborItem
            {
                RepairJobCardId = job1.Id,
                Description = "Front Brake Rotor & Ceramic Pad Replacement",
                Hours = 2.0m,
                HourlyRate = 95.00m
            },
            new RepairLaborItem
            {
                RepairJobCardId = job1.Id,
                Description = "Full Synthetic Oil & Filter Service Labor",
                Hours = 0.8m,
                HourlyRate = 85.00m
            }
        };
        var job1Parts = new List<RepairPartItem>
        {
            new RepairPartItem
            {
                RepairJobCardId = job1.Id,
                PartName = "Brembo Sport Slotted Front Brake Rotors (Pair)",
                PartNumber = "BRM-09C39413",
                Quantity = 1,
                UnitPrice = 240.00m
            },
            new RepairPartItem
            {
                RepairJobCardId = job1.Id,
                PartName = "Akebono Euro Ultra-Premium Ceramic Brake Pads",
                PartNumber = "EUR-1875",
                Quantity = 1,
                UnitPrice = 98.00m
            },
            new RepairPartItem
            {
                RepairJobCardId = job1.Id,
                PartName = "Castrol Edge 5W-30 Advanced Full Synthetic (5 Liters)",
                PartNumber = "CAS-03084",
                Quantity = 1,
                UnitPrice = 48.50m
            },
            new RepairPartItem
            {
                RepairJobCardId = job1.Id,
                PartName = "Mann-Filter High Efficiency Oil Filter Cartridge",
                PartNumber = "HU-816-X",
                Quantity = 1,
                UnitPrice = 18.00m
            }
        };
        await context.RepairLaborItems.AddRangeAsync(job1Labor);
        await context.RepairPartItems.AddRangeAsync(job1Parts);
        await context.SaveChangesAsync();

        // Job 2: Completed & Invoiced (Audi A6)
        var job2 = new RepairJobCard
        {
            JobCardNumber = "JOB-2026-8742",
            VehicleId = vehicles[3].Id,
            ServiceAppointmentId = appt4.Id,
            LeadMechanicId = mechanics[2].Id,
            Bay = "Bay 3 - Electrical & HVAC",
            OdometerIn = 62400,
            DiagnosticNotes = "HVAC blend door actuator faulty causing warm air on passenger side. Low R134a refrigerant levels detected.",
            WorkPerformedSummary = "Replaced passenger side blend door actuator, evacuated and recharged AC system with R134a, installed OEM activated carbon cabin filter.",
            Status = RepairStatus.Delivered,
            StartedAt = DateTime.UtcNow.AddDays(-5),
            EstimatedCompletionAt = DateTime.UtcNow.AddDays(-5).AddHours(4),
            CompletedAt = DateTime.UtcNow.AddDays(-5).AddHours(4),
            CheckEngineOil = true,
            CheckBrakeSystem = true,
            CheckBatteryHealth = true,
            CheckTirePressureTread = true,
            CheckCoolantRadiator = true,
            CheckSuspensionSteering = true,
            CheckLightsElectronics = true,
            OverallInspectionGrade = "Pass - Excellent condition"
        };
        await context.RepairJobCards.AddAsync(job2);
        await context.SaveChangesAsync();

        var job2Labor = new List<RepairLaborItem>
        {
            new RepairLaborItem
            {
                RepairJobCardId = job2.Id,
                Description = "HVAC Blend Door Actuator Removal & Replacement",
                Hours = 1.5m,
                HourlyRate = 95.00m
            },
            new RepairLaborItem
            {
                RepairJobCardId = job2.Id,
                Description = "AC Refrigerant Evacuation, Leak Test & Recharge",
                Hours = 1.0m,
                HourlyRate = 90.00m
            }
        };
        var job2Parts = new List<RepairPartItem>
        {
            new RepairPartItem
            {
                RepairJobCardId = job2.Id,
                PartName = "OEM Audi Climate Blend Door Actuator Motor",
                PartNumber = "VAG-4F0820511B",
                Quantity = 1,
                UnitPrice = 115.00m
            },
            new RepairPartItem
            {
                RepairJobCardId = job2.Id,
                PartName = "R134a Refrigerant Gas & UV Dye Injector (600g)",
                PartNumber = "REF-134A-600",
                Quantity = 1,
                UnitPrice = 45.00m
            },
            new RepairPartItem
            {
                RepairJobCardId = job2.Id,
                PartName = "Bosch HEPA Activated Carbon Cabin Air Filter",
                PartNumber = "BSH-6034C",
                Quantity = 1,
                UnitPrice = 32.00m
            }
        };
        await context.RepairLaborItems.AddRangeAsync(job2Labor);
        await context.RepairPartItems.AddRangeAsync(job2Parts);
        await context.SaveChangesAsync();

        // 6. Seed Invoices
        // Invoice 1: Paid invoice for Job 2
        decimal j2LaborTotal = job2Labor.Sum(l => l.Hours * l.HourlyRate);
        decimal j2PartsTotal = job2Parts.Sum(p => p.Quantity * p.UnitPrice);
        decimal j2SubTotal = j2LaborTotal + j2PartsTotal;
        decimal j2DiscountRate = 5.0m;
        decimal j2DiscountAmount = Math.Round(j2SubTotal * (j2DiscountRate / 100m), 2);
        decimal j2TaxRate = 10.0m;
        decimal j2TaxAmount = Math.Round((j2SubTotal - j2DiscountAmount) * (j2TaxRate / 100m), 2);
        decimal j2GrandTotal = (j2SubTotal - j2DiscountAmount) + j2TaxAmount;

        var invoice1 = new Invoice
        {
            InvoiceNumber = "INV-2026-0418",
            RepairJobCardId = job2.Id,
            VehicleId = vehicles[3].Id,
            InvoiceDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(2),
            LaborTotal = j2LaborTotal,
            PartsTotal = j2PartsTotal,
            SubTotal = j2SubTotal,
            DiscountPercentage = j2DiscountRate,
            DiscountAmount = j2DiscountAmount,
            TaxRate = j2TaxRate,
            TaxAmount = j2TaxAmount,
            GrandTotal = j2GrandTotal,
            AmountPaid = j2GrandTotal,
            BalanceDue = 0m,
            Status = InvoiceStatus.Paid,
            PaymentMethod = "Credit Card (Visa •••• 4092)",
            TransactionReference = "TXN-CC-89410294",
            PaidAt = DateTime.UtcNow.AddDays(-5).AddHours(5)
        };
        await context.Invoices.AddAsync(invoice1);

        // Job 3: Ready for Delivery (Tesla Model 3 - Alignment & Suspension)
        var job3 = new RepairJobCard
        {
            JobCardNumber = "JOB-2026-8809",
            VehicleId = vehicles[4].Id,
            LeadMechanicId = mechanics[2].Id,
            Bay = "Bay 3 - Electrical & HVAC",
            OdometerIn = 21000,
            DiagnosticNotes = "Vehicle pulling slightly right on freeway. Steering angle calibration off by 1.8 degrees.",
            WorkPerformedSummary = "Completed 3D laser 4-wheel alignment and calibrated electronic steering rack angle sensor. Inspected suspension bushings.",
            Status = RepairStatus.ReadyForDelivery,
            StartedAt = DateTime.UtcNow.AddHours(-6),
            EstimatedCompletionAt = DateTime.UtcNow.AddHours(-1),
            CompletedAt = DateTime.UtcNow.AddHours(-1),
            CheckEngineOil = true,
            CheckBrakeSystem = true,
            CheckBatteryHealth = true,
            CheckTirePressureTread = true,
            CheckCoolantRadiator = true,
            CheckSuspensionSteering = true,
            CheckLightsElectronics = true,
            OverallInspectionGrade = "Pass - Excellent condition"
        };
        await context.RepairJobCards.AddAsync(job3);
        await context.SaveChangesAsync();

        var job3Labor = new List<RepairLaborItem>
        {
            new RepairLaborItem
            {
                RepairJobCardId = job3.Id,
                Description = "3D Laser 4-Wheel Precision Alignment",
                Hours = 1.2m,
                HourlyRate = 95.00m
            },
            new RepairLaborItem
            {
                RepairJobCardId = job3.Id,
                Description = "Electronic Steering Angle Sensor Digital Calibration",
                Hours = 0.6m,
                HourlyRate = 95.00m
            }
        };
        var job3Parts = new List<RepairPartItem>
        {
            new RepairPartItem
            {
                RepairJobCardId = job3.Id,
                PartName = "Tire Nitrogen Purge & Pressure Sensor Kit",
                PartNumber = "NIT-TPMS-SET",
                Quantity = 1,
                UnitPrice = 35.00m
            }
        };
        await context.RepairLaborItems.AddRangeAsync(job3Labor);
        await context.RepairPartItems.AddRangeAsync(job3Parts);
        await context.SaveChangesAsync();

        // Invoice 2: Unpaid invoice for Job 3
        decimal j3LaborTotal = job3Labor.Sum(l => l.Hours * l.HourlyRate);
        decimal j3PartsTotal = job3Parts.Sum(p => p.Quantity * p.UnitPrice);
        decimal j3SubTotal = j3LaborTotal + j3PartsTotal;
        decimal j3TaxRate = 10.0m;
        decimal j3TaxAmount = Math.Round(j3SubTotal * (j3TaxRate / 100m), 2);
        decimal j3GrandTotal = j3SubTotal + j3TaxAmount;

        var invoice2 = new Invoice
        {
            InvoiceNumber = "INV-2026-0422",
            RepairJobCardId = job3.Id,
            VehicleId = vehicles[4].Id,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            LaborTotal = j3LaborTotal,
            PartsTotal = j3PartsTotal,
            SubTotal = j3SubTotal,
            DiscountPercentage = 0m,
            DiscountAmount = 0m,
            TaxRate = j3TaxRate,
            TaxAmount = j3TaxAmount,
            GrandTotal = j3GrandTotal,
            AmountPaid = 0m,
            BalanceDue = j3GrandTotal,
            Status = InvoiceStatus.Unpaid
        };
        await context.Invoices.AddAsync(invoice2);
        await context.SaveChangesAsync();
    }
}
