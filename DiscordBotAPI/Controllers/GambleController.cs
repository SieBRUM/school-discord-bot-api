using System.Web.Http;
using DiscordBotAPI.Services;
using System.Linq;
using System;

namespace DiscordBotAPI.Controllers
{
    public class GambleController : ApiController
    {
        IDatabaseServiceProvider _database;

        public GambleController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [Route("api/gamble/coinflip")]
        [HttpPost]
        public IHttpActionResult FlipCoin([FromBody]CoinflipUser coinflipUser)
        {
            var user = _database.Users.Where(x => x.DiscordId == coinflipUser.Id).FirstOrDefault();

            if(user == null)
            {
                return Ok("User does not exist.");
            }

            if(user.Points < coinflipUser.Points)
            {
                return Ok($"You dont have the points to gamble this much! ({string.Format("{0:C}", user.Points)} max)");
            }

            Random rnd = new Random();

            if(rnd.Next(1, 3) == coinflipUser.ChosenSide)
            {
                user.Points += coinflipUser.Points;
                _database.Context.SaveChanges();

                return Ok($"has won {coinflipUser.Points} and now has {string.Format("{0:C}", user.Points)}.");
            }
            else
            {
                user.Points -= coinflipUser.Points;
                _database.Context.SaveChanges();
                return Ok($"has lost {coinflipUser.Points} and now has {string.Format("{0:C}", user.Points)} left.");
            }
        }
    }

    public class CoinflipUser
    {
        public long Id;
        public int ChosenSide;
        public long Points;
    }
}