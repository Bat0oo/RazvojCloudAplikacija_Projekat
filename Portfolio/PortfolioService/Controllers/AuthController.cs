using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
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

        [HttpGet]
        [Route("api/getUser")]
        public HttpResponseMessage GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Korisnik nije pronađen.");
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, user);

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<HttpResponseMessage> Register()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var profilePicture = httpRequest.Files["profilePicture"];

                if (profilePicture == null || profilePicture.ContentLength == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Slika nije dostupna ili prazna.");
                }

                // Upload slike u blob servis
                var profilePictureUrl = await UploadImageToBlobAsync(profilePicture, httpRequest["email"]);

                // Kreiranje novog korisnika sa podacima iz zahteva
                //var user = new UserData
                //{
                //    FirstName = httpRequest["firstName"],
                //    LastName = httpRequest["lastName"],
                //    Password = httpRequest["password"],
                //    RowKey = httpRequest["email"],
                //    Address = httpRequest["address"],
                //    City = httpRequest["city"],
                //    Country = httpRequest["country"],
                //    PhoneNumber = httpRequest["phoneNumber"],
                //    ProfilePicture = profilePictureUrl
                //};


                var user = new UserData(httpRequest["firstName"], httpRequest["lastName"], httpRequest["email"], httpRequest["address"], httpRequest["city"], httpRequest["country"], httpRequest["phoneNumber"], httpRequest["password"], profilePictureUrl);

                // Provera da li korisnik sa istim emailom već postoji
                if (_userRepository.Exists(user.RowKey))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Korisnik sa istim emailom već postoji.");
                }

                // Dodavanje novog korisnika u bazu podataka
                _userRepository.AddUser(user);

                return Request.CreateResponse(HttpStatusCode.OK, "Registracija uspešna.");
            }
            catch (StorageException ex)
            {
                Trace.WriteLine($"Azure Storage Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Greška pri dodavanju korisnika u bazu.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"General Exception: {ex.Message}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<string> UploadImageToBlobAsync(HttpPostedFile file, string email)
        {
            string uniqueBlobName = string.Format("image_{0}", email.Replace("/", ""));
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobStorage.GetContainerReference("userimages");

            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine($"StorageException during container creation: {ex.Message}");
                throw;
            }

            CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
            blob.Properties.ContentType = file.ContentType;

            try
            {
                byte[] fileBytes;
                using (var fileStream = file.InputStream)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }

                Console.WriteLine("Uploading file to blob...");
                await blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
                Console.WriteLine("File uploaded successfully.");
            }
            catch (StorageException ex)
            {
                Console.WriteLine($"StorageException during upload: {ex.Message}");
                Console.WriteLine($"Request Information: {ex.RequestInformation}");
                if (ex.RequestInformation.ExtendedErrorInformation != null)
                {
                    Console.WriteLine($"Error Code: {ex.RequestInformation.ExtendedErrorInformation.ErrorCode}");
                    Console.WriteLine($"Error Message: {ex.RequestInformation.ExtendedErrorInformation.ErrorMessage}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during upload: {ex.Message}");
                throw;
            }

            return blob.Uri.ToString();
        }


        /*
        private string UploadImageToBlob(HttpPostedFile file)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("userimages");
            container.CreateIfNotExists();

            var permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(permissions);

            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            blob.Properties.ContentType = file.ContentType;

            using (var fileStream = file.InputStream)
            {
                blob.UploadFromStream(fileStream);
            }

            return blob.Uri.ToString();
        }
        */


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
            var existingUser = _userRepository.GetUserByEmail(email);
            if (existingUser == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Korisnik nije pronađen.");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;
            existingUser.City = user.City;
            existingUser.Country = user.Country;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.ProfilePicture = user.ProfilePicture;

            _userRepository.UpdateUser(existingUser);

            var response = Request.CreateResponse(HttpStatusCode.OK, "Uspešna izmena korisnika");

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }


    }
}
