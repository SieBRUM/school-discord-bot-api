using System.Web.Http;
using DiscordBotAPI.Services;
using System.Linq;
using System;
using TNSApi.Mapping;

namespace TNSApi.Controllers
{
    public class JackpotController : ApiController
    {
        IDatabaseServiceProvider _database;

        public JackpotController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [Route("api/gamble/startjackpot")]
        [HttpPost]
        public IHttpActionResult CreateJackpot(Jackpot jackpot)
        {
            if(jackpot.Points < 1)
            {
                // Error
                jackpot.Status = JackpotStatus.InvalidPoints;
            }

            var user = _database.Users.Where(x => x.DiscordId == jackpot.DiscordId).FirstOrDefault();
            if(user == null)
            {
                // Error
                jackpot.Status = JackpotStatus.UserDoesntExist;
            }

            if (jackpot.Points > user.Points)
            {
                // Error
                jackpot.Status = JackpotStatus.UserNotEnoughPoints;
            }

            if(_database.Jackpot.Any())
            {
                jackpot.Status = JackpotStatus.JackpotAlreadyExists;
            }


            _database.Jackpot.Add(jackpot);
            return Ok();
        }

        [Route("api/gamble/endjackpackpot")]
        [HttpPost]
        public IHttpActionResult EndJackpot(Jackpot jackpot)
        {
            return Ok();
        }

        [Route("api/gamble/addtojackpot")]
        [HttpPost]
        public IHttpActionResult AddToJackpot(Jackpot jackpot)
        {
            return Ok();
        }
    }
}