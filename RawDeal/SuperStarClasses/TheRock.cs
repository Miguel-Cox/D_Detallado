namespace RawDeal.SuperStarClasses;
using RawDealView.Formatters;

public class TheRock : SuperstarCard
{
    public TheRock()
    {
        Name = "THE ROCK";
        Logo = "TheRock";
        HandSize = 5;
        SuperstarValue = 5;
        SuperstarAbility =
            "At the start of your turn, before your draw segment, you may take 1 card from your Ringside pile and place it on the bottom of your Arsenal.";
        HasAbilityOption = false;
        HasBeginningAbility = true;
    }

    public override void Ability()
    {
        List<string> RingSideStringCards = GetRingsideStringCards();
        int RingSideSize = GetRingSideCardsSize();
        if (RingSideSize > 0)
        {
            if (View.DoesPlayerWantToUseHisAbility(this.Name))
            {
                View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
                int CardIndex = View.AskPlayerToSelectCardsToRecover(this.Name, 1, RingSideStringCards);
                Player.RecoverCardFromRingSideToArsenal(CardIndex);
            }
        }
    }

    private List<Card> GetRingsideCards()
    {
        List<Card> RingSide = Player.RingSide;
        return RingSide;
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

    private int GetRingSideCardsSize()
    {
        List<Card> RingSide = Player.RingSide;
        return RingSide.Count;
    }
}