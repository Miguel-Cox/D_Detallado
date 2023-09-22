using RawDealView;

namespace RawDeal;

public class Game
{
    private View _view;
    private string _deckFolder;
    
    public Game(View view, string deckFolder)
    {
        _view = view;
        _deckFolder = deckFolder;
    }

    public void Play()
    {   
        string deckPath1 = _view.AskUserToSelectDeck(_deckFolder);
        Console.WriteLine(deckPath1);
        Cards cards = new Cards();
        
        Deck deck1 = new Deck(_view, deckPath1, cards);

        deck1.CheckValidDeck();
    }
    
}