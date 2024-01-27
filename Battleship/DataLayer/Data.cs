using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Data
    {
        //private BattleshipDataContext db = new BattleshipDataContext(); 

        public Data() { }



        public IQueryable<Player> checkifplayerexists(string username)
        {
            using (var bscontext = new BattleshipDataContext()) // This is a using statement, which ensures that the context is disposed of when we're done with it//why//
            {
                var player = bscontext.Players.Where(p => p.Username == username);
                return player;
            }
        }

        public IQueryable<Player> confirmpassword(string username, string password)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                var player = bscontext.Players.Where(p => p.Username == username && p.Password == password);
                return player;
            }
        }
        public void addplayertodb(string username, string password)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                Player player = new Player();
                player.Username = username;
                player.Password = password;
                bscontext.Players.InsertOnSubmit(player);
                bscontext.SubmitChanges();
            }
        }
        public int addgametodb(string title, bool complete, string creator, string opponent)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                Game game = new Game();
                game.Title = title;
                game.Complete = complete;
                game.CreatorFK = creator;
                game.OpponentFK = opponent;
                bscontext.Games.InsertOnSubmit(game);
                bscontext.SubmitChanges();

                // Return the ID of the newly created game
                return game.ID;
            }
        }

        public IQueryable<Ship> getshipsfromdb()
        {
            using (var bscontext = new BattleshipDataContext())
            {
                var ships = bscontext.Ships;
                return ships;
            }
        }
        public Ship GetShipById(int shipId)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                return bscontext.Ships.FirstOrDefault(ship => ship.ID == shipId);
            }
        }
        public List<Ship> GetUnconfiguredShips(int gameId, string playerUsername)
        {
            // Fetch ships that are not configured by the specific player in the current game
            using (var bscontext = new BattleshipDataContext())
            {
                var unconfiguredShips = bscontext.Ships
                .Where(ship => !bscontext.GameShipConfigurations
                    .Any(conf => conf.ShipFK == ship.ID && conf.GameFK == gameId && conf.PlayerFK == playerUsername))
                .ToList();

                return unconfiguredShips;
            }
        }

        public void MarkShipAsConfigured(int shipId, int gameId, string playerUsername, string coordinate)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                GameShipConfiguration newConfig = new GameShipConfiguration
                {
                    ShipFK = shipId,
                    GameFK = gameId,
                    PlayerFK = playerUsername,
                    Coordinate = coordinate
                };

                bscontext.GameShipConfigurations.InsertOnSubmit(newConfig);
                bscontext.SubmitChanges();
            }
        }
        public List<Game> GetOngoingGames()
        {
            using (var bscontext = new BattleshipDataContext())
            {
                return bscontext.Games.Where(g => !g.Complete).ToList();
            }
        }
        public void MarkGameAsComplete(int gameId)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                var game = bscontext.Games.FirstOrDefault(g => g.ID == gameId);
                if (game != null)
                {
                    game.Complete = true;
                    bscontext.SubmitChanges();
                }
            }
        }
        public void ClearGameShipConfigurations(int gameId)
        {
            using (var bscontext = new BattleshipDataContext())
            {
                var configurations = bscontext.GameShipConfigurations.Where(config => config.GameFK == gameId);
                bscontext.GameShipConfigurations.DeleteAllOnSubmit(configurations);
                bscontext.SubmitChanges();
            }
        }
        public Ship GetShipAtCoordinates(string playerUsername, string coordinates, int gameId)
        {
            using (var context = new BattleshipDataContext())
            {
                // Assuming GameShipConfiguration links a Ship with its location for a specific game
                return context.GameShipConfigurations
                              .Where(c => c.PlayerFK == playerUsername && c.Coordinate == coordinates && c.GameFK == gameId)
                              .Select(c => c.Ship)
                              .FirstOrDefault();
            }
        }

        public bool DecrementShipHitpoints(int shipId)
        {
            using (var context = new BattleshipDataContext())
            {
                var ship = context.Ships.FirstOrDefault(s => s.ID == shipId);
                if (ship != null && ship.Size > 0)
                {
                    ship.Size--; // Decrement hitpoints
                    context.SubmitChanges();
                    return ship.Size == 0; // Return true if ship is destroyed
                }
                return false;
            }
        }

        public void RecordAttack(int gameId, string attackerUsername, string coordinates, bool isHit)
        {
            using (var context = new BattleshipDataContext())
            {
                var attack = new Attack
                {
                    GameFK = gameId,
                    UserNameFK = attackerUsername,
                    Coordinate = coordinates,
                    Hit = isHit
                };
                context.Attacks.InsertOnSubmit(attack);
                context.SubmitChanges();
            }
        }



    }
}