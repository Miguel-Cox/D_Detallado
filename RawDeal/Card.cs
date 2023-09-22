using RawDealView.Formatters;

namespace RawDeal;

public struct Card : IViewableCardInfo
{
    public string Title { get; }
    public string Fortitude { get; }
    public string Damage { get; }
    public string StunValue { get; }
    public List<string> Types { get; }
    public List<string> Subtypes { get; }
    public string CardEffect { get; }
    
}