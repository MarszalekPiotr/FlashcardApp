using Flashcard.Modules.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FlashCard.Modules.Users.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // jawna nazwa tabeli i (opcjonalnie) schemat
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Email).IsRequired();
        }
    }
}
