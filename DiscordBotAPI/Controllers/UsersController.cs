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

        [HttpPost]
        public IHttpActionResult SetUser([FromBody]frontendGivePoints frontendUser)
        {
            var user = _database.Users.Where(x => x.DiscordId == frontendUser.Id).FirstOrDefault();

            if (user == null)
            {
                user = new User();
                user.DiscordId = frontendUser.Id;
                user.Points = 250;
            }
            else
            {
                user.Points += frontendUser.points;
            }

            _database.Context.SaveChanges();

            return Ok(user);
        }
    }

    public class frontendGivePoints
    {
        public long Id;
        public long points;
    }
}