namespace RawDealView;

public class PlayerInfo
{
    private string _superstarName;
    private int _fortitudeRating;
    private int _numberOfCardsInHand;
    private int _numberOfCardsInArsenal;
    
    public int NumberOfCardsInHand
    {
        get { return _numberOfCardsInHand; }
        set { _numberOfCardsInHand = value; }
    }
    public int NumberOfCardsInArsenal
    {
        get { return _numberOfCardsInArsenal; }
        set { _numberOfCardsInArsenal = value; }
    }
    public PlayerInfo(string superstarName, int fortitudeRating, int numberOfCardsInHand, int numberOfCardsInArsenal)
    {
        _superstarName = superstarName;
        _fortitudeRating = fortitudeRating;
        _numberOfCardsInHand = numberOfCardsInHand;
        _numberOfCardsInArsenal = numberOfCardsInArsenal;
    }

    public override string ToString()
        => $"{_superstarName}: {_fortitudeRating}F, tiene {_numberOfCardsInHand} cartas en la mano y {_numberOfCardsInArsenal} en el arsenal.";
}