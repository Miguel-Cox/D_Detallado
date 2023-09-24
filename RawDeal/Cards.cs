using System.Text.Json;

namespace RawDeal;

public class Cards
{
    public List<Card>? AllCards;
    public List<SuperstarCard>? AllSuperstarCards = new List<SuperstarCard>();
    
    public Cards()
    {
        SaveAllCards();
        SaveAllSuperstarCards();
    }
    
    private void SaveAllCards()
    {
        string cards = Path.Combine("data", "cards.json");
        string allCardsJson = File.ReadAllText(cards);
        AllCards = JsonSerializer.Deserialize<List<Card>>(allCardsJson);
    }
    
    private void SaveAllSuperstarCards()
    {
        AllSuperstarCards?.AddRange(new List<SuperstarCard>
        {
            new StoneCold(),
            new Undertaker(),
            new Mankind(),
            new HHH(),
            new TheRock(),
            new Kane(),
            new Jericho()
        });
    }
    
    public Card GetCardDataByTitle(string title)
    {
        Card card = AllCards.FirstOrDefault(x => x.Title == title);
        return card;
    }
    
}