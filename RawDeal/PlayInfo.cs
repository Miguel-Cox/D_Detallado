using RawDealView.Formatters;

namespace RawDeal;

public struct PlayInfo : IViewablePlayInfo
{
    public IViewableCardInfo CardInfo { get; init; }
    public string PlayedAs { get; init; }
}