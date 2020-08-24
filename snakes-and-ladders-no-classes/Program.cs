using System;

namespace snakes_and_ladders
{
    class Program
    {
        static Random randomNumberGenerator = new Random();

        static void Main(string[] args)
        {
            int numberOfPlayers;
            string[] playerNames;
            string[] playerColours;
            int[] playerPositions;
            int currentPlayerIndex = 0;
            int boardSize = 100;

            int[] snakeStartPositions = { 98, 79, 55, 39, 96, 70, 28 };
            int[] snakeEndPositions   = { 60, 56, 45, 18, 33, 49, 10 };

            int[] ladderStartPositions = { 04, 15, 30, 90, 66, 61, 36, 07 };
            int[] ladderEndPositions   = { 23, 72, 52, 93, 77, 82, 41, 12 };

            Console.Write("How many players? ");
            numberOfPlayers = int.Parse(Console.ReadLine());

            playerNames = new string[numberOfPlayers];
            playerColours = new string[numberOfPlayers];
            playerPositions = new int[numberOfPlayers];

            for (int p = 1; p <= numberOfPlayers; p++)
            {
                Console.Write($"Enter the name of player {p}: ");
                playerNames[p - 1] = Console.ReadLine();
                Console.Write($"Enter the colour for player {p}: ");
                playerColours[p - 1] = Console.ReadLine();
            }

            while (true)
            {
                currentPlayerIndex = SelectFirstPlayerIndex(playerNames, playerColours);

                Console.ReadLine();

                while (true)
                {
                    playerPositions[currentPlayerIndex] = TakeTurn(playerNames[currentPlayerIndex], playerColours[currentPlayerIndex], playerPositions[currentPlayerIndex], snakeStartPositions, snakeEndPositions, ladderStartPositions, ladderEndPositions, boardSize);

                    if (playerPositions[currentPlayerIndex] == boardSize)
                    {
                        SayWinner(playerNames[currentPlayerIndex]);
                        break;
                    }

                    Console.ReadLine();

                    currentPlayerIndex = SelectNextPlayerIndex(currentPlayerIndex, numberOfPlayers);
                }

                Console.WriteLine();
                Console.Write("Play again? (y/n) ");
                bool playAgain = Console.ReadLine().ToLower() == "y";

                if (playAgain == false)
                    break;

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    playerPositions[i] = 0;
                }

                currentPlayerIndex = -1;
            }
        }

        static int TakeTurn(string playerName, string playerColour, int playerPosition, int[] snakeStartPositions, int[] snakeEndPositions, int[] ladderStartPositions, int[] ladderEndPositions, int boardSize)
        {
            int roll;
            bool playerGetsAnotherGo = false;

            do
            {
                // Roll the dice
                roll = rollDice();
                SayRoll(playerName, playerColour, roll);

                // Move the player that many spaces forward on the board
                // Take into account snakes, ladders and bounce-back
                // Advance the player forwards
                playerPosition += roll;

                // If player over-shoots the final square, they bounce backwards
                if (playerPosition > boardSize)
                {
                    int bounce = playerPosition - boardSize;
                    playerPosition = boardSize - bounce;
                    SayBounce(playerName, playerColour);
                }

                SaySquareLandedOn(playerName, playerColour, playerPosition);

                // Has the player landed on a ladder?
                // If so, advance up the ladder
                for (int i = 0; i < ladderStartPositions.Length; i++)
                {
                    int ladderStart = ladderStartPositions[i];
                    if (ladderStart == playerPosition)
                    {
                        SayLandedOnLadder(playerName, playerColour);
                        playerPosition = ladderEndPositions[i];
                        SaySquareLandedOn(playerName, playerColour, playerPosition);
                        break;
                    }
                }

                // Has the player landed on a snake?
                // If so, slide down the snake
                for (int i = 0; i < snakeStartPositions.Length; i++)
                {
                    int snakeStart = snakeStartPositions[i];
                    if (snakeStart == playerPosition)
                    {
                        SayLandedOnSnake(playerName, playerColour);
                        playerPosition = snakeEndPositions[i];
                        SaySquareLandedOn(playerName, playerColour, playerPosition);
                        break;
                    }
                }

                // Player wins if they have landed on the final square of the board
                if (playerPosition == boardSize)
                {
                    return playerPosition;
                }

                // Player will get another go if they rolled a six
                playerGetsAnotherGo = false;
                if (roll == 6)
                {
                    playerGetsAnotherGo = true;
                    SayAnotherGo(playerName, playerColour);
                }
            }
            while (playerGetsAnotherGo);

            return playerPosition;
        }

        static int SelectNextPlayerIndex(int currentPlayerIndex, int numberOfPlayers)
        {
            return (currentPlayerIndex + 1) % numberOfPlayers;
        }

        private static int rollDice()
        {
            // Return random number between 1 - 6
            return randomNumberGenerator.Next(1, 6 + 1);
        }

        private static int SelectFirstPlayerIndex(string[] playerNames, string[] playerColours)
        {
            // Each player rolls the dice
            // Whoever gets highest roll goes first
            // If two or more players both roll same same highest roll, the last one to roll wins

            SaySelectingFirstPlayer();

            int highestRoll = 0;
            int highestRollPlayerIndex = 0;

            for (int i = 0; i < playerNames.Length; i++)
            {
                int roll = rollDice();
                SayRoll(playerNames[i], playerColours[i], roll);

                if (roll >= highestRoll)
                {
                    highestRoll = roll;
                    highestRollPlayerIndex = i;
                }
            }

            SayFirstPlayer(playerNames[highestRollPlayerIndex]);

            return highestRollPlayerIndex;
        }

        static void SaySelectingFirstPlayer()
        {
            Console.WriteLine();
            Console.WriteLine($"Selecting the first player...");
        }

        static void SayFirstPlayer(string playerName)
        {
            Console.WriteLine($"{playerName} will be the first player");
        }

        static void SayRoll(string playerName, string playerColour, int roll)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} rolled a {roll}");
            Console.ResetColor();
        }

        static void SaySquareLandedOn(string playerName, string playerColour, int playerPosition)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} is now on square {playerPosition}");
            Console.ResetColor();
        }

        static void SayLandedOnLadder(string playerName, string playerColour)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} has landed on a LADDER");
            Console.ResetColor();
        }

        static void SayLandedOnSnake(string playerName, string playerColour)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} has landed on a SNAKE");
            Console.ResetColor();
        }

        static void SayWinner(string playerName)
        {
            Console.WriteLine();
            Console.WriteLine($"GAME OVER - {playerName} is the winner!");
        }

        static void SayBounce(string playerName, string playerColour)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} bounced backwards at the end of the board");
            Console.ResetColor();
        }

        static void SayAnotherGo(string playerName, string playerColour)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), playerColour, true);
            Console.WriteLine($"{playerName} gets another go, because of rolling a six");
            Console.ResetColor();
        }
    }
}
