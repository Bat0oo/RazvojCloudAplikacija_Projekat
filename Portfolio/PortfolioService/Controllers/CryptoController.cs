using Crypto_Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TransactionHistory_Data;

namespace PortfolioService.Controllers
{
    public class CryptoController : ApiController
    {
        private readonly CryptoDataRepository _cryptoRepository;
        private readonly TransactionHistoryRepository _transactionRepository;

        public CryptoController(CryptoDataRepository cryptoRepository, TransactionHistoryRepository transactionRepository)
        {
            _cryptoRepository = cryptoRepository ?? throw new ArgumentNullException(nameof(cryptoRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public CryptoController()
        {
            _cryptoRepository = new CryptoDataRepository();
            _transactionRepository = new TransactionHistoryRepository();
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

                Crypto newCrypto = new Crypto(crypto.Symbol) { UserEmail = crypto.UserEmail, Name = crypto.Name, CurrentPrice = crypto.CurrentPrice, Symbol = crypto.Symbol, Amount = crypto.Amount, TransactionDate = crypto.TransactionDate};

                _cryptoRepository.AddCrypto(newCrypto);

                var transaction = new TransactionHistory(crypto.UserEmail, Guid.NewGuid().ToString())
                {
                    CryptoSymbol = crypto.Symbol,
                    TransactionDate = crypto.TransactionDate,
                    Amount = crypto.Amount,
                    Price = crypto.CurrentPrice,
                    TotalValue = crypto.Amount * crypto.CurrentPrice,
                    IsPurchase = false,
                    Buy = true
                };
                _transactionRepository.AddTransaction(transaction);

                return Request.CreateResponse(HttpStatusCode.OK, "Crypto added successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [Route("api/crypto/buy")]
        public HttpResponseMessage BuyCrypto([FromBody] Crypto crypto)
        {
            try
            {
                var existingCrypto = _cryptoRepository.GetCryptoBySymbol(crypto.UserEmail, crypto.Symbol);
                if (existingCrypto == null)
                {
                    // Ako kriptovaluta ne postoji, dodajemo je kao novu
                    existingCrypto = new Crypto(crypto.Symbol) { UserEmail = crypto.UserEmail, Name = crypto.Name, CurrentPrice = crypto.CurrentPrice, TransactionDate = crypto.TransactionDate };
                    _cryptoRepository.AddCrypto(existingCrypto);

                    // Dodajemo transakciju u istoriju
                    var transaction = new TransactionHistory(crypto.UserEmail, Guid.NewGuid().ToString())
                    {
                        CryptoSymbol = crypto.Symbol,
                        TransactionDate = crypto.TransactionDate,
                        Amount = crypto.Amount,
                        Price = crypto.CurrentPrice,
                        TotalValue = crypto.Amount * crypto.CurrentPrice,
                        IsPurchase = true,
                        Buy=true
                    };
                    _transactionRepository.AddTransaction(transaction);
                }
                else
                {
                    // Ako kriptovaluta već postoji, samo ažuriramo količinu
                    existingCrypto.Amount += crypto.Amount;
                    _cryptoRepository.UpdateCrypto(existingCrypto);

                    // Dodajemo transakciju u istoriju
                    var transaction = new TransactionHistory(crypto.UserEmail, Guid.NewGuid().ToString())
                    {
                        CryptoSymbol = crypto.Symbol,
                        TransactionDate = DateTime.UtcNow,
                        Amount = crypto.Amount,
                        Price = crypto.CurrentPrice,
                        TotalValue = crypto.Amount * crypto.CurrentPrice,
                        IsPurchase = true,
                        Buy=true
                    };
                    _transactionRepository.AddTransaction(transaction);
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Crypto bought successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [HttpPost]
        [Route("api/crypto/sell")]
        public HttpResponseMessage SellCrypto([FromBody] Crypto crypto)
        {
            try
            {
                var existingCrypto = _cryptoRepository.GetCryptoBySymbol(crypto.UserEmail, crypto.Symbol);
                if (existingCrypto == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Crypto not found.");
                }

                if (existingCrypto.Amount < crypto.Amount)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not enough crypto to sell.");
                }

                existingCrypto.Amount -= crypto.Amount;
                _cryptoRepository.UpdateCrypto(existingCrypto);

                var transaction = new TransactionHistory(crypto.UserEmail, Guid.NewGuid().ToString())
                {
                    CryptoSymbol = crypto.Symbol,
                    TransactionDate = DateTime.UtcNow,
                    Amount = crypto.Amount,
                    Price = crypto.CurrentPrice,
                    TotalValue = crypto.Amount * crypto.CurrentPrice,
                    IsPurchase = false,
                    Sell = true
                };
                _transactionRepository.AddTransaction(transaction);

                return Request.CreateResponse(HttpStatusCode.OK, "Crypto sold successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/crypto/transactions")]
        public IHttpActionResult GetTransactionHistory(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email is required.");
            }

            var transactions = _transactionRepository.GetTransactionsByUserEmail(userEmail);

            return Ok(transactions);
        }

        [HttpDelete]
        [Route("api/crypto")]
        public HttpResponseMessage DeleteCrypto(string userEmail, string symbol)
        {
            try
            {
                _cryptoRepository.DeleteCrypto(userEmail, symbol);
                return Request.CreateResponse(HttpStatusCode.OK, "Crypto deleted successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
