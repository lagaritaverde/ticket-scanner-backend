using Home.Auth.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Home.Auth.Controllers {
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase {
        private readonly UserDbContext userDbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(UserDbContext userDbContext, ILogger<UserController> logger) {
            this.userDbContext = userDbContext;
            _logger = logger;
        }

        [HttpPut]
        [Route("AccountingGroup", Name = "SetAccountingGroup")]
        public async Task<IActionResult> SetAccountingGroup(AccountingGroup accountingGroup) {

            if (accountingGroup.AccoutingGroupId.Contains(":")) {
                return BadRequest("Group can't contina :");
            }

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await userDbContext.Users
                .SingleOrDefaultAsync(x => x.Id == userId);

            if (user == null) {
                return this.Problem();
            }

            if (user.AllowedAccountingGroup.Contains(accountingGroup.AccoutingGroupId)) {
                return Ok();
            }

            user.AllowedAccountingGroup.Add(accountingGroup.AccoutingGroupId);

            await userDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("AccountingGroup", Name = "GetAccountingGroup")]
        public async Task<IActionResult> GetAccountingGroup() {

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await userDbContext.Users
                .SingleOrDefaultAsync(x => x.Id == userId);

            if (user == null) {
                return this.Problem();
            }

            return Ok(user.AllowedAccountingGroup);
        }


        public class AccountingGroup {
            public string OwnerUserId { get; set; }
            public string AccoutingGroupId { get; set; }
        }
    }
}
