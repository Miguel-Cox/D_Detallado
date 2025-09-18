namespace RawDeal.SuperStarClasses;

using RawDealView;

public class SuperstarCard
{
    public string Name { get; set; }
    public string Logo { get; set; }
    public int HandSize { get; set; }
    public int SuperstarValue { get; set; }
    public string SuperstarAbility { get; set; }

    public bool HasAbilityOption { get; set; }

    public Player Player { get; set; }
    public Game Game { get; set; }
    public bool UsedAbility { get; set; }

    public bool HasBeginningAbility { get; set; }

    public View View;

    public virtual void Ability()
    {
        View.SayThatPlayerIsGoingToUseHisAbility(this.Name, this.SuperstarAbility);
    }

    public virtual bool HaveAbilityRequirements()
    {
        return true;
    }
}