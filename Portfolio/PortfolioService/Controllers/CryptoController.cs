using Crypto_Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PortfolioService.Controllers
{
    public class CryptoController : ApiController
    {
        private readonly CryptoDataRepository _cryptoRepository;

        public CryptoController(CryptoDataRepository cryptoRepository)
        {
            _cryptoRepository = cryptoRepository ?? throw new ArgumentNullException(nameof(cryptoRepository));
        }

        public CryptoController()
        {
            _cryptoRepository = new CryptoDataRepository();
        }

        [HttpGet]
        [Route("api/crypto")]
        public IHttpActionResult GetAllCryptos(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email is required.");
            }

            var cryptos = _cryptoRepository.RetrieveAllCryptos(userEmail);
            return Ok(cryptos);
        }


        [HttpPost]
        [Route("api/crypto")]
        public HttpResponseMessage AddCrypto([FromBody] Crypto crypto)
        {
            try
            {
                Console.WriteLine("UserEmail: " + crypto.UserEmail);
                Console.WriteLine("Name: " + crypto.Name);
                Console.WriteLine("Current price: " + crypto.CurrentPrice);

                Crypto newCrypto = new Crypto(crypto.Symbol) { UserEmail = crypto.UserEmail, Name = crypto.Name, CurrentPrice = crypto.CurrentPrice, Symbol = crypto.Symbol, Amount = crypto.Amount};

                _cryptoRepository.AddCrypto(newCrypto);
                return Request.CreateResponse(HttpStatusCode.OK, "Crypto added successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
