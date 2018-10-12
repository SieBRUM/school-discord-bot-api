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

            jackpot.User = user;

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
            user.Points -= jackpot.Points;
            // New
            if(existingJackpot == null)
            {
                jackpot.UserId = user.Id;
                _database.Jackpot.Add(jackpot);
                _database.Context.SaveChanges();
                jackpot.WinChancePercentage = Math.Round((double)jackpot.Points / totalPoints * 100.00F, 2);
                jackpot.TotalPoints = totalPoints;

                return Ok(jackpot);
            }
            // Existing
            else
            {
                existingJackpot.Points += jackpot.Points;
                existingJackpot.UserId = user.Id;
                existingJackpot.User = user;
                existingJackpot.WinChancePercentage = Math.Round((double)existingJackpot.Points / totalPoints * 100.00F, 2);
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
            if(jackpots.Count() < 2)
            {
                // User one gets money and function returns
                var id = jackpots[0].UserId;
                var user = _database.Users.Where(x => x.Id == id).FirstOrDefault();
                user.Points += jackpots[0].Points;
                jackpots[0].User = user;
                jackpots[0].WinChancePercentage = 100;
                jackpots[0].TotalPoints = jackpots[0].Points;
                _database.Jackpot.RemoveRange(_database.Jackpot);
                _database.Context.SaveChanges();
                return Ok(jackpots[0]);
            }

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

            int wonIndex = 0;

            for (int i = 0; i < keyValuePairs.Count(); i++)
            {
                if(i == 0)
                {
                    if(keyValuePairs.ElementAt(i + 1).Value < randomNumber)
                    {
                        // User 1 won!
                        wonIndex = i;
                    }
                }
                else if(i == keyValuePairs.Count())
                {
                    if(keyValuePairs.ElementAt(i - 1).Value < randomNumber && keyValuePairs.ElementAt(i + 1).Value > randomNumber)
                    {
                        // User won!
                        wonIndex = i;
                    }
                }
                else
                {
                    if(keyValuePairs.ElementAt(i - 1).Value < randomNumber)
                    {
                        // Last user won!
                        wonIndex = i;
                    }
                }
            }

            var userId = jackpots[wonIndex].UserId;
            var winner = _database.Users.Where(x => x.Id == userId).FirstOrDefault();
            winner.Points += keyValuePairs.Last().Value;
            _database.Jackpot.RemoveRange(_database.Jackpot);
            _database.Context.SaveChanges();
            jackpots[wonIndex].User = winner;
            jackpots[wonIndex].WinChancePercentage = Math.Round(((double)jackpots.Where(x => x.UserId == userId).FirstOrDefault().Points / keyValuePairs.Last().Value) * 100.00F, 2);

            jackpots[wonIndex].TotalPoints = keyValuePairs.Last().Value;
            return Ok(jackpots[wonIndex]);
        }
    }
}