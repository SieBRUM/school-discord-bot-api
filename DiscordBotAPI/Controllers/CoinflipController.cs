using System.Web.Http;
using DiscordBotAPI.Services;
using System.Linq;
using System;
using TNSApi.Mapping;

namespace DiscordBotAPI.Controllers
{
    public class CoinflipController : ApiController
    {
        IDatabaseServiceProvider _database;

        public CoinflipController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [Route("api/gamble/coinflip")]
        [HttpPost]
        public IHttpActionResult FlipCoin([FromBody]CoinflipResult coinflipUser)
        {
            var user = _database.Users.Where(x => x.DiscordId == coinflipUser.User.DiscordId).FirstOrDefault();
            CoinflipResult result = new CoinflipResult();

            if(user == null)
            { 
                return BadRequest();
            }

            if(user.Points < coinflipUser.User.Points)
            {
                result.Result = CoinflipResults.NoPoints;
                result.User = user;

                return Ok(result);
            }

            Random rnd = new Random();

            if(rnd.Next(0, 2) == coinflipUser.ChosenSide)
            {
                result.Result = CoinflipResults.Won;
                user.Points += coinflipUser.User.Points;
                user.BetsExecuted++;
                user.BetsWon++;
                _database.Context.SaveChanges();
                result.User = user;

                return Ok(result);
            }
            else
            {
                result.Result = CoinflipResults.Lost;
                user.Points -= coinflipUser.User.Points;
                user.BetsExecuted++;
                user.BetsLost++;
                _database.Context.SaveChanges();
                result.User = user;

                return Ok(result);
            }
        }

        [Route("api/gamble/coinflipvs")]
        [HttpPost]
        public IHttpActionResult FlipCoinBattle([FromBody]Coinflip coinflip)
        {
            var challenger = _database.Users.Where(x => x.DiscordId == coinflip.Challenger.DiscordId).FirstOrDefault();
            var enemy = _database.Users.Where(x => x.DiscordId == coinflip.Enemy.DiscordId).FirstOrDefault();

            if(challenger == null || enemy == null)
            {
                return BadRequest();
            }

            coinflip.ChallengerId = challenger.Id;
            coinflip.EnemyId = enemy.Id;
            coinflip.Challenger = challenger;
            coinflip.Enemy = enemy;

            var existingCoinFlip = _database.Coinflips.Where(x => x.ChallengerId == challenger.Id && x.EnemyId == enemy.Id).FirstOrDefault();

            if (existingCoinFlip != null)
            {
                existingCoinFlip.Result = CoinflipVsResults.ChallengeAlreadyExists;
                existingCoinFlip.Challenger = challenger;
                existingCoinFlip.Enemy = enemy;
                return Ok(existingCoinFlip);
            }

            if (challenger.Points < coinflip.Points)
            {
                coinflip.Result = CoinflipVsResults.ChallengerNoPoints;
                return Ok(coinflip);
            }

            if(enemy.Points < coinflip.Points)
            {
                // Error
                coinflip.Result = CoinflipVsResults.EnemyNoPoints;
                return Ok(coinflip);
            }

            coinflip.Result = CoinflipVsResults.ChallengeRequestSet;
            _database.Coinflips.Add(coinflip);
            _database.Context.SaveChanges();

            return Ok(coinflip);
        }

        [Route("api/gamble/acceptcoinflip")]
        [HttpPost]
        public IHttpActionResult AcceptCoinflip([FromBody]Coinflip coinflip)
        {
            var challenger = _database.Users.Where(x => x.DiscordId == coinflip.Challenger.DiscordId).FirstOrDefault();
            var enemy = _database.Users.Where(x => x.DiscordId == coinflip.Enemy.DiscordId).FirstOrDefault();

            if (challenger == null || enemy == null)
            {
                return BadRequest();
            }

            var existingCoinFlip = _database.Coinflips.Where(x => x.ChallengerId == challenger.Id && x.EnemyId == enemy.Id).FirstOrDefault();

            existingCoinFlip.ChallengerId = challenger.Id;
            existingCoinFlip.EnemyId = enemy.Id;
            existingCoinFlip.Challenger = challenger;
            existingCoinFlip.Enemy = enemy;

            if (existingCoinFlip == null)
            {
                // Error
                _database.Context.Entry(existingCoinFlip).State = System.Data.Entity.EntityState.Detached;
                existingCoinFlip = new Coinflip();
                existingCoinFlip.Result = CoinflipVsResults.ChallengeDoesntExist;
                existingCoinFlip.Challenger = challenger;
                existingCoinFlip.Enemy = enemy;
                return Ok(existingCoinFlip);
            }

            if (challenger.Points < existingCoinFlip.Points)
            {
                // Error
                existingCoinFlip.Result = CoinflipVsResults.ChallengerNoPoints;
                return Ok(existingCoinFlip);
            }

            if (enemy.Points < existingCoinFlip.Points)
            {
                // Error
                existingCoinFlip.Result = CoinflipVsResults.EnemyNoPoints;
                return Ok(existingCoinFlip);
            }

            Random rnd = new Random();

            if (rnd.Next(0, 2) == existingCoinFlip.Side)
            {
                existingCoinFlip.Result = CoinflipVsResults.ChallengerWon;
                challenger.Points += existingCoinFlip.Points;
                enemy.Points -= existingCoinFlip.Points;
                challenger.BetsExecuted++;
                enemy.BetsExecuted++;
                challenger.BetsWon++;
                enemy.BetsLost++;

                _database.Coinflips.Remove(existingCoinFlip);
                _database.Context.SaveChanges();

                return Ok(existingCoinFlip);
            }
            else
            {
                existingCoinFlip.Result = CoinflipVsResults.EnemyWon;
                challenger.Points -= existingCoinFlip.Points;
                enemy.Points += existingCoinFlip.Points;
                challenger.BetsExecuted++;
                enemy.BetsExecuted++;
                challenger.BetsLost++;
                enemy.BetsWon++;

                _database.Coinflips.Remove(existingCoinFlip);
                _database.Context.SaveChanges();

                return Ok(existingCoinFlip);
            }
        }

        [Route("api/gamble/declinecoinflip")]
        [HttpPost]
        public IHttpActionResult DeclineCoinflip([FromBody]Coinflip coinflip)
        {
            var challenger = _database.Users.Where(x => x.DiscordId == coinflip.Challenger.DiscordId).FirstOrDefault();
            var enemy = _database.Users.Where(x => x.DiscordId == coinflip.Enemy.DiscordId).FirstOrDefault();

            if (challenger == null || enemy == null)
            {
                return BadRequest();
            }

            var existingCoinFlip = _database.Coinflips.Where(x => x.ChallengerId == challenger.Id && x.EnemyId == enemy.Id).FirstOrDefault();

            if (existingCoinFlip == null)
            {
                // Error
                existingCoinFlip = new Coinflip();
                _database.Context.Entry(existingCoinFlip).State = System.Data.Entity.EntityState.Detached;
                existingCoinFlip.Result = CoinflipVsResults.ChallengeDoesntExist;
                existingCoinFlip.Challenger = challenger;
                existingCoinFlip.Enemy = enemy;
                return Ok(existingCoinFlip);
            }

            existingCoinFlip.Result = CoinflipVsResults.ChallengeDeclined;
            existingCoinFlip.Challenger = challenger;
            existingCoinFlip.Enemy = enemy;

            _database.Coinflips.Remove(existingCoinFlip);
            _database.Context.SaveChanges();

            return Ok(existingCoinFlip);
        }
    }
}