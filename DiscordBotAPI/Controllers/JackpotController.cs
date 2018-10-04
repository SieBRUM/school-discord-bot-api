using System.Web.Http;
using DiscordBotAPI.Services;
using System.Linq;
using TNSApi.Mapping;
using System;
using System.Collections.Generic;

namespace TNSApi.Controllers
{
    public class JackpotController : ApiController
    {
        IDatabaseServiceProvider _database;

        public JackpotController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [Route("api/gamble/updatejackpot")]
        [HttpPost]
        public IHttpActionResult UpdateJackpot(Jackpot jackpot)
        {
            if(jackpot.Points < 1)
            {
                // Error
                jackpot.Status = JackpotStatus.InvalidPoints;
                return Ok(jackpot);
            }

            var user = _database.Users.Where(x => x.DiscordId == jackpot.DiscordId).FirstOrDefault();
            if(user == null)
            {
                // Error
                jackpot.Status = JackpotStatus.UserDoesntExist;
                return Ok(jackpot);
            }

            if (jackpot.Points > user.Points)
            {
                // Error
                jackpot.Status = JackpotStatus.UserNotEnoughPoints;
                return Ok(jackpot);
            }

            var allJackpots = _database.Jackpot.ToList();
            long totalPoints = jackpot.Points;
            for (int i = 0; i < allJackpots.Count; i++)
            {
                totalPoints += allJackpots[i].Points;
            }

            var existingJackpot = _database.Jackpot.Where(x => x.UserId == user.Id).FirstOrDefault();
            // New
            if(existingJackpot == null)
            {
                jackpot.UserId = user.Id;
                _database.Jackpot.Add(jackpot);
                _database.Context.SaveChanges();
                jackpot.WinChancePercentage = (double)jackpot.Points / totalPoints * 100.00F;
                jackpot.TotalPoints = totalPoints;

                return Ok(jackpot);
            }
            // Existing
            else
            {
                existingJackpot.Points += jackpot.Points;
                existingJackpot.UserId = user.Id;
                existingJackpot.WinChancePercentage = (double)existingJackpot.Points / totalPoints * 100.00;
                existingJackpot.TotalPoints = totalPoints;
                _database.Context.SaveChanges();

                return Ok(existingJackpot);
            }
        }

        [Route("api/gamble/endjackpot")]
        [HttpGet]
        public IHttpActionResult EndJackpot()
        {
            var jackpots = _database.Jackpot.OrderByDescending(x => x.Points).ToList();
            Dictionary<int, long> keyValuePairs = new Dictionary<int, long>();

            for (int i = 0; i < jackpots.Count(); i++)
            {
                if (i == 0)
                    keyValuePairs.Add(jackpots[i].Id, jackpots[i].Points);
                else
                    keyValuePairs.Add(jackpots[i].Id, jackpots[i - 1].Points + jackpots[i].Points);
            }

            Random random = new Random();
            int randomNumber = random.Next(0, (int)keyValuePairs.Last().Value + 1);

            _database.Jackpot.RemoveRange(_database.Jackpot);
            _database.Context.SaveChanges();
            return Ok();
        }
    }
}