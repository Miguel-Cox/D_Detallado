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
        string deckPath = _view.AskUserToSelectDeck(_deckFolder);
        Cards cards = new Cards();
        
        Deck deck1 = new Deck(_view, deckPath, cards);

        if (deck1.CheckValidDeck())
        {
            string deckPath2 = _view.AskUserToSelectDeck(_deckFolder);
            Deck deck2 = new Deck(_view, deckPath2, cards);
            deck2.CheckValidDeck();
        }
        
        
    }
    
}