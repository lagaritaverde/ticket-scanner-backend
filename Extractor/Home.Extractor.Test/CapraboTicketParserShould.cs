using FluentAssertions;
using Home.Extractor.Entities;
using Home.Extractor.Parsers;
using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Home.Extractor.Test {
    public class CapraboTicketParserShould {

        [Theory]
        [InlineData("Ticket_08092023_CAPRABO")]
        [InlineData("Ticket_01092023_CAPRABO")]
        [InlineData("Ticket_22072023_CAPRABO")]
        public void Parse(string ticketName) {
            // Arrange

            var ticketInfo = GetTicket(ticketName);
            var parser = new CapraboTicketParser();


            // Act
            var result = parser.Parse(ticketInfo.TicketData);

            var options1 = new JsonSerializerOptions {
                WriteIndented = true,
            };

            var jsonString = JsonSerializer.Serialize(result.Items, options1);

            // Assert
            result.Should().BeEquivalentTo(ticketInfo.Ticket);
        }


        public static (Ticket Ticket, string TicketData) GetTicket(string ticketName) {
            var jsonPath = System.IO.Path.Combine("data/", ticketName + ".json");
            var textPath = System.IO.Path.Combine("data/", ticketName + ".txt");

            var jsonData = System.IO.File.ReadAllText(jsonPath);

            var textData = System.IO.File.ReadAllText(textPath);
            var ticket = System.Text.Json.JsonSerializer.Deserialize<Ticket>(jsonData);
            return new(ticket, textData);
        }
    }


}