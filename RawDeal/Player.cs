using RawDealView;

namespace RawDeal;

public class Player
{
    private View _view;
    private Deck _deck;
    public String Name;
    public List<Card> Hand = new List<Card>();
    public List<Card> Arsenal = new List<Card>();
    public List<Card> RingArea = new List<Card>();
    public List<Card> RingSide = new List<Card>();
    public Game Game;
    public int Fortitude;
    public SuperstarCard SuperstarCard;

    public PlayerInfo PlayerInfo => new(SuperstarCard.Name, Fortitude, Hand.Count, Arsenal.Count);

    public Player(View view, Deck deck, Game game)
    {
        _view = view;
        _deck = deck;
        Game = game;
        SaveCards();
        SaveName();
    }

    private void SaveCards()
    {
        SaveSuperStarCard();
        foreach (var card in _deck.DeckCards.Skip(1))
        {
            Arsenal.Add(_deck.AllCards.GetCardDataByTitle(card));
        }
    }
    
    private void SaveSuperStarCard()
    {
        SuperstarCard = _deck.DeckSuperstarCard;
        SuperstarCard.Player = this;
        SuperstarCard.Game = Game;
        SuperstarCard.View = _view;

    }
    private void SaveName()
    {
        Name = SuperstarCard.Name;
    }

    public void DrawCard()
    {
        if (!IsArsenalEmpty())
        {
            Card drawnCard = Arsenal[^1];
            int handSize = Hand.Count;
            drawnCard.IndexHand = handSize;
            Hand.Add(drawnCard);
            Arsenal.RemoveAt(Arsenal.Count - 1);
        }
    }

    public bool IsArsenalEmpty()
    {
        if (Arsenal.Count == 0)
        {
            return true;
        }
        else return false;
    }

    public void PlayCard(Card card)
    {
        RemoveCardFromHand(card.IndexHand);
        int aditionalFortitude = int.Parse(card.Damage);
        Fortitude += aditionalFortitude;
        RingArea.Add(card);
    }

    public void RemoveCardFromHand(int removedIndex)
    {
        Hand.RemoveAt(removedIndex);
        if (Hand.Count > 0)
        {
            FixIndexHand(removedIndex);
        }
    }

    private void FixIndexHand(int removedIndex)
    {
        var lastCard = Hand[^1];
        for (int i = removedIndex; i < lastCard.IndexHand; i++)
        {
            var card = Hand[i];
            card.IndexHand -= 1;
            Hand[i] = card;
        }
    }

    public Card LostCardForDamage()
    {
        Card drawnCard = Arsenal[^1];
        RingSide.Add(drawnCard);
        Arsenal.RemoveAt(Arsenal.Count - 1);

        return drawnCard;
    }

    public void StartingDraw()
    {
        int counter = 0;
        while (counter < SuperstarCard.HandSize)
        {
            DrawCard();
            counter++;
        }
    }

    public string ShowPlayOptions()
    {
        string selectedOption = _view.ShowPlayerOptions();
        return selectedOption;
    }
    
    public string ShowPlayOptionsForSuperstar()
    {
        string selectedOption = _view.ShowSpecialPlayerOptions();
        return selectedOption;
    }
    
    public void RecoverCardFromRingSideToArsenal(int index)
    {
        Card card = RingSide[index];
        RingSide.RemoveAt(index);
        Arsenal.Insert(0, card);

        if (RingSide.Count > 0)
        {
            FixIndexRingSide(index);
        }
    }
    
    public void RecoverCardFromRingSideToHand(int index)
    {
        Card card = RingSide[index];
        RingSide.RemoveAt(index);
        Hand.Add(card);

        if (RingSide.Count > 0)
        {
            FixIndexRingSide(index);
        }
    }
    
    public void DiscardCardFromHand(int index)
    {
        Card card = Hand[index];
        Hand.RemoveAt(index);
        RingSide.Add(card);

        if (Hand.Count > 0)
        {
            FixIndexHand(index);
        }
    }
    
    private void FixIndexRingSide(int removedIndex)
    {
        var lastCard = RingSide[^1];
        for (int i = removedIndex; i < lastCard.IndexRingSide; i++)
        {
            var card = RingSide[i];
            card.IndexRingSide -= 1;
            RingSide[i] = card;
        }
    }
    
    public void RemoveCardFromHandToArsenal(int index)
    {
        Card card = Hand[index];
        Hand.RemoveAt(index);
        Arsenal.Insert(0, card);

        if (Hand.Count > 0)
        {
            FixIndexHand(index);
        }
    }
    
    public Card GetHandCardByTitle(string title)
    {
        Card card = Hand.FirstOrDefault(x => x.Title == title);
        return card;
    }
    
}

// private void IsFullMenuOptions() {}