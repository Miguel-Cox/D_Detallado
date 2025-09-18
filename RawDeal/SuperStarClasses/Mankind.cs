namespace RawDeal.SuperStarClasses;

public class Mankind : SuperstarCard
{
    public Mankind()
    {
        Name = "MANKIND";
        Logo = "Mankind";
        HandSize = 2;
        SuperstarValue = 4;
        SuperstarAbility =
            "You must always draw 2 cards, if possible, during your draw segment. All damage from opponent is at -1D.";
        HasAbilityOption = false;
        HasBeginningAbility = true;
        UsedAbility = false;
        
    }

    public override void Ability()
    {
        if (HaveAbilityRequirements())
        {
            Player.DrawCard();
        }
    }
    private bool HaveAbilityRequirements()
    {
        return (Player.Arsenal.Count >= 1);
    }
}