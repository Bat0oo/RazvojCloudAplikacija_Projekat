using Microsoft.WindowsAzure.Storage;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using User_Data;

namespace PortfolioService.Controllers
{
    public class RegisterController : ApiController
    {
        private readonly UserDataRepository _userRepository;

        //UserDataRepository repo = new UserDataRepository();

        public RegisterController(UserDataRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public RegisterController()
        {
            _userRepository = new UserDataRepository();
        }


        [HttpPost]
        [Route("api/register")]
        public HttpResponseMessage Register([FromBody] UserData user)
        {
            try
            {
                Console.WriteLine("firstName: " + user.FirstName);
                Console.WriteLine("lastName: " + user.LastName);
                Console.WriteLine("password: " + user.Password);
                Console.WriteLine("email: " + user.Email);
                Console.WriteLine("country: " + user.Country);

                if (_userRepository.Exists(user.Email))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Korisnik sa istim emailom već postoji.");
                }

                UserData newUser = new UserData(user.Email)
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    Email = user.Email,
                    Address = "",
                    City = "",
                    Country = "",
                    PhoneNumber = "",
                    ProfilePicture = ""
                };

                _userRepository.AddUser(newUser);

                var allUsers = _userRepository.RetrieveAllUsers();
                foreach (var u in allUsers)
                {
                    Console.WriteLine($"User in table: {u.Email}");
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Registracija uspješna.");
            }
            catch (StorageException ex)
            {
                Console.WriteLine($"Azure Storage Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Greška pri dodavanju korisnika u bazu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



    }
}
