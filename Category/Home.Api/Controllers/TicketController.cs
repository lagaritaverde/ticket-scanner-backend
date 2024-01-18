using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Home.Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    //[Produces("application/json")]
    public class TicketController : ControllerBase {
        private readonly IRepository<Ticket> repository;
        private readonly ILogger<TicketController> _logger;

        public TicketController(IRepository<Ticket> repository, ILogger<TicketController> logger) {
            this.repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}", Name = "GetTicket")]
        [Produces<TicketModel>]
        public async Task<IActionResult> GetTicket(string id) {

            var ticket = await repository.Get(id);

            if (ticket == null) {
                return NotFound();
            }

            var model = new TicketModel() {
                Id = ticket.Id,
                ClosedAt = ticket.ClosedAt,
                EmitedAt = ticket.EmitedAt,
                Shop = ticket.Shop,
                Total = ticket.Total,
                TotalPaid = ticket.TotalPaid,
                Items = ticket.Items.Select(x => new TicketModel.TicketItemModel() {
                    Description = x.Description,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                    UnitPrice = x.UnitPrice
                }).ToArray()
            };

            return Ok(model);
        }

        [HttpGet]
        [Route("", Name = "GetTickets")]
        public IEnumerable<TicketModel> GetTickets() {

            return Array.Empty<TicketModel>();
        }

        [HttpPost]
        [Route("", Name = "CreateTicket")]
        public async Task<string> CreateTicket(TicketCreateModel ticketModel) {

            var ticket = new Ticket(ticketModel.Shop, ticketModel.EmitedAt, ticketModel.TotalPaid);

            ticket.AddItemRange(ticketModel.Items.Select(x => new Ticket.TicketItem(x.Description, x.TotalPrice, x.Quantity)));

            await repository.Add(ticket);

            return ticket.Id;
        }

        [HttpPost]
        [Route("{id}/close", Name = "CloseTicket")]
        public async Task<IActionResult> CloseTicket(string id) {

            var ticket = await repository.Get(id);

            if (ticket == null) {
                return NotFound();
            }

            ticket.Close();

            return Ok();
        }
    }

    public class TicketCreateModel {

        public string Shop { get; set; }
        public DateTime EmitedAt { get; set; }
        public decimal TotalPaid { get; set; }
        public TicketCreateItemModel[] Items { get; set; }

        public class TicketCreateItemModel {
            public string Description { get; set; }
            public decimal TotalPrice { get; set; }
            public int Quantity { get; set; }
        }

    }

    public class TicketModel {

        public string Id { get; set; }
        public string Shop { get; set; }
        public DateTime EmitedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Total { get; set; }
        public TicketItemModel[] Items { get; set; }

        public class TicketItemModel {
            public string Description { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public int Quantity { get; set; }
        }

    }
}
