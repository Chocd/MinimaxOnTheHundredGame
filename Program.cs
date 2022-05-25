using System;


    class Program
    {
        static int maxMove = 0;
        static int startingGameNumber = 0;
        static bool ABPruneSelect = false;
        static long CurrentTimeInSeconds;
        static long DeltaTime;
        static int timeInSeconds;
        static DateTimeOffset now;
        static void Main(string[] args)
        {

            //Determine the game
            //Randomly generated move trees? how?
            //start with a max number
            //substraction maximum number
            //So the game goes like 1000 - 5 - 3 - 2 - 5 so on with 1 through 5 as possible moves


            Console.WriteLine("you need to go down from the wanted number to 0, while substracting a number between 1 and MAX");
            Console.WriteLine("the first to reach 0 wins");
            Console.WriteLine();
            Console.WriteLine("READY?");
            string readycheck = Console.ReadLine();
            if (readycheck.ToLower() != "yes")
            {
                Console.WriteLine("YOU ARE NOT READY");
                Console.WriteLine("GAME OVER");
                return;
            }
            Console.WriteLine(" \n \n ");
            Random rnd = new Random();
            int gameNumber = 35; //rnd.Next(20, 40);
            startingGameNumber = gameNumber;
            rnd = new Random();
            maxMove = 6;//rnd.Next(2, 10);

            Console.WriteLine("WHAT IS THE MAX ALLOWED TIME FOR COMPUTER TO MOVE?");
            try
            {
                timeInSeconds = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("please enter the time correctly");
                Console.WriteLine("Quitting...");
                return;
            }
            Console.WriteLine("DO YOU WANT ALPHA BETA PRUNING?");
            string ABPruneSelector = Console.ReadLine();
            if (ABPruneSelector.ToLower() == "yes")
            {
                ABPruneSelect = true;
            }
            else if (ABPruneSelector.ToLower() == "no")
            {
                ABPruneSelect = false;
            }
            else
            {
                Console.WriteLine("Defaulting to YES");
                ABPruneSelect = true;
            }
            while (true)
            {

                if (gameNumber <= 0)
                {
                    Console.WriteLine("YOU LOSE");
                    break;
                }

                Console.WriteLine("Current Number is :" + gameNumber + ", enter a number between 1 and " + maxMove);
                try
                {
                    int playerInput = Convert.ToInt32(Console.ReadLine());
                    if (playerInput > maxMove || playerInput < 0)
                    {
                        Console.WriteLine("Wrong input (too low or too high)");
                        continue;
                    }
                    gameNumber -= playerInput;
                }
                catch
                {
                    Console.WriteLine("Wrong input (not a number)");
                    continue;
                }


                if (gameNumber <= 0)
                {
                    Console.WriteLine("YOU WIN");
                    break;
                }
                now = DateTimeOffset.UtcNow;
                CurrentTimeInSeconds = now.ToUnixTimeSeconds();


                //we do the first minimax step here, just so we can make sure we are able to use the move
                //there is for sure a better way, no idea what though :b
                int bestScore = -int.MaxValue;
                int chosenMove = 1; //default move is one if computer couldn't choose a better move in time (nearly impossible)
                for (int i = 1; i <= maxMove; i++)
                {
                    gameNumber -= i;
                    int score = ABPruneSelect ? MinimaxAB(gameNumber, int.MinValue, int.MaxValue, 0, 'C') : Minimax(gameNumber, 0, 'C');
                    gameNumber += i;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        chosenMove = i;
                    }

                }
                Console.WriteLine("Time taken for a decision :" + DeltaTime);

                gameNumber -= chosenMove;

            }


        }

        //Minimax returns 100 if player wins, 0 if no one won, and -100 if AI wins.
        public static int Minimax(int gameNumber, int depth, char turnOfWho)
        {

            int bestScore = turnOfWho == 'C' ? int.MaxValue : -int.MaxValue;


            DeltaTime = now.ToUnixTimeSeconds() - CurrentTimeInSeconds;
            now = DateTime.UtcNow;
            if (DeltaTime > timeInSeconds)
            {
                return bestScore;
            }

            if (gameNumber <= 0)
            {
                //Depth control allows us to finish the game as soon as possible, this can be disabled but will lead to worsened game performance.
                if (turnOfWho == 'C')
                {
                    return 10000000 - depth; //player wins in this scenario, as the turn passed to the Computer with 0 left
                }
                else if (turnOfWho == 'P')
                {
                    return -10000000 + depth; //Computer wins in this scenario, as the turn passed to the Player with 0 left
                }
            }
            depth++; //game hasn't ended and we are one layer deeper
            if (turnOfWho == 'C')
            {
                for (int i = 1; i <= maxMove; i++)
                {
                    gameNumber -= i;
                    int score = Minimax(gameNumber, depth, 'P');
                    gameNumber += i;
                    bestScore = bestScore < score ? score : bestScore; //if score is better than best score, change the score (MAX)

                }
                return bestScore;
            }
            else if (turnOfWho == 'P')
            {
                for (int i = 1; i <= maxMove; i++)
                {
                    gameNumber -= i;
                    int score = Minimax(gameNumber, depth, 'C');
                    gameNumber += i;
                    bestScore = bestScore > score ? score : bestScore; //if score is worse than best score, change the score (MIN)

                }
                return bestScore;
            }
            else
            {
                throw new Exception("GAME BROKE");
            }

        }
        public static int MinimaxAB(int gameNumber, int alpha, int beta, int depth, char turnOfWho)
        {
            int bestScore = turnOfWho == 'C' ? -int.MaxValue : int.MaxValue;


            DeltaTime = now.ToUnixTimeSeconds() - CurrentTimeInSeconds;
            now = DateTime.UtcNow;
            if (DeltaTime >= timeInSeconds) //timer starts at 0, then it goes to 1, 2, 3 so on. So it is always one bigger.
            {
                return bestScore;
            }

            if (gameNumber <= 0)
            {
                //Depth control allows us to finish the game as soon as possible, this can be disabled but will lead to worsened game performance.
                if (turnOfWho == 'C')
                {
                    return int.MaxValue - depth; //player wins in this scenario, as the turn passed to the Computer with 0 left
                }
                else if (turnOfWho == 'P')
                {
                    return -int.MaxValue + depth; //Computer wins in this scenario, as the turn passed to the Player with 0 left
                }
            }
            depth++; //game hasn't ended and we are one layer deeper
            if (turnOfWho == 'C')
            {
                for (int i = 1; i <= maxMove; i++)
                {
                    gameNumber -= i;
                    int score = MinimaxAB(gameNumber, alpha, beta, depth, 'P');
                    gameNumber += i;
                    bestScore = bestScore < score ? score : bestScore; //if score is better than best score, change the score (MAX)
                    alpha = alpha < score ? score : alpha;
                    if (beta <= alpha)
                    {
                        break;
                    }

                }
                return bestScore;
            }
            else if (turnOfWho == 'P')
            {
                for (int i = 1; i <= maxMove; i++)
                {
                    gameNumber -= i;
                    int score = MinimaxAB(gameNumber, alpha, beta, depth, 'C');
                    gameNumber += i;
                    bestScore = bestScore > score ? score : bestScore; //if score is worse than best score, change the score (MIN)
                    beta = beta > score ? score : beta;
                    if (beta <= alpha)
                    {
                        break;
                    }

                }
                return bestScore;
            }
            else
            {
                throw new Exception("GAME BROKE");
            }

        }
    }

