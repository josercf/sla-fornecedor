using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace sla_fornecedor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public static IDictionary<string, Account> _accounts;
        private static int _count = 0;

        public PaymentController()
        {
            if (_accounts == null)
                _accounts = new Dictionary<string, Account>();
        }

        [HttpPost]
        public IActionResult Post([FromBody] PaymentModel payment)
        {
            _count++;
            if (!_accounts.ContainsKey(payment.Account))
            {
                _accounts.Add(payment.Account, new Account { Number = payment.Account, Balance = 0 });
            }

            _accounts[payment.Account].Balance += payment.Ammount;

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
            if (!_accounts.ContainsKey(account))
            {
                return NotFound();
            }

            return Ok(_accounts[account]);
        }
    }

    public class Account
    {
        public string Number { get; set; }
        public decimal Balance { get; set; }
    }


    public class PaymentModel
    {
        public string Account { get; set; }
        public decimal Ammount { get; set; }
    }
}
