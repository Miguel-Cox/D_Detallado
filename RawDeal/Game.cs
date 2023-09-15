using System.Text.Json;
using RawDealView;

namespace RawDeal;

public class Game
{
    private View _view;
    private string _deckFolder;
    private List<CardInfo>? allCards;
    private List<SuperstarCardInfo>? allSuperstarCards;
    private string P1Superstar;
    private string[] P1Cards;
    private string[] P1Hand;
    private string P2Superstar;
    private string[] P2Cards;
    private string[] P2Hand;
    private PlayerInfo PlayerInfo1;
    private PlayerInfo PlayerInfo2;
    
    public Game(View view, string deckFolder)
    {
        _view = view;
        _deckFolder = deckFolder;
    }

    private void SaveAllCards()
    {
        string cards = Path.Combine("data", "cards.json");
        string allCardsJson = File.ReadAllText(cards);
        allCards = JsonSerializer.Deserialize<List<CardInfo>>(allCardsJson);
        
        string superstarCards = Path.Combine("data", "superstar.json");
        string allSuperstarCardsJson = File.ReadAllText(superstarCards);
        allSuperstarCards = JsonSerializer.Deserialize<List<SuperstarCardInfo>>(allSuperstarCardsJson);
        
    }
    
    public class CardInfo
    {
        public string Title { get; set; }
        public List<string> Types { get; set; }
        public List<string> Subtypes { get; set; }
        public string Fortitude { get; set; }
        public string Damage { get; set; }
        public string StunValue { get; set; }
        public string CardEffect { get; set; }
    }
    public class SuperstarCardInfo
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public int HandSize { get; set; }
        public int SuperstarValue { get; set; }
        public string SuperstarAbility { get; set; }
    }

    private CardInfo GetCardDataByTitle(string title)
    {
        CardInfo obj = allCards.FirstOrDefault(x => x.Title == title);
        return obj;
    }

    private SuperstarCardInfo GetSuperstarCardDataByName(string name)
    {
        SuperstarCardInfo obj = allSuperstarCards.FirstOrDefault(x => x.Name == name);
        return obj;
    }
    
    private void RegisterDeck(string deck_path, string player)
    {
        string[] cards = File.ReadAllLines(deck_path);
        int cardNewLength = cards[0].Length - 17;
        string superstar = cards[0].Substring(0, cardNewLength);
        
        if (player == "P1")
        {
            P1Superstar = superstar;
            P1Cards = cards.Skip(1).ToArray();
        } else if (player == "P2")
        {
            P2Superstar = superstar;
            P2Cards = cards.Skip(1).ToArray();
        }
        
    }
    private bool CheckValidDeck(string deck_path)
    {
            string[] cards = File.ReadAllLines(deck_path);
            string[] superstarCardsLogos = {"StoneCold", "Undertaker", "Mankind", "HHH", "TheRock", "Kane", "Jericho"};
            string mySuperstarLogo = new string("");
            bool isHeel = false;
            bool isFace = false;
            
            if (cards.Length != 61)
            {   
                _view.SayThatDeckIsInvalid();
                return false;
            }

            bool skipFirst = true;
            foreach (var card in cards)
            {
                if (skipFirst) // Obtenemos primero la carta superstar
                {
                    int cardNewLength = card.Length - 17;
                    string superstarname = card.Substring(0, cardNewLength);
                    SuperstarCardInfo SP = GetSuperstarCardDataByName(superstarname);
                    mySuperstarLogo = SP.Logo;
                    skipFirst = false;
                    continue;
                }

                CardInfo DataInfo = GetCardDataByTitle(card);

                // Vemos si hay solo hay Heel o Face
                if (DataInfo.Subtypes.Contains("Face"))
                {
                    isFace = true;
                }
                if (DataInfo.Subtypes.Contains("Heel"))
                {
                    isHeel = true;
                }
                
                if (isHeel & isFace)
                {
                    _view.SayThatDeckIsInvalid();
                    return false;
                }
                
                // Vemos si esta repetida más de 3 veces, cumpliendo Unique y Setup
                int count = cards.Count(str => str == card);

                if ((DataInfo.Subtypes.Contains("Unique") & count > 1) || count > 3 & !DataInfo.Subtypes.Contains("SetUp"))
                {
                    _view.SayThatDeckIsInvalid();
                    return false;
                }
                
                // Vemos si tiene el logo de algun otro superstar
                foreach (var logo in superstarCardsLogos)
                {
                    if (DataInfo.Subtypes.Contains(logo) & logo != mySuperstarLogo)
                    {
                        _view.SayThatDeckIsInvalid();
                        return false;
                    }
                }
            }
            
            return true;
    }

    private void RegisterPlayers()
    {
        SuperstarCardInfo SP1 = GetSuperstarCardDataByName(P1Superstar);
        SuperstarCardInfo SP2 = GetSuperstarCardDataByName(P2Superstar);
        PlayerInfo P1 = new PlayerInfo(P1Superstar, 0, SP1.HandSize, 60 - SP1.HandSize);
        PlayerInfo P2 = new PlayerInfo(P2Superstar, 0, SP2.HandSize, 60 - SP2.HandSize);
        
        PlayerInfo1 = P1;
        PlayerInfo2 = P2;
    }
    
    public void StartGame()
    {   
        SuperstarCardInfo SP1 = GetSuperstarCardDataByName(P1Superstar);
        SuperstarCardInfo SP2 = GetSuperstarCardDataByName(P2Superstar);

        if (SP2.SuperstarValue > SP1.SuperstarValue)
        {   
            _view.SayThatATurnBegins(P2Superstar);
            PlayerInfo2.NumberOfCardsInHand++;
            PlayerInfo2.NumberOfCardsInArsenal--;
            _view.ShowGameInfo(PlayerInfo2, PlayerInfo1);
            
            // Cambiar esto, esta muy feo
            string selectedOption = _view.ShowPlayerOptions();
            if (selectedOption == "4")
            {
                _view.CongratulateWinner(P1Superstar);
            }
        }
        else
        {
            _view.SayThatATurnBegins(P1Superstar);
            PlayerInfo1.NumberOfCardsInHand++;
            PlayerInfo1.NumberOfCardsInArsenal--;
            _view.ShowGameInfo(PlayerInfo1, PlayerInfo2);
            
            // Cambiar esto, esta muy feo
            string selectedOption = _view.ShowPlayerOptions();
            if (selectedOption == "4")
            {
                _view.CongratulateWinner(P2Superstar);
            }
        }
        
        

    }
    
    public void Play()
    {   
        SaveAllCards();
        string deck_path1 = _view.AskUserToSelectDeck(_deckFolder);
        bool validDeck1 = CheckValidDeck(deck_path1);

        if (validDeck1)
        {   
            RegisterDeck(deck_path1, "P1");
            string deck_path2 = _view.AskUserToSelectDeck(_deckFolder);
            bool validDeck2 = CheckValidDeck(deck_path2);
            if (validDeck2)
            {
                RegisterDeck(deck_path2, "P2");
                RegisterPlayers();
                StartGame();
            }
        }
    }
}