using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Home.OutBox.Service;
using System.Text.Json;

namespace Home.Tickets.Infrastructure.Database.EntityMappings {
    public class OutBoxEntityMappingConfiguration : IEntityTypeConfiguration<OutBox.Service.OutBox> {
        public void Configure(EntityTypeBuilder<OutBox.Service.OutBox> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Message).IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.Sender).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(300);

            //builder.Property(x => x.Properties).HasConversion(new ValueConverter<List<OutBoxProperty>, string>(
            //v => Serialize(v),
            //v => Deserialize(v)));

            builder.OwnsMany(x => x.Properties, x => x.ToJson());
        }

        private List<OutBoxProperty> Deserialize(string v) {
            return JsonSerializer.Deserialize<List<OutBoxProperty>>(v) ?? new List<OutBoxProperty>();
        }

        private string Serialize(List<OutBoxProperty> v) {
            return JsonSerializer.Serialize(v);
        }
    }
}
