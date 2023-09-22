using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;

namespace RawDeal;

public class Player
{
    private View _view;
    private Deck _deck;
    private String _name;
    private List<Card> _hand = new List<Card>();
    private List<Card> _arsenal = new List<Card>();
    private List<Card> _ringSide = new List<Card>();
    private List<Card> _ringArea = new List<Card>();
    private int fortitude;
    private SuperstarCard _superstarCard;
    
    public Player(View view, Deck deck)
    {
        _view = view;
        _deck = deck;
        SaveCards();
    }

    private void SaveCards()
    {
        _superstarCard = _deck._deckSuperstarCard;
        foreach (var card in _deck._deckCards.Skip(1))
        {
            _arsenal.Add(_deck._allCards.GetCardDataByTitle(card));
        }
        
    }

    public void DrawCard()
    {
        if (_arsenal.Count > 0)
        {
            Card drawnCard = _arsenal[_arsenal.Count - 1];
            _hand.Add(drawnCard);
            _arsenal.RemoveAt(_arsenal.Count - 1);
        }
    }

    public void StartingDraw()
    {
        int counter = 0;
        while (counter < _superstarCard.SuperstarValue)
        {
            DrawCard();
            counter++;
        }
    }

    public void WatchCards()
    {
        CardSet cardset = _view.AskUserWhatSetOfCardsHeWantsToSee();
    }
}