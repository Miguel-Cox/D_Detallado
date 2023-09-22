using RawDealView;

namespace RawDeal;

public class Game
{
    private View _view;
    private string _deckFolder;
    private Cards _cards = new Cards();
    private Deck _deck1;
    private Deck _deck2;
    private Player _player1;
    private Player _player2;

    public Game(View view, string deckFolder)
    {
        _view = view;
        _deckFolder = deckFolder;
    }

    private bool CheckStartRequirements()
    {
        string deckPath = _view.AskUserToSelectDeck(_deckFolder);
        _deck1 = new Deck(_view, deckPath, _cards);

        if (_deck1.CheckValidDeck())
        {
            string deckPath2 = _view.AskUserToSelectDeck(_deckFolder);
            _deck2 = new Deck(_view, deckPath2, _cards);
            return _deck2.CheckValidDeck();
        }

        return false;
    }

    private Deck GetStarterDeck()
    {
        if (_deck2._deckSuperstarCard.SuperstarValue > _deck1._deckSuperstarCard.SuperstarValue)
        {
            return _deck2;
        }
        else return _deck1;
    }


    private void CreatePlayers()
    {
        Deck p1Deck = GetStarterDeck();
        Deck p2Deck = (p1Deck == _deck1) ? _deck2 : _deck1;

        _player1 = new Player(_view, p1Deck);
        _player2 = new Player(_view, p2Deck);
    }

    public void Play()
    {
        if (CheckStartRequirements())
        {
            CreatePlayers();
            _player1.StartingDraw();
            _player1.WatchCards();
        }
    }
}