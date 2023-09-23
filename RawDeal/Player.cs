using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;

namespace RawDeal;

public class Player
{
    private View _view;
    private Deck _deck;
    public String _name;
    public List<Card> _hand = new List<Card>();
    public List<Card> _arsenal = new List<Card>();
    public List<Card> _ringArea = new List<Card>();
    public List<Card> _ringSide = new List<Card>();
    public int Fortitude;
    public SuperstarCard _superstarCard;
    
    public PlayerInfo PlayerInfo => new(_superstarCard.Name, Fortitude, _hand.Count, _arsenal.Count);

    public Player(View view, Deck deck)
    {
        _view = view;
        _deck = deck;
        SaveCards();
        SaveName();
    }
    
    private void SaveCards()
    {
        _superstarCard = _deck._deckSuperstarCard;
        foreach (var card in _deck._deckCards.Skip(1))
        {
            _arsenal.Add(_deck._allCards.GetCardDataByTitle(card));
        }
        
    }

    private void SaveName()
    {
        _name = _superstarCard.Name;
    }

    public void DrawCard()
    {
        if (_arsenal.Count > 0)
        {
            Card drawnCard = _arsenal[_arsenal.Count - 1];
            int handSize = _hand.Count;
            drawnCard.IndexHand = handSize;
            _hand.Add(drawnCard);
            _arsenal.RemoveAt(_arsenal.Count - 1);
        }
    }

    public void PlayCard(Card card)
    {
        RemoveCardFromHand(card);
        int aditionalFortitude = int.Parse(card.Damage);
        Fortitude += aditionalFortitude;
        _ringArea.Add(card);
    }

    private void RemoveCardFromHand(Card card)
    {
        int removedIndex = card.IndexHand;
        Console.WriteLine($"El largo de la mano1 es de {_hand.Count}");
        _hand.RemoveAt(removedIndex);
        Console.WriteLine($"El largo de la mano2 es de {_hand.Count}");
        if (_hand.Count > 0)
        {
            FixIndex(removedIndex);
        }
    }

    private void FixIndex(int removedIndex)
    {   
        var lastCard = _hand[^1];
        for (int i = removedIndex; i < lastCard.IndexHand; i++)
        {
            var card = _hand[i];
            card.IndexHand -= 1;
            _hand[i] = card;
        }
    }
    
    public Card LostCardForDamage()
    {
        Card drawnCard = _arsenal[_arsenal.Count - 1];
        _ringSide.Add(drawnCard);
        _arsenal.RemoveAt(_arsenal.Count - 1);
        
        return drawnCard;
    }
    
    public void StartingDraw()
    {
        int counter = 0;
        while (counter < _superstarCard.HandSize)
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
}