using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Home.Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    //[Produces("application/json")]
    [Authorize]
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

        [HttpGet]
        [Route("byGroup", Name = "ByGroup")]
        [Produces<PurchaseModel[]>]
        public async Task<IActionResult> ByGroup(string groupId) {

            var purchases = repository.List();
            var models = await purchases.Where(x => x.GroupId == groupId).Select(x =>
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

        [HttpGet]
        [Route("{id}", Name = "GetPurchase")]
        [Produces<PurchaseModel>]
        public async Task<IActionResult> GetPurchase(string id) {

            var purchase = await repository.Get(id);

            if (purchase == null) {
                return NotFound();
            }

            var model = new PurchaseModel() {
                Category = purchase.Category,
                Date = purchase.Date,
                Description = purchase.Description,
                ShopItemName = purchase.ShopItemName,
                GroupId = purchase.GroupId,
                Id = purchase.Id,
                MustPayTo = purchase.MustPayTo,
                Owner = purchase.Owner,
                Price = purchase.Price,
                Quantity = purchase.Quantity,
                Shop = purchase.Shop,
                UnitPrice = purchase.UnitPrice,
                WhoPaid = purchase.WhoPaid,
            };

            return Ok(model);
        }

        [HttpPatch]
        [Route("{id}/{category}", Name = "SetCategory")]
        public async Task<IActionResult> SetCategory(string id, string category) {

            var purchase = await repository.Get(id);

            if (purchase == null) {
                return NotFound();
            }

            purchase.SetCategory(category);

            return Ok();
        }

        [HttpPatch]
        [Route("{id}/{description}", Name = "SetDescription")]
        public async Task<IActionResult> SetDescription(string id, string description) {

            var purchase = await repository.Get(id);

            if (purchase == null) {
                return NotFound();
            }

            purchase.SetDescription(description);

            return Ok();
        }

        [HttpPatch]
        [Route("{id}/metadata", Name = "SetMetadata")]
        public async Task<IActionResult> SetDescription(string id, PurchaseMetadata metadata) {

            var purchase = await repository.Get(id);

            if (purchase == null) {
                return NotFound();
            }

            purchase.SetMetadata(metadata.Owner, metadata.MustPayTo, metadata.WhoPaid);

            return Ok();
        }

        public class PurchaseMetadata {
            public string? Owner { get; set; }
            public string? MustPayTo { get; set; }
            public string? WhoPaid { get; set; }
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
