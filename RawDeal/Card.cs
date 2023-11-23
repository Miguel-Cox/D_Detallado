using RawDealView.Formatters;

namespace RawDeal;

public struct Card : IViewableCardInfo
{
    public string Title { get; set; }
    public string Fortitude { get; set; }
    public string Damage { get; set; }
    public string StunValue { get; set; }
    public List<string> Types { get; set; }
    public List<string> Subtypes { get; set; }
    public string CardEffect { get; set; }
    public int IndexHand { get; set; }
    public int IndexRingSide { get; set; }
    
    public string TypeUsed { get; set; }
}