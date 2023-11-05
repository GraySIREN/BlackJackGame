using System;
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Black;
    Deck deck = new();

    deck.Shuffle(new Random());

    List<Card> playerHand = new();
    List<Card> dealerHand = new();

    Console.WriteLine("Hello, what is your name?");
    string playerName = Console.ReadLine();
    Console.WriteLine("Welcome to Blackjack!");

    // Not dealing with Hard or Soft 17, Splitting, int-Downs, etc.
    //******************************************************************************

    bool readyToPlay = PromptUserForDecision(playerName);

    if (readyToPlay)
    {
        Console.WriteLine("Let's play BlackJack!");
    }
    else
    {
        Console.WriteLine("Maybe next time. Goodbye!");
        return;
    }

    static bool PromptUserForDecision(string playerName)
    {
        Console.WriteLine($"Are you ready to play?");
        Console.WriteLine("[Y] To Play.");
        Console.WriteLine("[N] To Exit.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            char keyChar = char.ToUpper(keyInfo.KeyChar);

            if (keyChar == 'Y')
            {
                Console.WriteLine();
                return true;

            }
            else if (keyChar == 'N')
            {
                Console.WriteLine();
                return false;

            }
            else
            {
                Console.WriteLine("\nInvalid input. Please press [Y] to play or [N] to exit.");
            }
        }
    }
    //******************************************************************************
    // Round 1 Start, Buy-In

    Console.WriteLine("Do you want to buy in at $10, $25, $50, or $100?");
    int buyInValue;

    while (true)
    {
        if (!int.TryParse(Console.ReadLine(), out buyInValue) || (buyInValue != 10 && buyInValue != 25 && buyInValue != 50 && buyInValue != 100))
        {
            Console.WriteLine("Invalid response. Please enter [10], [25], [50], or [100].");
        }
        else
        {
            break;
        }
    }

    int roundOutcome = 0;
    int bankRollAmount = buyInValue + -roundOutcome;

    Console.WriteLine($"Great! You've bought in at {buyInValue}. Your Bankroll is ${bankRollAmount}.");
    bool playAgain = true;

    //******************************************************************************
    // All continuous round start

    while (playAgain == true)
    {
        deck.Shuffle(new Random());
        playerHand.Clear();
        dealerHand.Clear();

        // Deal 2 new cards to player and dealer
        playerHand.Add(deck.DealCard());
        dealerHand.Add(deck.DealCard());
        playerHand.Add(deck.DealCard());
        dealerHand.Add(deck.DealCard());

        if (bankRollAmount <= 0)
        {
            Console.WriteLine("Sorry! You have bust out. Better luck next time!");
            return;
        }

        int playerTotal = CalculateHandValue(playerHand);
        int dealerTotal = CalculateHandValue(dealerHand);

        // Introduce betting mechanic
        // No decimals, need to test negative numbers

        int betValue;

        while (true)
        {
            Console.WriteLine("How much would you like to bet? Min ($1) Max ($10000)");
            if (!int.TryParse(Console.ReadLine(), out betValue) || betValue < 0 || betValue > 10000)
            {
                Console.WriteLine("Invalid bet amount. Please enter an amount between $1.00 and $10000.");
            }
            else
            {
                break;
            }
        }

        Console.WriteLine($"You're betting ${betValue}. Good Luck!");
        Console.WriteLine("");
        Console.WriteLine($"Player's hand: {DisplayHand(playerHand, true)} ({playerTotal})");
        //Console.WriteLine($"Dealer's hand: {DisplayHand(dealerHand, true)} ({dealerTotal})");  //Only show the first card and sequential cards dealer draws

        //******************************************************************************
        // Actual Gameplay - Need to refine some of the mechanics and prompts

        while (playerTotal < 21)
        {
            Console.WriteLine("Do you want to hit [H] or stand [S]?");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            // Not handling invalid inputs

            if (choice == 'H' && playerTotal < 21)
            {
                Card newCard = deck.DealCard();
                playerHand.Add(newCard);
                playerTotal = CalculateHandValue(playerHand);
                Console.WriteLine($"\nYou drew {DisplayCard(newCard)}");
                Console.WriteLine($"Player's hand: {DisplayHand(playerHand, true)} ({playerTotal})");

                if (dealerTotal < 17)
                // Need to make a new variable for facedown and faceup cards for the Dealer

                {
                    Card newCardDealer = deck.DealCard();
                    dealerHand.Add(newCardDealer);
                    dealerTotal = CalculateHandValue(dealerHand);
                    Console.WriteLine($"\nDealer has to hit. Dealer draws {DisplayCard(newCardDealer)}");
                    //Console.WriteLine($"Dealer hand: {DisplayHand(dealerHand, true)} ({dealerTotal})");
                    continue;
                }
                continue;
            }

            if (choice == 'S')
            {
                Console.WriteLine($"\nYou Stand. {DisplayHand(playerHand, true)} ({playerTotal})");

                while (dealerTotal <= 17) //"Hit on Soft 17, Stand on Hard 17" when implemented
                {
                    Card newCard = deck.DealCard();
                    dealerHand.Add(newCard);
                    dealerTotal = CalculateHandValue(dealerHand);
                    Console.WriteLine($"\nDealer has to hit. Dealer draws {DisplayCard(newCard)}");
                    Console.WriteLine($"Dealer hand: {DisplayCard(newCard)} ({dealerTotal})");
                }
                break;
            }

            else if (choice != 'H' && choice != 'S')
            {
                Console.WriteLine("Invalid Response.\nPlease enter 'H' to Hit or 'S' to Stand.");
                continue;
            }
        }

        //******************************************************************************
        // Round over, determine winner
        // Retrieve cards first

        DetermineWinner(playerTotal, dealerTotal);

        string DisplayHand(List<Card> hand, bool revealAll = false)
        {
            List<string> cardStrings = new();

            for (int i = 0; i < hand.Count; i++)
            {
                if (i == 0 && !revealAll)
                {
                    cardStrings.Add("Hidden");
                }
                else
                {
                    cardStrings.Add(DisplayCard(hand[i]));
                }
            }

            return string.Join(", ", cardStrings);
        }

        // Calculate hand value

        int CalculateHandValue(List<Card> hand)
        {
            int total = 0;
            int numAces = 0;

            foreach (var card in hand)
            {
                if (Enum.TryParse(card.FaceValue, out FaceValue faceValue))
                {
                    if (faceValue == FaceValue.Ace)
                    {
                        numAces++;
                        total += 11; // Assume Ace is 11 initially
                    }
                    else if (faceValue >= FaceValue.Two && faceValue <= FaceValue.Ten)
                    {
                        total += (int)faceValue;
                    }
                    else
                    {
                        total += 10; // Jack, Queen, and King are all worth 10
                    }
                }
            }

            // Adjust the value of Aces if needed

            while (numAces > 0 && total > 21)
            {
                total -= 10; // Change the value of one Ace from 11 to 1
                numAces--;
            }

            return total;
        }

        string DisplayCard(Card card)
        {
            return $"{card.FaceValue} of {card.Suit}";
        }

        //******************************************************************************
        // Tell user outcome of round


        // Could turn this into a switch statement

        void DetermineWinner(int playerTotal, int dealerTotal)
        {
            if (playerTotal > 21)                                       //Player loses
            {
                Console.WriteLine("Player busts! Dealer wins!");
                bankRollAmount = bankRollAmount - betValue;
                Console.WriteLine($"- ${betValue}\nBankroll: ${bankRollAmount}");
            }
            else if (dealerTotal > 21)                                  //Player wins
            {
                Console.WriteLine($"Dealer busts! {playerName} wins!");
                bankRollAmount = bankRollAmount + (betValue * 2);
                Console.WriteLine($"+ ${betValue * 2}\nBankroll: ${bankRollAmount}");
            }
            else if (playerTotal == 21 && playerTotal != dealerTotal)    //Player wins
            {
                Console.WriteLine($"{playerName} has Blackjack!");
                bankRollAmount = bankRollAmount + (betValue * 2);
                Console.WriteLine($"+ ${betValue * 2}\nBankroll: ${bankRollAmount}");
            }
            else if (dealerTotal == 21)                                 //Player loses
            {
                Console.WriteLine("Dealer has Blackjack!");
                bankRollAmount = bankRollAmount - betValue;
                Console.WriteLine($"- ${betValue}\nBankroll: ${bankRollAmount}");
                return;
                //Immediate loss if Dealer gets 21
            }
            else if (playerTotal > dealerTotal)                         //Player wins
            {
                Console.WriteLine($"{playerName} wins!");
                bankRollAmount = bankRollAmount + (betValue * 2);
                Console.WriteLine($"+ ${betValue * 2}\nBankroll: ${bankRollAmount}");
            }
            else if (playerTotal < dealerTotal)                         //Player loses
            {
                Console.WriteLine("Dealer wins!");
                bankRollAmount = bankRollAmount - betValue;
                Console.WriteLine($"- ${betValue}\nBankroll: ${bankRollAmount}");
            }
            else if (playerTotal == dealerTotal)
            {
                Console.WriteLine("It's a tie!");                       // TIE
                Console.WriteLine($"All bets returned. BankRoll : {bankRollAmount}");
            }
            else if (dealerTotal == 21)
            {
                Console.WriteLine("Dealer has BlackJack! You lose!");
                bankRollAmount = bankRollAmount - betValue;
                Console.WriteLine($"- ${betValue}\nBankroll: ${bankRollAmount}");
                return;
            }
        }

        //******************************************************************************
        // Can/Does user want to play another round?

        if (bankRollAmount > 0)
        {
            Console.WriteLine("Would you like to play another round? [Y] [N]");
            string anotherRound = Console.ReadLine();
            string anotherRoundUpper = anotherRound.ToUpper();

            bool playAnotherRound = anotherRoundUpper == "Y";

            if (playAnotherRound == true)
            {
                playAgain = true;
                continue;
            }
            if (playAnotherRound != true && playAnotherRound)
            {
                Console.WriteLine("Invalid Response. Please enter [Y] to play another round or [N] to quit the game.");
            }
            if (playAnotherRound == false)
            {
                Console.WriteLine("Okay, see ya again next time!");
                break;
            }
            if (bankRollAmount <= 0)
            {
                Console.WriteLine($"Sorry, you have busted out of the game. You are at {bankRollAmount}. Better luck next time!");
                break;
            }
        }
    }
}

//Class
public class Deck
{
    public List<Card> Cards { get; set; }

    public Deck()
    {
        Cards = new List<Card>();
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (FaceValue faceValue in Enum.GetValues(typeof(FaceValue)))
            {
                Cards.Add(new Card { Suit = suit.ToString(), FaceValue = faceValue.ToString() });
            }
        }
    }

    //Method
    public void Shuffle(Random rng)
    {
        //Random rng = new Random();
        int n = Cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = value;
        }
    }

    //Method
    public Card DealCard()
    {
        Card card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }
}

//Class
public class Card
{
    public string? Suit { get; set; }
    public string? FaceValue { get; set; }
}

//Enum Class
public enum FaceValue
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace
}

//Enum Class
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}
