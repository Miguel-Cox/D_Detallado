namespace RawDeal.SuperStarClasses;
using RawDealView.Formatters;

public class Undertaker : SuperstarCard
{
    public Undertaker()
    {
        Name = "THE UNDERTAKER";
        Logo = "Undertaker";
        HandSize = 6;
        SuperstarValue = 4;
        SuperstarAbility =
            "Once during your turn, you may discard 2 cards to the Ringside pile and take 1 card from the Ringside pile and place it into your hand.";
        HasAbilityOption = true;
        HasBeginningAbility = false;
        UsedAbility = false;
    }

    public override void Ability()
    {
        View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
        DiscardTwoCardsFromHand();
        RecoverCardFromRingSide();
        this.UsedAbility = true;
        
    }
    
    private void RecoverCardFromRingSide()
    {   
        List<string> RingSideStringCards = GetRingsideStringCards();
        int CardIndex = View.AskPlayerToSelectCardsToPutInHisHand(this.Name, 1, RingSideStringCards);
        Player.RecoverCardFromRingSideToHand(CardIndex);
    }
    
    private List<string> GetRingsideStringCards()
    {
        List<Card> RingSideCards = GetRingsideCards();
        List<string> stringCards = new List<string>();
        foreach (var card in RingSideCards)
        {
            string cardString = Formatter.CardToString(card);
            stringCards.Add(cardString);
        }

        return stringCards;
    }
    
    private List<Card> GetRingsideCards()
    {
        List<Card> RingSide = Player.RingSide;
        return RingSide;
    }
    
    private void DiscardTwoCardsFromHand()
    {
        for (int i = 2; i > 0; i--)
        {
            List<string> HandStringCards = GetHandStringCards();
            this.UsedAbility = true;
            int CardIndex = View.AskPlayerToSelectACardToDiscard(HandStringCards, this.Name, 
                this.Name, i);
            Player.DiscardCardFromHand(CardIndex);
        }
    }
    public override bool HaveAbilityRequirements()
    {
        return (UsedAbility == false && Player.Hand.Count >= 2);
    }

    private List<string> GetHandStringCards()
    {
        List<Card> HandCards = GetHandCards();
        List<string> stringCards = new List<string>();
        foreach (var card in HandCards)
        {
            string cardString = Formatter.CardToString(card);
            stringCards.Add(cardString);
        }

        return stringCards;
    }

    private List<Card> GetHandCards()
    {
        List<Card> Hand = Player.Hand;
        return Hand;
    }
}