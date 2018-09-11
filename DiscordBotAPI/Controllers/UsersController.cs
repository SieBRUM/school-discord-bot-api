using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DiscordBotAPI.Mapping;
using DiscordBotAPI.Services;

namespace DiscordBotAPI.Controllers
{
    public class UsersController : ApiController
    {
        IDatabaseServiceProvider _database;

        public UsersController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            List<User> users = _database.Users.ToList();

            return Ok(users);
        }

        [HttpGet]
        public IHttpActionResult GetUser(long Id)
        {
            User user = _database.Users.Where(x => x.DiscordId == Id).FirstOrDefault();

            if(user == null)
            {
                user = new User();
                user.DiscordId = Id;
                user.Points = 250;
                _database.Users.Add(user);
                _database.Context.SaveChanges();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("api/highscore")]
        public IHttpActionResult GetHighscore()
        {
            var users = _database.Users.OrderByDescending(x => x.Points).ToList();
            users.OrderBy(x => x.Points);
            users = users.GetRange(0, 5);

            return Ok(users);
        }

        [HttpPost]
        public IHttpActionResult SetUser([FromBody]User frontendUser)
        {
            var user = _database.Users.Where(x => x.DiscordId == frontendUser.DiscordId).FirstOrDefault();

            if (user == null)
            {
                user = new User();
                user.DiscordId = frontendUser.DiscordId;
                user.Points = 250;
            }
            else
            {
                user.Points += frontendUser.Points;
            }

            _database.Context.SaveChanges();

            return Ok(user);
        }
    }
}