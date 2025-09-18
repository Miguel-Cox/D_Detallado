namespace RawDeal.SuperStarClasses;
using RawDealView.Formatters;

public class Jericho : SuperstarCard
{
    public Jericho()
    {
        Name = "CHRIS JERICHO";
        Logo = "Jericho";
        HandSize = 7;
        SuperstarValue = 3;
        SuperstarAbility =
            "Once during your turn, you may discard a card from your hand to force your opponent to discard a card from his hand.";
        HasAbilityOption = true;
        HasBeginningAbility = false;
        UsedAbility = false;
    }

    public override void Ability()
    {
        View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
        DiscardCardFromHand();
        Game.DiscardOpponentCard();
        this.UsedAbility = true;
    }
    
    public override bool HaveAbilityRequirements()
    {
        return (UsedAbility == false && Player.Hand.Count >= 1);
    }
    
    private void DiscardCardFromHand()
    {
        List<string> HandStringCards = GetHandStringCards();
        int CardIndex = View.AskPlayerToSelectACardToDiscard(HandStringCards, this.Name, 
            this.Name, 1);
        Player.DiscardCardFromHand(CardIndex);
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