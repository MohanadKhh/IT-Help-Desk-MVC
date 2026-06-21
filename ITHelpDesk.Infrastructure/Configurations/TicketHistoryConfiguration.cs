using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITHelpDesk.Infrastructure.Configurations;

public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("Ticket_History");
        builder.HasKey(history => history.HistoryId);

        builder.Property(history => history.FieldChanged)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(history => history.OldValue)
            .HasMaxLength(200);

        builder.Property(history => history.NewValue)
            .HasMaxLength(200);

        builder.Property(history => history.ChangedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(history => history.Ticket)
            .WithMany(ticket => ticket.TicketHistories)
            .HasForeignKey(history => history.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(history => history.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
