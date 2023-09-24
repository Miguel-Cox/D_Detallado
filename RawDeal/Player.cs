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
    public int Fortitude;
    private SuperstarCard _superstarCard;

    public PlayerInfo PlayerInfo => new(_superstarCard.Name, Fortitude, Hand.Count, Arsenal.Count);

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
            Arsenal.Add(_deck._allCards.GetCardDataByTitle(card));
        }
    }

    private void SaveName()
    {
        Name = _superstarCard.Name;
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

    private bool IsArsenalEmpty()
    {
        if (Arsenal.Count == 0)
        {
            return true;
        }
        else return false;
    }

    public void PlayCard(Card card)
    {
        RemoveCardFromHand(card);
        int aditionalFortitude = int.Parse(card.Damage);
        Fortitude += aditionalFortitude;
        RingArea.Add(card);
    }

    private void RemoveCardFromHand(Card card)
    {
        int removedIndex = card.IndexHand;
        Hand.RemoveAt(removedIndex);
        if (Hand.Count > 0)
        {
            FixIndex(removedIndex);
        }
    }

    private void FixIndex(int removedIndex)
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