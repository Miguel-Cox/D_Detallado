using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;

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
    private Player _currentPlayer;
    private Player _waitingPlayer;

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
            StartGame();
        }
    }

    private void StartGame()
    {
        _currentPlayer = _player1;
        _waitingPlayer = _player2;
        _player1.StartingDraw();
        _player2.StartingDraw();
        Turns();
    }

    private void ShowGameInfo()
    {
        _view.ShowGameInfo(_player1.PlayerInfo, _player2.PlayerInfo);
    }
    
    private void SwitchPlayers()
    {
        (_currentPlayer, _waitingPlayer) = (_waitingPlayer, _currentPlayer);
    }
    
    private void Turns()
    {
        _view.SayThatATurnBegins(_currentPlayer._superstarCard.Name);
        ShowGameInfo();
        var selected = _currentPlayer.ShowPlayOptions();
        DelegateDuties(selected);
    }
    
    private void DelegateDuties(string selected)
    {
        int number = int.Parse(selected);
        
        switch (number)
        {
            case 1:
                ChooseCardsToSee();
                break;
            case 2:
                // Function2();
                break;
            case 3:
                // Function3();
                break;
            case 4:
                _currentPlayer.Surrender();
                break;
            default:
                break;
        }
    }

    private void ChooseCardsToSee()
    {
        CardSet cardset = _view.AskUserWhatSetOfCardsHeWantsToSee();
        
        switch (cardset)
        {
            case CardSet.Hand:
                PrintCards(_currentPlayer._hand);
                break;
            case CardSet.RingArea:
                PrintCards(_currentPlayer._ringArea);
                break;
            case CardSet.RingsidePile:
                PrintCards(_currentPlayer._ringSide);
                break;
            case CardSet.OpponentsRingArea:
                PrintCards(_waitingPlayer._ringArea);
                break;
            case CardSet.OpponentsRingsidePile:
                PrintCards(_waitingPlayer._ringSide);
                break;
        }
        
    }

    private void PrintCards(List<Card> cardList)
    {
        List<string> stringList = new List<string>();
        foreach (var card in cardList)
        {
            string cardString = Formatter.CardToString(card);
            stringList.Add(cardString);
        }
        _view.ShowCards(stringList);
    }
}