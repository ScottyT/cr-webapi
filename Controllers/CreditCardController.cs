using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cr_app_webapi.Controllers
{
    [ApiController]
    [Route("api/credit-card")]
    [Authorize]
    public class CreditCardController : ControllerBase
    {
        private readonly IMongoRepo<CreditCard,CreditCard> _creditCard;
        public CreditCardController(IMongoRepo<CreditCard,CreditCard> creditCard)
        {
            _creditCard = creditCard;
        }

        [HttpGet]
        public List<CreditCard> Get() =>
            _creditCard.AsQueryable().ToList();

        [HttpGet("{cardNumber}")]
        public ActionResult<CreditCard> GetByCardNumber(string cardNumber)
        {
            var card = _creditCard.FilterBy(
                filter => filter.cardNumber == cardNumber,
                projection => new CreditCard
                {
                    cardholderInfo = projection.cardholderInfo,
                    cardholderName = projection.cardholderName,
                    cardholderZip = projection.cardholderZip,
                    cardNumber = projection.cardNumber,
                    creditCardCompany = projection.creditCardCompany,
                    billingAddressFirst = projection.billingAddressFirst,
                    billingAddressOther = projection.billingAddressOther,
                    customerSig = projection.customerSig,
                    customerSignDate = projection.customerSignDate
                }).FirstOrDefault();

            if (card is null)
            {
                return NotFound();
            }
            return card;
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddNewCreditCard([FromBody] CreditCard newCard)
        {
            string? cardnumber = newCard.cardNumber;
            if (cardnumber is null) return BadRequest();
            var card = GetByCardNumber(cardnumber);
            if (card.Value is not null)
            {
                return Ok(new {error = true, message = "Duplicate card numbers are not allowed."});
            }
            
            await _creditCard.InsertOneAsync(newCard);
            return CreatedAtAction(nameof(Get), "Successfully created the credit card!");
        }
    }
}