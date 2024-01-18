using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Home.Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    //[Produces("application/json")]
    public class PurchaseController : ControllerBase {
        private readonly IRepository<Purchase> repository;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IRepository<Purchase> repository, ILogger<PurchaseController> logger) {
            this.repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("", Name = "GetPurchases")]
        [Produces<PurchaseModel[]>]
        public async Task<IActionResult> List() {

            var purchases = repository.List();
            var models = await purchases.Select(x =>
                new PurchaseModel() {
                    Category = x.Category,
                    Date = x.Date,
                    Description = x.Description,
                    ShopItemName = x.ShopItemName,
                    GroupId = x.GroupId,
                    Id = x.Id,
                    MustPayTo = x.MustPayTo,
                    Owner = x.Owner,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Shop = x.Shop,
                    UnitPrice = x.UnitPrice,
                    WhoPaid = x.WhoPaid,
                }
            ).ToArrayAsync();

            return Ok(models);
        }

        public class PurchaseModel {
            public string Id { get; set; }
            public string Shop { get; set; }
            public DateTime Date { get; set; }
            public decimal Price { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public string ShopItemName { get; set; }
            public string Description { get; set; }
            public string GroupId { get; set; }


            public string? Owner { get; set; }
            public string? MustPayTo { get; set; }
            public string? Category { get; set; }
            public string? WhoPaid { get; set; }

        }

        /*
        [HttpGet]
        [Route("{id}", Name = "GetPurchase")]
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
        */
    }
}
