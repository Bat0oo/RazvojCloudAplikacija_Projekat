using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using User_Data;

namespace PortfolioService.Controllers
{
    public class AuthController : ApiController
    {
        private readonly UserDataRepository _userRepository;

        public AuthController(UserDataRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public AuthController()
        {
            _userRepository = new UserDataRepository();
        }

        /*
        private string UploadImageToBlob(HttpPostedFileBase file)
        {
            var storageAccount = CloudStorageAccount.Parse("YourAzureStorageConnectionString");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("userimages");
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            blob.Properties.ContentType = file.ContentType;
            blob.UploadFromStream(file.InputStream);
            return blob.Uri.ToString();
        }
        */

        [HttpGet]
        [Route("api/getUser")]
        public HttpResponseMessage GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Greska prilikom vracanja korisnika.");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Uspesno vracen korisnik.");
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

                return Request.CreateResponse(HttpStatusCode.OK, "Registracija uspešna.");
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

        [HttpPost]
        [Route("api/login")]
        public HttpResponseMessage Login([FromBody] UserCredentialsDTO credentials)
        {
            try
            {
                if (credentials == null || string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Nevalidni kredencijali.");
                }

                UserData user = _userRepository.GetUserByEmail(credentials.Email);
                if (user == null || user.Password != credentials.Password)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Nevalidna email adresa ili lozinka.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Prijava uspješna.");
            }
            catch (StorageException ex)
            {
                Console.WriteLine($"Azure Storage Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Greška pri prijavi korisnika.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut]
        [Route("api/editProfile")]
        public HttpResponseMessage UpdateUser(string email, UserData user)
        {
            var existingUser = _userRepository.GetUser(email);
            if (existingUser == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Greska");
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                existingUser.FirstName = user.FirstName;
            }
            if (!string.IsNullOrEmpty(user.LastName))
            {
                existingUser.LastName = user.LastName;
            }
            if (!string.IsNullOrEmpty(user.Address))
            {
                existingUser.Address = user.Address;
            }
            if (!string.IsNullOrEmpty(user.City))
            {
                existingUser.City = user.City;
            }
            if (!string.IsNullOrEmpty(user.Country))
            {
                existingUser.Country = user.Country;
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                existingUser.PhoneNumber = user.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                existingUser.ProfilePicture = user.ProfilePicture;
            }

            _userRepository.UpdateUser(existingUser);
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Uspesna izmena korisnika.");
        }

    }
}
