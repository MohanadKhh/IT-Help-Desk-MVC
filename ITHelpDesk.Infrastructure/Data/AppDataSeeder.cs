using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Data;

public static class AppDataSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // Require database to be created
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Roles
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        // 2. Seed Users
        ApplicationUser adminUser = null!;
        ApplicationUser normalUser = null!;

        if (!userManager.Users.Any())
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@helpdesk.com",
                Email = "admin@helpdesk.com",
                FullName = "System Administrator",
                Department = "IT",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            normalUser = new ApplicationUser
            {
                UserName = "user@helpdesk.com",
                Email = "user@helpdesk.com",
                FullName = "Regular User",
                Department = "HR",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(normalUser, "User@123");
            await userManager.AddToRoleAsync(normalUser, "User");
        }
        else
        {
            adminUser = await userManager.FindByEmailAsync("admin@helpdesk.com") ?? userManager.Users.First();
            normalUser = await userManager.FindByEmailAsync("user@helpdesk.com") ?? userManager.Users.First();
        }

        //DateTime values for seeding
        var createdAt = new DateTime(2026, 6, 6);
        var updatedAt = new DateTime(2026, 6, 7);
        var dueDate = new DateTime(2026, 7, 1, 12, 0, 0);

        // 3. Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new Category[]
            {
                new Category { Name = "Hardware", Description = "Issues related to physical hardware" },
                new Category { Name = "Software", Description = "Application and OS related issues" },
                new Category { Name = "Network", Description = "Connectivity and network issues" },
                new Category { Name = "Access", Description = "Account, login, and access issues" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 4. Seed Tickets
        if (!await context.Tickets.AnyAsync())
        {
            var softwareCategory = await context.Categories.FirstAsync(c => c.Name == "Software");
            var hardwareCategory = await context.Categories.FirstAsync(c => c.Name == "Hardware");
            var networkCategory = await context.Categories.FirstAsync(c => c.Name == "Network");

            var tickets = new Ticket[]
            {
                // Ticket 1
                new Ticket
                {
                    Title        = "Cannot login to CRM",
                    Description  = "I keep getting an invalid credentials error when trying to access the CRM system.",
                    Status       = TicketStatus.Open,
                    Priority     = "High",
                    CategoryId   = softwareCategory.CategoryId,
                    CreatedById  = normalUser.Id,
                    CreatedAt    = createdAt,
                    UpdatedAt    = updatedAt,
                    DueDate      = dueDate
                },
                // Ticket 2
                new Ticket
                {
                    Title        = "Laptop battery dying quickly",
                    Description  = "My laptop battery lasts only 30 minutes after a full charge.",
                    Status       = TicketStatus.InProgress,
                    Priority     = "Medium",
                    CategoryId   = hardwareCategory.CategoryId,
                    CreatedById  = normalUser.Id,
                    AssignedToId = adminUser.Id,
                    CreatedAt    = createdAt,
                    UpdatedAt    = updatedAt,
                    DueDate      = dueDate.AddHours(2)
                },
                // Ticket 3
                new Ticket
                {
                    Title        = "Cannot connect to VPN",
                    Description  = "VPN client throws a timeout error every time I try to connect from home.",
                    Status       = TicketStatus.Open,
                    Priority     = "High",
                    CategoryId   = networkCategory.CategoryId,
                    CreatedById  = normalUser.Id,
                    CreatedAt    = createdAt,
                    UpdatedAt    = updatedAt,
                    DueDate      = dueDate.AddDays(1)
                },
                // Ticket 4
                new Ticket
                {
                    Title        = "Printer not responding",
                    Description  = "The shared office printer on Floor 2 is offline and not printing any jobs.",
                    Status       = TicketStatus.Resolved,
                    Priority     = "Low",
                    CategoryId   = hardwareCategory.CategoryId,
                    CreatedById  = normalUser.Id,
                    AssignedToId = adminUser.Id,
                    CreatedAt    = createdAt,
                    UpdatedAt    = updatedAt,
                    DueDate      = dueDate.AddDays(-1)
                },
                // Ticket 5
                new Ticket
                {
                    Title        = "Email client crashing on startup",
                    Description  = "Outlook crashes immediately after opening. Reinstalling did not fix the issue.",
                    Status       = TicketStatus.InProgress,
                    Priority     = "Critical",
                    CategoryId   = softwareCategory.CategoryId,
                    CreatedById  = normalUser.Id,
                    AssignedToId = adminUser.Id,
                    CreatedAt    = createdAt,
                    UpdatedAt    = updatedAt,
                    DueDate      = dueDate.AddDays(7)
                }
            };

            await context.Tickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();

            // ── Comments ────────────────────────────────────────────

            var comments = new TicketComment[]
            {
                // Ticket 1 — Open, no resolution yet
                new TicketComment
                {
                    TicketId  = tickets[0].TicketId,
                    CreatedById    = adminUser.Id,
                    Content      = "Checked the account — password may have expired. Please try resetting it via the portal.",
                    CreatedAt = createdAt
                },
                // Ticket 2 — InProgress
                new TicketComment
                {
                    TicketId  = tickets[1].TicketId,
                    CreatedById    = adminUser.Id,
                    Content      = "I have ordered a replacement battery. It should arrive by tomorrow.",
                    CreatedAt = createdAt
                },
                // Ticket 3 — Open
                new TicketComment
                {
                    TicketId  = tickets[2].TicketId,
                    CreatedById    = adminUser.Id,
                    Content      = "Please confirm your VPN client version. We pushed a config update last week that may require a reinstall.",
                    CreatedAt = createdAt
                },
                // Ticket 4 — Resolved
                new TicketComment
                {
                    TicketId  = tickets[3].TicketId,
                    CreatedById    = adminUser.Id,
                    Content      = "Printer driver was corrupted. Reinstalled the driver and reconnected the device. Issue resolved.",
                    CreatedAt = createdAt
                },
                // Ticket 5 — InProgress
                new TicketComment
                {
                    TicketId  = tickets[4].TicketId,
                    CreatedById    = adminUser.Id,
                    Content      = "Reproduced the crash on our end. Likely a corrupted profile. Creating a new Outlook profile now.",
                    CreatedAt = createdAt
                }
            };

            await context.TicketComments.AddRangeAsync(comments);

            // ── Histories ───────────────────────────────────────────

            var histories = new TicketHistory[]
            {
                // Ticket 1 — still Open, no status change yet
                new TicketHistory
                {
                    TicketId      = tickets[0].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.TicketCreated,
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[0].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.Priority,
                    OldValue      = "Medium",
                    NewValue      = "High",
                    ChangedAt     = createdAt
                },

                // Ticket 2 — moved to InProgress
                new TicketHistory
                {
                    TicketId      = tickets[1].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.TicketCreated,
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[1].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.Status,
                    OldValue      = TicketStatus.Open.ToString(),
                    NewValue      = TicketStatus.InProgress.ToString(),
                    ChangedAt     = createdAt
                },

                // Ticket 3 — assigned to admin
                new TicketHistory
                {
                    TicketId      = tickets[2].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.TicketCreated,
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[2].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.AssignedTo,
                    OldValue      = "Unassigned",
                    NewValue      = adminUser.UserName,
                    ChangedAt     = createdAt
                },

                // Ticket 4 — moved to Resolved
                new TicketHistory
                {
                    TicketId      = tickets[3].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.TicketCreated,
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[3].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.Status,
                    OldValue      = TicketStatus.InProgress.ToString(),
                    NewValue      = TicketStatus.Resolved.ToString(),
                    ChangedAt     = createdAt
                },

                // Ticket 5 — moved to InProgress + priority escalated
                new TicketHistory
                {
                    TicketId      = tickets[4].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.TicketCreated,
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[4].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.Status,
                    OldValue      = TicketStatus.Open.ToString(),
                    NewValue      = TicketStatus.InProgress.ToString(),
                    ChangedAt     = createdAt
                },
                new TicketHistory
                {
                    TicketId      = tickets[4].TicketId,
                    ChangedById   = adminUser.Id,
                    FieldChanged  = TicketHistoryField.Priority,
                    OldValue      = "High",
                    NewValue      = "Critical",
                    ChangedAt     = createdAt
                }
            };

            await context.TicketHistories.AddRangeAsync(histories);
            await context.SaveChangesAsync();
        }
    }
}
