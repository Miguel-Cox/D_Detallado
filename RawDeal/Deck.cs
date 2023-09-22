using RawDealView;

namespace RawDeal;

public class Deck
{
    public Cards _allCards;
    public string[] _deckCards;
    private View _view;
    public SuperstarCard? _deckSuperstarCard;
    private bool _isHeel;
    private bool _isFace;


    public Deck(View view, string path, Cards allCards)
    {
        _view = view;
        _deckCards = File.ReadAllLines(path);
        _allCards = allCards;
        GetSuperstarCard();
    }


    private void GetSuperstarCard()
    {
        string card = _deckCards[0];
        int cardNewLength = card.Length - 17;
        string superstarName = card.Substring(0, cardNewLength);
        
        SaveSuperstarCard(superstarName);
    }

    private void SaveSuperstarCard(string superstarName)
    {
        foreach (var superstarCard in _allCards.AllSuperstarCards)
        {
            if (superstarCard.Name == superstarName)
            {
                _deckSuperstarCard = superstarCard;
            }
        }
    }

    private bool CheckLength()
    {
        return _deckCards.Length != 61;
    }

    private void ContainHeelOrFace(Card cardInfo)
    {
        if (cardInfo.Subtypes.Contains("Face"))
        {
            _isFace = true;
        }

        if (cardInfo.Subtypes.Contains("Heel"))
        {
            _isHeel = true;
        }
    }

    private bool CheckSubtype()
    {
        foreach (var card in _deckCards.Skip(1))
        {
            Card cardInfo = _allCards.GetCardDataByTitle(card);
            ContainHeelOrFace(cardInfo);
        }

        return (_isHeel & _isFace);
    }

    private bool CheckCount()
    {
        return _deckCards.Skip(1).Any(card =>
        {
            var count = _deckCards.Count(str => str == card);
            var cardInfo = _allCards.GetCardDataByTitle(card);
            return (cardInfo.Subtypes.Contains("Unique") && count > 1) ||
                   (count > 3 && !cardInfo.Subtypes.Contains("SetUp"));
        });
    }

    private bool SuperstarSubtypeCheck(Card cardInfo)
    {
        foreach (SuperstarCard superstarCard in _allCards.AllSuperstarCards)
        {
            if (cardInfo.Subtypes.Contains(superstarCard.Logo) && superstarCard.Logo != _deckSuperstarCard?.Logo)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool CheckSuperstarSubtype()
    {
        foreach (var card in _deckCards.Skip(1))
        {
            var cardInfo = _allCards.GetCardDataByTitle(card);
            if (SuperstarSubtypeCheck(cardInfo))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool CheckValidDeck()
    {
        if (CheckLength() || CheckSubtype() || CheckCount() || CheckSuperstarSubtype())
        {
            _view.SayThatDeckIsInvalid();
            return false;
        }
        
        return true;
    }
}
