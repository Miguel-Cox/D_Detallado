using RawDealView;
using RawDeal.SuperStarClasses;

namespace RawDeal;

public class Deck
{
    public Cards AllCards;
    public string[] DeckCards;
    private View _view;
    public SuperstarCard? DeckSuperstarCard;
    private bool _isHeel;
    private bool _isFace;


    public Deck(View view, string path, Cards allCards)
    {
        _view = view;
        DeckCards = File.ReadAllLines(path);
        AllCards = allCards;
        GetSuperstarCard();
    }


    private void GetSuperstarCard()
    {
        string card = DeckCards[0];
        int cardNewLength = card.Length - 17;
        string superstarName = card.Substring(0, cardNewLength);
        
        SaveSuperstarCard(superstarName);
    }

    private void SaveSuperstarCard(string superstarName)
    {
        foreach (var superstarCard in AllCards.AllSuperstarCards)
        {
            if (superstarCard.Name == superstarName)
            {
                DeckSuperstarCard = superstarCard;
            }
        }
    }

    private bool CheckLength()
    {
        var correctLength = 61;
        return DeckCards.Length != correctLength;
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
        foreach (var card in DeckCards.Skip(1))
        {
            Card cardInfo = AllCards.GetCardDataByTitle(card);
            ContainHeelOrFace(cardInfo);
        }

        return (_isHeel & _isFace);
    }

    private bool CheckCount()
    {
        return DeckCards.Skip(1).Any(card =>
        {
            const int maxHandCount = 3;
            const int maxCountUniques = 1;
            var count = DeckCards.Count(str => str == card);
            var cardInfo = AllCards.GetCardDataByTitle(card);
            return (cardInfo.Subtypes.Contains("Unique") && count > maxCountUniques) ||
                   (count > maxHandCount && !cardInfo.Subtypes.Contains("SetUp"));
        });
    }

    private bool SuperstarSubtypeCheck(Card cardInfo)
    {
        foreach (SuperstarCard superstarCard in AllCards.AllSuperstarCards)
        {
            if (IsSuperstarSubtypeMatch(cardInfo, superstarCard))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsSuperstarSubtypeMatch(Card cardInfo, SuperstarCard superstarCard)
    {
        return cardInfo.Subtypes.Contains(superstarCard.Logo) && superstarCard.Logo != DeckSuperstarCard?.Logo;
    }
    
    private bool CheckSuperstarSubtype()
    {
        foreach (var card in DeckCards.Skip(1))
        {
            var cardInfo = AllCards.GetCardDataByTitle(card);
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
