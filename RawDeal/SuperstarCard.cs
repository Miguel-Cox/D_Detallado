namespace RawDeal;

public class SuperstarCard
{
    public string Name { get; set; }
    public string Logo { get; set; }
    public int HandSize { get; set; }
    public int SuperstarValue { get; set; }
    public string SuperstarAbility { get; set; }
}

public class StoneCold : SuperstarCard
{
    public StoneCold()
    {
        Name = "STONE COLD STEVE AUSTIN";
        Logo = "StoneCold";
        HandSize = 7;
        SuperstarValue = 5;
        SuperstarAbility = "Once during your turn, you may draw a card, but you must then take a card from your hand and place it on the bottom of your Arsenal.";
    }
}

public class Undertaker : SuperstarCard
{
    public Undertaker()
    {
        Name = "THE UNDERTAKER";
        Logo = "Undertaker";
        HandSize = 6;
        SuperstarValue = 4;
        SuperstarAbility = "Once during your turn, you may discard 2 cards to the Ringside pile and take 1 card from the Ringside pile and place it into your hand.";
    }
}

public class Mankind : SuperstarCard
{
    public Mankind()
    {
        Name = "MANKIND";
        Logo = "Mankind";
        HandSize = 2;
        SuperstarValue = 4;
        SuperstarAbility = "You must always draw 2 cards, if possible, during your draw segment. All damage from opponent is at -1D.";
    }
}

public class HHH : SuperstarCard
{
    public HHH()
    {
        Name = "HHH";
        Logo = "HHH";
        HandSize = 10;
        SuperstarValue = 3;
        SuperstarAbility = "None, isn't the starting HandSize enough! He is \"The Game\" after all!";
    }
}

public class TheRock : SuperstarCard
{
    public TheRock()
    {
        Name = "THE ROCK";
        Logo = "TheRock";
        HandSize = 5;
        SuperstarValue = 5;
        SuperstarAbility = "At the start of your turn, before your draw segment, you may take 1 card from your Ringside pile and place it on the bottom of your Arsenal.";
    }
}

public class Kane : SuperstarCard
{
    public Kane()
    {
        Name = "KANE";
        Logo = "Kane";
        HandSize = 7;
        SuperstarValue = 2;
        SuperstarAbility = "At the start of your turn, before your draw segment, opponent must take the top card from his Arsenal and place it into his Ringside pile.";
    }
}

public class Jericho : SuperstarCard
{
    public Jericho()
    {
        Name = "CHRIS JERICHO";
        Logo = "Jericho";
        HandSize = 7;
        SuperstarValue = 3;
        SuperstarAbility = "Once during your turn, you may discard a card from your hand to force your opponent to discard a card from his hand.";
    }
}
