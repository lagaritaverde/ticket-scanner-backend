using Home.OutBox.Service;
using Home.Tickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Home.Tickets.Infrastructure.Database.EntityMappings {
    public class PurchaseEntityMapping : IEntityTypeConfiguration<Purchase> {
        public void Configure(EntityTypeBuilder<Purchase> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Shop).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AccountingGroup).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.GroupId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.UnitPrice).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(50);
            builder.Property(x => x.ShopItemName).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Category).HasMaxLength(50);
            builder.Property(x => x.MustPayTo).HasMaxLength(50);
            builder.Property(x => x.Owner).HasMaxLength(50);
            builder.Property(x => x.WhoPaid).HasMaxLength(50);
        }
    }
}
