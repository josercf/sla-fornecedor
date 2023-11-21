using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace sla_fornecedor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private static int _count = 0;

        public TransferController()
        {
            if(PaymentController._accounts == null)
            PaymentController._accounts = new Dictionary<string, Account>();
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransferModel transfer)
        {
            _count++;
            if (!PaymentController._accounts.ContainsKey(transfer.Account))
            {
                return BadRequest($"Transfer account {transfer.Account} doesn't exists");
            }

            if (!PaymentController._accounts.ContainsKey(transfer.DestinationAccount))
            {
                return BadRequest($"Destination account {transfer.DestinationAccount} doesn't exists");
            }


            if(PaymentController._accounts[transfer.Account].Balance >= transfer.Ammount)
            {
                PaymentController._accounts[transfer.Account].Balance -= transfer.Ammount;
                PaymentController._accounts[transfer.DestinationAccount].Balance += transfer.Ammount;
            }
            else
            {
                return BadRequest($"Account {transfer.Account} doesn't have enough balance");
            }

            if (_count % 2 == 0)
            {
                Thread.Sleep(_count * 200);
                return Ok();
            }

            if (_count % 3 == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpGet]
        public ActionResult<Account> Get(string account)
        {
            if (!PaymentController._accounts.ContainsKey(account))
            {
                return NotFound();
            }

            return Ok(PaymentController._accounts[account]);
        }
    }

    public class TransferModel
    {
        public string Account { get; set; }
        public string DestinationAccount { get; set; }
        public decimal Ammount { get; set; }
    }
}
