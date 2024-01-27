using DataLayer; 
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PresentationLayer.Presentation;

namespace PresentationLayer
{
    public enum GameState
    {
        Initial,
        PlayersAdded,
        Player1ConfiguringShips,
        Player2ConfiguringShips,
        ShipsConfigured,
    }
    public class Presentation
    {

       
        private GameState gameState = GameState.Initial;
        private Logic logic = new Logic();
        private string currentPlayer;
        private string opponentPlayer;
        private GameScreen gameScreenp1 = new GameScreen(new List<GameScreen.Cell>());
        private GameScreen gameScreenp2 = new GameScreen(new List<GameScreen.Cell>());
        private GameScreen currentGameScreen; //currentGameScreen: This variable is used to keep track of which player's game screen is currently active. 
        public Presentation() 
        {
            // Initialize gameScreen with an empty list of cells
            

        }
        private int currentGameId;
        public string username;
        public string username2;
       

        public void Start()
        {
            Console.Clear();
            
            ShowMenu();
        }
        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Battleship!");
                Console.WriteLine("Please select an option:");

                switch (gameState)
                {
                    case GameState.Initial:
                        Console.WriteLine("1. Add Player Details");
                        Console.WriteLine("4. Quit");
                        break;
                    case GameState.PlayersAdded:
                        Console.WriteLine("2. Configure Ships");
                        Console.WriteLine("4. Quit");
                        break;
                    case GameState.ShipsConfigured:
                        Console.WriteLine("3. Launch Attack");
                        Console.WriteLine("4. Quit");
                        break;
                }

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        if (gameState == GameState.Initial)
                        {
                            addplayer();
                            gameState = GameState.PlayersAdded;
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "2":
                        if (gameState == GameState.PlayersAdded)
                        {
                            Player1Shipchoice();
                            Player2ShipChoice();
                            gameState = GameState.ShipsConfigured; 
                        }
                       
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "3":
                        if (gameState == GameState.ShipsConfigured)
                        {
                           LaunchAttack();
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "4":
                        Quit();
                        return;

                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        private void addplayer()
        {
            addPlayer1();
            addPlayer2();
            addtitle();
        }
        private void addPlayer1()
        {
            Console.Clear();
            Console.WriteLine("Add Player 1 details:");
            Console.WriteLine("Please enter a username:");
            username = Console.ReadLine();

            if (logic.checkifplayerexists(username))
            {
                while (true)
                {
                    Console.WriteLine($"Welcome back {username}, Please enter your password or press Esc to exit:");
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        Quit();
                        return;
                    }

                    // Read the password (concatenating the first character if not Escape)
                    string password = keyInfo.KeyChar + Console.ReadLine();

                    if (logic.confirmpassword(username, password))
                    {
                        Console.WriteLine("Login successful");
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Password incorrect. Please try again.");
                    }
                }
            }
            else
            {

                Console.WriteLine("Username not found.");
                Console.WriteLine("Please enter a password to sign up.");
                string password = Console.ReadLine();
                logic.addplayer(username, password);
                Console.WriteLine("Player has been successfully added to the database!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

            }
        }
        private void addPlayer2()
        {
            Console.Clear();
            Console.WriteLine("Add Player 2 details:");
            Console.WriteLine("Please enter a username:");
            username2 = Console.ReadLine();

            if (logic.checkifplayerexists(username2))
            {
                while (true)
                {
                    Console.WriteLine($"Welcome back {username2}, Please enter your password or press Esc to exit:");
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        Quit();
                        return;
                    }


                    string password2 = keyInfo.KeyChar + Console.ReadLine();

                    if (logic.confirmpassword(username2, password2))
                    {
                        Console.WriteLine("Login successful");
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Password incorrect. Please try again.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Username not found.");
                Console.WriteLine("Please enter a password to sign up.");
                string password2 = Console.ReadLine();
                logic.addplayer(username2, password2);
                Console.WriteLine("Player has been successfully added to the database!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        private void Quit()
        {
            Console.WriteLine("Thank you for playing Battleship!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void addtitle()
        {
            // Check and handle ongoing games
            HandleOngoingGames();

            // Proceed with adding a new game
            bool complete = false;
            Console.WriteLine("Please enter the title of the game:");
            string title = Console.ReadLine();

            // Store the returned game ID in currentGameId
            currentGameId = logic.addgame(title, complete, username, username2);
            Console.WriteLine("Game has been added to the database.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void HandleOngoingGames()
        {
            var ongoingGames = logic.GetOngoingGames();
            if (ongoingGames.Any())
            {
                foreach (var game in ongoingGames)
                {
                    // Mark each ongoing game as complete and clear its configurations
                    logic.MarkGameAsComplete(game.ID);
                    logic.ClearGameShipConfigurations(game.ID);
                }
            }
        }


        public void Player1Shipchoice()
        {
            gameScreenp1.printgrid();
            
            while (true)
            {
                var unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, username);
                if (!unconfiguredShips.Any())
                {
                    Console.WriteLine("All ships have been configured!");
                    Console.WriteLine("Congratulations, Player 1, all your ships have been placed.");
                    Console.WriteLine("Now it's Player 2's turn to configure their ships.");
                    Console.WriteLine("Press any key to continue to Player 2's configuration...");
                    Console.ReadKey();
                    // Ensuring that we move to Player 2's configuration phase.
                    gameState = GameState.PlayersAdded;
                    Player2ShipChoice(); // Call Player 2's configuration method directly.
                    break;
                }

                Console.Clear();
                Console.WriteLine("Player 1, this is your board:");
                gameScreenp1.printgrid();

                Console.WriteLine("Please select a ship by ID:");
                foreach (var ship in unconfiguredShips)
                {
                    Console.WriteLine($"ID: {ship.ID}, Name: {ship.Title}");
                }

                int shipId;
                while (!int.TryParse(Console.ReadLine(), out shipId) || !unconfiguredShips.Any(ship => ship.ID == shipId))
                {
                    Console.WriteLine("Invalid ship ID. Please enter a valid ID:");
                }

                bool validPlacement = false;
                while (!validPlacement)
                {
                    char orientation;
                    do
                    {
                        Console.WriteLine("Enter orientation (V for vertical, H for horizontal):");
                        orientation = Console.ReadLine().ToUpper()[0];
                    } while (orientation != 'V' && orientation != 'H');

                    char startRowChar;
                    int startRow;
                    do
                    {
                        Console.WriteLine("Enter starting row (A, B, C, etc.):");
                        startRowChar = Console.ReadLine().ToUpper()[0];
                        startRow = startRowChar - 'A' + 1; // +1 is removed
                    } while (startRow < 1 || startRow > GameScreen.height); // Adjusted condition

                    int startColumn;
                    do
                    {
                        Console.WriteLine("Enter starting column (1, 2, 3, etc.):");
                    } while (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width); //why startcolumn < 1?//

                    string coordinate = ConvertToCoordinateString(startRow, startColumn);
                    if (CanPlaceShip(gameScreenp1, shipId, orientation, startRow, startColumn, username))
                    {
                        validPlacement = true;
                        Ship selectedShip = logic.GetShipById(shipId);
                        gameScreenp1.PlaceShipInGrid(startRow, startColumn, selectedShip.Size, orientation);
                        logic.MarkShipAsConfigured(shipId, currentGameId, username, coordinate);

                        // Re-fetch the list of unconfigured ships
                        unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, username);

                        Console.Clear();
                        Console.WriteLine("Ship placed successfully. Here's the updated board:");
                        gameScreenp1.printgrid();
                    }
                    else
                    {
                        Console.WriteLine("You cannot overlay ships, select different coordinates.");
                    }
                }
            }

        }
        public void Player2ShipChoice()
        {
            gameScreenp2.printgrid();

            while (true)
            {
                var unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, username2);
                if (!unconfiguredShips.Any())
                {
                    Console.WriteLine("All ships have been configured!");
                    Console.WriteLine("Congratulations, Player 2, all your ships have been placed.");
                    Console.WriteLine("Press any key to contine...");
                    Console.ReadKey();
                    break;
                }

                Console.Clear();
                Console.WriteLine("Player 2, this is your board:");
                gameScreenp2.printgrid();

                Console.WriteLine("Please select a ship by ID:");
                foreach (var ship in unconfiguredShips)
                {
                    Console.WriteLine($"ID: {ship.ID}, Name: {ship.Title}");
                }
                int shipId;
                while (!int.TryParse(Console.ReadLine(), out shipId) || !unconfiguredShips.Any(ship => ship.ID == shipId))
                {
                    Console.WriteLine("Invalid ship ID. Please enter a valid ID:");
                }

                bool validPlacement = false;
                while (!validPlacement)
                {
                    char orientation;
                    do
                    {
                        Console.WriteLine("Enter orientation (V for vertical, H for horizontal):");
                        orientation = Console.ReadLine().ToUpper()[0];
                    } while (orientation != 'V' && orientation != 'H');

                    char startRowChar;
                    int startRow;
                    do
                    {
                        Console.WriteLine("Enter starting row (A, B, C, etc.):");
                        startRowChar = Console.ReadLine().ToUpper()[0];
                        startRow = startRowChar - 'A' + 1; // +1 is removed
                    } while (startRow < 1 || startRow > GameScreen.height); // Adjusted condition

                    int startColumn;
                    do
                    {
                        Console.WriteLine("Enter starting column (1, 2, 3, etc.):");
                    } while (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width); //why startcolumn < 1?//

                    string coordinate = ConvertToCoordinateString(startRow, startColumn);
                    if (CanPlaceShip(gameScreenp2, shipId, orientation, startRow, startColumn, username2))
                    {
                        validPlacement = true;
                        Ship selectedShip = logic.GetShipById(shipId);
                        gameScreenp2.PlaceShipInGrid(startRow, startColumn, selectedShip.Size, orientation);
                        logic.MarkShipAsConfigured(shipId, currentGameId, username2, coordinate);


                        // Re-fetch the list of unconfigured ships
                        unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, username2);

                        Console.Clear();
                        Console.WriteLine("Ship placed successfully. Here's the updated board:");
                        gameScreenp2.printgrid();
                    }
                    else
                    {
                        Console.WriteLine("You cannot overlay ships, select different coordinates.");
                    }
                }
            }
        }
        public void LaunchAttack()
        {
            currentPlayer = username; // Player 1 starts
            opponentPlayer = username2; // Player 2

            if (gameState != GameState.ShipsConfigured)
            {
                Console.WriteLine("The game is not in a state where attacks can be launched.");
                return;
            }

            while (gameState == GameState.ShipsConfigured)
            {
                SetCurrentGameScreen();
                if (currentGameScreen == null)
                {
                    Console.WriteLine("Error: Game screen not set up correctly.");
                    break;
                }

                DisplayCurrentPlayerBoard();

                Console.WriteLine($"{currentPlayer}, it's your turn to attack.");

                // Get Row for Attack
                int startRow;
                do
                {
                    Console.WriteLine("Enter the row letter for your attack (A to G):");
                    char rowChar = Console.ReadLine().ToUpper()[0];
                    startRow = rowChar - 'A' + 1;
                    if (startRow < 1 || startRow > GameScreen.height)
                    {
                        Console.WriteLine("Invalid row. Please try again.");
                    }
                } while (startRow < 1 || startRow > GameScreen.height);

                // Get Column for Attack
                int startColumn;
                do
                {
                    Console.WriteLine("Enter the column number for your attack (1 to 8):");
                    if (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width)
                    {
                        Console.WriteLine("Invalid column. Please try again.");
                    }
                } while (startColumn < 1 || startColumn > GameScreen.width);

                // Convert row and column to coordinate string
                string attackCoordinates = ConvertToCoordinateString(startRow, startColumn);

                // Process the attack
                ProcessAttack(attackCoordinates);

                // Switch to the other player for the next turn
                SwitchPlayer();
            }
        }
        private void ProcessAttack(string coordinates)
        {
            // Determine which player is the opponent
            string targetPlayer = currentPlayer == username ? username2 : username;

            // Check if the attack hits any of the opponent's ships
            var attackResult = logic.ProcessPlayerAttack(targetPlayer, coordinates);

            // Update the game screen based on the attack result
            if (attackResult.IsHit)
            {
                Console.WriteLine("Hit!");
                currentGameScreen.MarkHit(coordinates);

                // Check if the hit ship is destroyed
                if (attackResult.IsShipDestroyed)
                {
                    Console.WriteLine($"You have destroyed {attackResult.Ship.Title}!");
                }
            }
            else
            {
                Console.WriteLine("Miss!");
                currentGameScreen.MarkMiss(coordinates);
            }
        }
        private void SetCurrentGameScreen()
        {
            // Ensure game screens are initialized
            if (gameScreenp1 != null && gameScreenp2 != null)
            {
                currentGameScreen = currentPlayer == username ? gameScreenp1 : gameScreenp2;
            }
            else
            {
                currentGameScreen = null;
            }
        }

        private void DisplayCurrentPlayerBoard()
        {
            if (currentGameScreen != null)
            {
                currentGameScreen.printgrid();
            }
            else
            {
                Console.WriteLine("Error: Current game screen is null.");
            }
        }

        private void SwitchPlayer()
        {
            string temp = currentPlayer;
            currentPlayer = opponentPlayer;
            opponentPlayer = temp;
            SetCurrentGameScreen();
        }








        private string ConvertToCoordinateString(int startRow, int startColumn)
        {
            char rowChar = (char)('A' + startRow - 1); // Adjusted for zero-based indexing
            return $"{rowChar}{startColumn}";
        }

        private bool CanPlaceShip(GameScreen gameScreen, int shipId, char orientation, int startRow, int startColumn, string playerUsername)
        {
            Ship selectedShip = logic.GetShipById(shipId);
            if (selectedShip == null || selectedShip.IsConfigured())
            {
                return false;
            }

            return gameScreen.IsValidPlacement(selectedShip, orientation, startRow, startColumn);
        }
        


        public class GameScreen
        {
            public const int width = 8;  // Width of the game grid
            public const int height = 7; // Height of the game grid

            private List<Cell> cells;
            public GameScreen(List<Cell> cells)
            {
                this.cells = cells;
            }
            public void ClearGrid()
            {
                cells.Clear();
            }
            public void printgrid()
            {
                
                char startLetter = 'A';

                // Print the top header numbers
                Console.Write("  ");
                for (int num = 1; num <= width; num++)
                {
                    Console.Write(num + " ");
                }
                Console.WriteLine();

                // Print the grid
                for (int row = 0; row < height; row++)
                {
                    // Print the row letter
                    Console.Write((char)(startLetter + row) + " ");

                    for (int col = 0; col < width; col++)
                    {
                        var cell = cells.FirstOrDefault(c => c.Row == row && c.Column == col);
                        if (cell != null)
                        {
                            cell.PrintCell();
                        }
                        else
                        {
                            Console.Write(". ");
                        }
                    }

                    Console.WriteLine();

                }
                Console.ReadKey();
            }
            public abstract class Cell
            {
                public int Row { get; set; }
                public int Column { get; set; }
                public abstract void PrintCell();
            }
            public void MarkAttack(int row, int column)
            {
                var cell = cells.FirstOrDefault(c => c.Row == row && c.Column == column);
                if (cell != null)
                {
                    if (cell is ShipCell shipCell)
                    {
                        shipCell.IsHit = true; // Mark the ship cell as hit
                        Console.WriteLine("Hit!");
                    }
                    else
                    {
                        cells.Add(new AttackCell(false) { Row = row, Column = column }); // Add a miss marker
                        Console.WriteLine("Miss!");
                    }
                }
                else
                {
                    cells.Add(new AttackCell(false) { Row = row, Column = column }); // Add a miss marker
                    Console.WriteLine("Miss!");
                }
            }
        
        public class ShipCell : Cell
            {
                public bool IsHit { get; set; } // Indicates if the ship cell is hit
                public override void PrintCell()
                {
                    Console.Write(IsHit ? "X " : "S ");//if ishit is true print x else print s
                }
            }
            public class AttackCell : Cell
            {
                public bool IsHit { get; set; } // Determines if the attack was successful

                public AttackCell(bool isHit)
                {
                    IsHit = isHit;
                }

                public override void PrintCell()
                {
                    Console.Write(IsHit ? "X " : "O ");
                }
            }
            public bool IsCellEmpty(int row, int column)
            {
                return !cells.Any(c => c.Row == row && c.Column == column);
            }

            public void PlaceShipInGrid(int row, int column, int shipSize, char orientation)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    int adjustedRow = row - 1; // Adjust for zero-based indexing
                    int adjustedColumn = column - 1; // Adjust for zero-based indexing

                    if (orientation == 'H')
                    {
                        // For horizontal placement, increment the column
                        cells.Add(new ShipCell { Row = adjustedRow, Column = adjustedColumn + i });
                    }
                    else if (orientation == 'V')
                    {
                        // For vertical placement, increment the row
                        cells.Add(new ShipCell { Row = adjustedRow + i, Column = adjustedColumn });
                    }
                }
            }
            public bool IsValidPlacement(Ship ship, char orientation, int startRow, int startColumn)
            {
                int shipSize = ship.Size;

                // Adjust for zero-based indexing
                int adjustedStartRow = startRow - 1;
                int adjustedStartColumn = startColumn - 1;

                // Check if ship placement is within the grid boundaries
                if (orientation == 'H' && (adjustedStartColumn + shipSize) > width) return false;
                if (orientation == 'V' && (adjustedStartRow + shipSize) > height) return false;

                // Check each cell the ship will occupy
                for (int i = 0; i < shipSize; i++)
                {
                    int checkRow = orientation == 'V' ? adjustedStartRow + i : adjustedStartRow;
                    int checkColumn = orientation == 'H' ? adjustedStartColumn + i : adjustedStartColumn;

                    if (!IsCellEmpty(checkRow, checkColumn))
                    {
                        return false; // Collision with another ship
                    }
                }

                return true; // No collisions
            }
            


        }
    }
}




           
    
