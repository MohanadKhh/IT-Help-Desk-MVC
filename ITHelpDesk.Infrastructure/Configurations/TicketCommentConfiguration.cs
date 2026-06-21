using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITHelpDesk.Infrastructure.Configurations;

public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("Ticket_Comments");
        builder.HasKey(comment => comment.CommentId);

        builder.Property(comment => comment.Content)
            .IsRequired();

        builder.Property(comment => comment.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(comment => comment.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(comment => comment.Ticket)
            .WithMany(ticket => ticket.TicketComments)
            .HasForeignKey(comment => comment.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(comment => comment.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
