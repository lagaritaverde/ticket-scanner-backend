using Home.Extractor.Entities;

namespace Home.Extractor.Parsers {
    public interface ITicketParser {
        Ticket Parse(string ticketData);
    }
}
