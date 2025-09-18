namespace RawDeal.SuperStarClasses;
using RawDealView.Formatters;

public class StoneCold : SuperstarCard
{
    public StoneCold()
    {
        Name = "STONE COLD STEVE AUSTIN";
        Logo = "StoneCold";
        HandSize = 7;
        SuperstarValue = 5;
        SuperstarAbility =
            "Once during your turn, you may draw a card, but you must then take a card from your hand and place it on the bottom of your Arsenal.";
        HasAbilityOption = true;
        HasBeginningAbility = false;
        UsedAbility = false;
    }

    public override void Ability()
    {
        if (HaveAbilityRequirements())
        {   
            View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
            Player.DrawCard();
            List<string> HandStringCards = GetHandStringCards();
            View.SayThatPlayerDrawCards(this.Name, 1);
            int CardIndex = View.AskPlayerToReturnOneCardFromHisHandToHisArsenal(this.Name, HandStringCards);
            Player.RemoveCardFromHandToArsenal(CardIndex);
            this.UsedAbility = true;
        }
    }
    public override bool HaveAbilityRequirements()
    {
        return (UsedAbility == false && Player.Arsenal.Count > 0);
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