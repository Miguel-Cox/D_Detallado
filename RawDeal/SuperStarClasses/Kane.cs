namespace RawDeal.SuperStarClasses;

public class Kane : SuperstarCard
{
    public Kane()
    {
        Name = "KANE";
        Logo = "Kane";
        HandSize = 7;
        SuperstarValue = 2;
        SuperstarAbility =
            "At the start of your turn, before your draw segment, opponent must take the top card from his Arsenal and place it into his Ringside pile.";
        HasAbilityOption = false;
        HasBeginningAbility = true;
    }

    public override void Ability()
    {
        View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
        View.SayThatSuperstarWillTakeSomeDamage(Game.WaitingPlayer.Name, 1);
        Game.DamageOpponent(1, 1);
    }
}