using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PortfolioService.Models;
using User_Data;

namespace PortfolioService.Controllers
{
    public class UserController : ApiController
    {
        private readonly UserDataRepository _userRepository;

        public UserController()
        {
            _userRepository = new UserDataRepository();
        }

        private static List<User> users = new List<User>
        {
            //new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
            //new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };



        // GET api/user
        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult GetUsers()
        {
            return Ok(users);
        }

        // GET api/user/1
        [HttpGet]
        [Route("api/user/{id}")]
        public IHttpActionResult GetUser(int id)
        {
            //var user = users.FirstOrDefault(u => u.Id == id);
            //if (user == null)
            {
                return NotFound();
            }
            //return Ok(user);
        }

        // POST api/user
        [HttpPost]
        [Route("api/user")]
        public IHttpActionResult PostUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Podaci o korisniku nisu pravilno poslani.");
            }

            Console.WriteLine("user.Id" + user.Id);
            Console.WriteLine("user.Name" + user.Name);
            Console.WriteLine("user.Email" + user.Email);

            try
            {

                var userData = new UserData(user.Email)
                {
                    FirstName = user.Name,
                    Email = user.Email
                };

                _userRepository.AddUser(userData);

                return Ok("Korisnik uspešno dodat.");

                //user.Id = users.Count() + 1;
                //user.Id = 70;
                users.Add(user);
                return Ok(users);
                //return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



    }
}
