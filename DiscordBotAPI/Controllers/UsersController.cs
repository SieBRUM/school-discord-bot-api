using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DiscordBotAPI.Mapping;
using DiscordBotAPI.Services;
using TNSApi.Mapping;

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
        [Route("api/pay")]
        public IHttpActionResult PayPerson([FromBody] Coinflip request)
        {
            var donator = _database.Users.Where(x => x.DiscordId == request.Challenger.DiscordId).FirstOrDefault();
            var receiver = _database.Users.Where(x => x.DiscordId == request.Enemy.DiscordId).FirstOrDefault();

            if(donator == null)
            {
                request.Result = CoinflipVsResults.ChallengeDoesntExist;
                return Ok(request);
            }

            if (receiver == null)
            {
                request.Result = CoinflipVsResults.EnemyWon;
                return Ok(request);
            }

            if (donator.Points < request.Points)
            {
                request.Challenger = donator;
                request.Enemy = receiver;
                request.Result = CoinflipVsResults.ChallengerNoPoints;
                return Ok(request);
            }

            request.Result = CoinflipVsResults.ChallengerWon;
            donator.Points -= request.Points;
            receiver.Points += request.Points;
            _database.Context.SaveChanges();

            request.Challenger = donator;
            request.Enemy = receiver;

            return Ok(request);
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