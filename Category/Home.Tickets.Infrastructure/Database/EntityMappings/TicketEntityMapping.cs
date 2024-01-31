using Home.Tickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using static Home.Tickets.Domain.Entities.Ticket;

namespace Home.Tickets.Infrastructure.Database.EntityMappings {
    public class TicketEntityMappingConfiguration : IEntityTypeConfiguration<Ticket> {
        public void Configure(EntityTypeBuilder<Ticket> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Shop).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AccountingGroup).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EmitedAt).IsRequired();
            builder.Property(x => x.ClosedAt);
            builder.Property(x => x.IsOpen).IsRequired();
            builder.Property(x => x.Total).IsRequired();
            builder.Property(x => x.TotalPaid).IsRequired();

            builder.Ignore(x => x.Items);

            var comparer = new ValueComparer<ICollection<TicketItem>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => (ICollection<TicketItem>)c.ToList());

            builder.Property("ticketItems").HasConversion(new ValueConverter<ICollection<TicketItem>, string>(
            v => Serialize(v),
            v => Deserialize(v)), comparer).UsePropertyAccessMode(PropertyAccessMode.Field);


            //builder.OwnsMany<TicketItem>("ticketItems", x => {
            //    //x.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
            //    x.ToJson();
            //    x.WithOwner();
            //});
        }

        private ICollection<TicketItem> Deserialize(string v) {
            return JsonSerializer.Deserialize<ICollection<TicketItem>>(v) ?? new List<TicketItem>();
        }

        private string Serialize(ICollection<TicketItem> v) {
            return JsonSerializer.Serialize(v);
        }
    }

    //public class TicketItemEntityMappingConfiguration : IEntityTypeConfiguration<TicketItem> {
    //    public void Configure(EntityTypeBuilder<TicketItem> builder) {
    //        builder.HasNoKey();
    //    }
    //}
}
