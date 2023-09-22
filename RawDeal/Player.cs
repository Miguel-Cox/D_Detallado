using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;

namespace RawDeal;

public class Player
{
    private View _view;
    private Deck _deck;
    private String _name;
    public bool _surrender;
    public List<Card> _hand = new List<Card>();
    private List<Card> _arsenal = new List<Card>();
    public List<Card> _ringArea = new List<Card>();
    public List<Card> _ringSide = new List<Card>();
    private int fortitude;
    public SuperstarCard _superstarCard;
    
    public PlayerInfo PlayerInfo => new(_superstarCard.Name, fortitude, _hand.Count, _arsenal.Count);

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
    
    public string ShowPlayOptions()
    {
        string selectedOption = _view.ShowPlayerOptions();
        return selectedOption;
    }
    

    public void Surrender()
    {
        _surrender = true;
    }
}