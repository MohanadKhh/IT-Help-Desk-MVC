using ITHelpDesk.Application.Common.Helpers;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITHelpDesk.Application.Services
{
    public class SlaCheckService : ISlaCheckService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ILogger<SlaCheckService> _logger;

        // Adjust this for demo vs production
        private static readonly TimeSpan WarningWindow = TimeSpan.FromHours(1);
        private static readonly Dictionary<string, TimeSpan> UnassignedThresholds = new()
        {
            ["Critical"] = TimeSpan.FromMinutes(1),
            ["High"] = TimeSpan.FromHours(2),
            ["Medium"] = TimeSpan.FromHours(8),
            ["Low"] = TimeSpan.FromHours(24)
        };

        public SlaCheckService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IUserService userService,
            ILogger<SlaCheckService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userService = userService;
            _logger = logger;
        }

        public async Task RunCheckAsync()
        {
            var now = DateTime.UtcNow;
            var tickets = await _unitOfWork.TicketRepository.GetTicketsNeedingSlaCheckAsync();

            if (!tickets.Any())
            {
                _logger.LogInformation("SLA check: no tickets need checking.");
                return;
            }

            foreach (var ticket in tickets)
            {
                var warningThreshold = ticket.DueDate - WarningWindow;

                // Warning case
                if (!ticket.WarningEmailSent && now >= warningThreshold && now < ticket.DueDate)
                {
                    await SendNotificationAsync(ticket, isBreach: false);
                    ticket.WarningEmailSent = true;
                    _unitOfWork.TicketRepository.Update(ticket);
                }

                // Breach case
                if (!ticket.BreachEmailSent && now >= ticket.DueDate)
                {
                    await SendNotificationAsync(ticket, isBreach: true);
                    ticket.BreachEmailSent = true;
                    _unitOfWork.TicketRepository.Update(ticket);
                }


                //Unassigned Case
                if (!ticket.AssignedToId.HasValue && !ticket.UnassignedReminderSent)
                {
                    var threshold = UnassignedThresholds.GetValueOrDefault(ticket.Priority, TimeSpan.FromHours(8));

                    if (now - ticket.CreatedAt >= threshold)
                    {
                        await SendUnassignedReminderAsync(ticket);
                        ticket.UnassignedReminderSent = true;
                        _unitOfWork.TicketRepository.Update(ticket);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SendNotificationAsync(Ticket ticket, bool isBreach)
        {
            if (!ticket.AssignedToId.HasValue)
            {
                _logger.LogInformation("Ticket #{Id} has no assignee, skipping notification.", ticket.TicketId);
                return;
            }

            var assignedEmail = await _userService.GetEmailByUserIdAsync(ticket.AssignedToId.Value);
            if (string.IsNullOrEmpty(assignedEmail))
                return;

            var subject = isBreach
                ? $"🔴 SLA Breached: Ticket #{ticket.TicketId}"
                : $"⚠️ SLA Warning: Ticket #{ticket.TicketId} due soon";

            var body = isBreach
                ? $"""
              <h3>SLA Breached</h3>
              <p>Ticket <strong>{ticket.Title}</strong> has passed its due date.</p>
              <p>Priority: {ticket.Priority}</p>
              <p>Was due: {DateHelper.FormatCairoTime(ticket.DueDate)}</p>
              """
                : $"""
              <h3>SLA Warning</h3>
              <p>Ticket <strong>{ticket.Title}</strong> is approaching its due date.</p>
              <p>Priority: {ticket.Priority}</p>
              <p>Due: {DateHelper.FormatCairoTime(ticket.DueDate)}</p>
              """;

            await _emailService.SendEmailAsync(assignedEmail, subject, body);

            _logger.LogInformation(
                "{Type} CreatorEmail sent for Ticket #{Id}",
                isBreach ? "Breach" : "Warning",
                ticket.TicketId);
        }

        private async Task SendUnassignedReminderAsync(Ticket ticket)
        {
            var CreatorEmail = await _userService.GetEmailByUserIdAsync(ticket.CreatedById);
            if (string.IsNullOrEmpty(CreatorEmail))
                return;

            var subject = $"⏰ Unassigned Ticket: #{ticket.TicketId}";
            var body = $"""
                        <h3>Ticket Still Unassigned</h3>
                        <p>Ticket <strong>{ticket.Title}</strong> has been unassigned for over the expected threshold for its priority.</p>
                        <p>Priority: {ticket.Priority}</p>
                        <p>Created: {DateHelper.FormatCairoTime(ticket.CreatedAt)}</p>
                        """;

            var adminEmails = await _userService.GetAdminEmailsAsync();
            await _emailService.SendEmailAsync(CreatorEmail, subject, body, adminEmails);

            _logger.LogWarning("Ticket #{Id} unassigned reminder sent to admins.", ticket.TicketId);
        }
    }
}
