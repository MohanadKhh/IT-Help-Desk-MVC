using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITHelpDesk.Infrastructure.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(ticket => ticket.TicketId);

        builder.Property(ticket => ticket.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ticket => ticket.Description)
            .IsRequired();

        builder.Property(ticket => ticket.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TicketStatus.Open)
            .IsRequired();

        builder.Property(ticket => ticket.Priority)
            .HasMaxLength(20)
            .HasDefaultValue("Medium")
            .IsRequired();

        builder.Property(ticket => ticket.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        //builder.Property(ticket => ticket.UpdatedAt)
        //    .HasDefaultValueSql("GETDATE()");

        builder.HasOne(ticket => ticket.Category)
            .WithMany(category => category.Tickets)
            .HasForeignKey(ticket => ticket.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ticket => ticket.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ticket => ticket.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
