using WarGame.Models.Enums;
using WarGame.Models.Responses;
using WarGame.Models;

public class DeckService : IDeckService
{
    private Player[] _players; // Holds the current state of the players
    private int _maxRounds = 1000; // A larger maximum number of rounds

    private readonly Suit[] AvailableSuits = { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades };
    private readonly List<int> AvailableNumbers = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

    public Player[] GetPlayers()
    {
        return _players;
    }
    public Player[] DealCards()
    {
        _players = new Player[2];

        var shuffledDeck = ShuffleDeck();

        Player firstPlayer = new Player();
        Player secondPlayer = new Player();

        int index = 0;
        while (shuffledDeck.Count > 0)
        {
            Card card = shuffledDeck.Pop();
            if (index % 2 == 0)
            {
                firstPlayer.Deck.Cards.Push(card);
            }
            else
            {
                secondPlayer.Deck.Cards.Push(card);
            }
            index++;
        }

        _players[0] = firstPlayer;
        _players[1] = secondPlayer;

        return _players;
    }

    public PlayResponse Play()
    {
        if (_players == null || _players[0].Deck.Cards.Count == 0 || _players[1].Deck.Cards.Count == 0)
        {
            throw new InvalidOperationException("Cards have not been dealt yet. Please deal the cards first.");
        }

        Player firstPlayer = _players[0];
        Player secondPlayer = _players[1];

        // Each player draws their top card
        Card firstPlayerCard = firstPlayer.Deck.Cards.Pop();
        Card secondPlayerCard = secondPlayer.Deck.Cards.Pop();

        PlayResponse response = new PlayResponse
        {
            FirstPlayerCard = firstPlayerCard.Name,
            SecondPlayerCard = secondPlayerCard.Name
        };

        Stack<Card> wonCards = new Stack<Card>();
        wonCards.Push(firstPlayerCard);
        wonCards.Push(secondPlayerCard);

        // Determine who wins the round
        if (firstPlayerCard.Number > secondPlayerCard.Number)
        {
            response.Message = "First player wins this round!";
            AddWonCardsToBottomOfDeck(firstPlayer, wonCards);
        }
        else if (firstPlayerCard.Number < secondPlayerCard.Number)
        {
            response.Message = "Second player wins this round!";
            AddWonCardsToBottomOfDeck(secondPlayer, wonCards);
        }
        else
        {
            response.Message = "It's a tie! Initiating war...";
            HandleWar(firstPlayer, secondPlayer, wonCards, response);
        }

        // Check if any player has won the game
        if (firstPlayer.Deck.Cards.Count == 0)
        {
            response.Message = "Second player wins the game!";
            return response;
        }
        if (secondPlayer.Deck.Cards.Count == 0)
        {
            response.Message = "First player wins the game!";
            return response;
        }

        response.Players = [firstPlayer, secondPlayer];
        response.FirstPlayerDeckSize = firstPlayer.Deck.Cards.Count;
        response.SecondPlayerDeckSize = secondPlayer.Deck.Cards.Count;

        return response;
    }

    private void HandleWar(Player firstPlayer, Player secondPlayer, Stack<Card> wonCards, PlayResponse response)
    {
        Random random = new Random();

        int firstPlayerWarPoints = 0;
        int secondPlayerWarPoints = 0;

        // Each player draws 3 additional cards (or as many as they have left)
        for (int i = 0; i < 3 && firstPlayer.Deck.Cards.Count > 0 && secondPlayer.Deck.Cards.Count > 0; i++)
        {
            var firstWarCard = firstPlayer.Deck.Cards.Pop();
            var secondWarCard = secondPlayer.Deck.Cards.Pop();

            wonCards.Push(firstWarCard);
            wonCards.Push(secondWarCard);

            firstPlayerWarPoints += firstWarCard.Number;
            secondPlayerWarPoints += secondWarCard.Number;
        }

        // Randomize the war outcome slightly to introduce unpredictability
        if (firstPlayerWarPoints + random.Next(0, 5) > secondPlayerWarPoints + random.Next(0, 5))
        {
            response.Message += " First player wins the war!";
            AddWonCardsToBottomOfDeck(firstPlayer, wonCards);
        }
        else
        {
            response.Message += " Second player wins the war!";
            AddWonCardsToBottomOfDeck(secondPlayer, wonCards);
        }
    }

    private void AddWonCardsToBottomOfDeck(Player player, Stack<Card> wonCards)
    {
        var cardList = wonCards.ToList();
        var shuffledWonCards = cardList.OrderBy(card => Guid.NewGuid()).ToList(); // Shuffle won cards
        foreach (var card in shuffledWonCards)
        {
            player.Deck.Cards.Push(card); // Insert the cards at the bottom of the deck
        }
    }

    private Stack<Card> ShuffleDeck()
    {
        List<Card> deck = new List<Card>();

        foreach (var suit in AvailableSuits)
        {
            foreach (var number in AvailableNumbers)
            {
                string cardName = number switch
                {
                    11 => "Jack",
                    12 => "Queen",
                    13 => "King",
                    14 => "Ace",
                    _ => number.ToString()
                };

                deck.Add(new Card()
                {
                    Number = number,
                    Suit = suit,
                    Name = $"{cardName} of {suit}"
                });
            }
        }

        Random random = new Random();
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            var temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }

        return new Stack<Card>(deck);
    }
}
