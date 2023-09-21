using System.Text.Json;
using RawDealView;

namespace RawDeal
{
    public class Game
    {
        private readonly View _view;
        private readonly string _deckFolder;
        private List<CardInfo>? _allCards;
        private List<SuperstarCardInfo>? _allSuperstarCards;
        private string _p1Superstar;
        private string[] _p1Cards;
        private string[] _p1Hand;
        private string _p2Superstar;
        private string[] _p2Cards;
        private string[] _p2Hand;
        private PlayerInfo _playerInfo1;
        private PlayerInfo _playerInfo2;

        public Game(View view, string deckFolder)
        {
            _view = view;
            _deckFolder = deckFolder;
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


        private void SaveAllCards()
        {
            string cardsPath = Path.Combine("data", "cards.json");
            string allCardsJson = File.ReadAllText(cardsPath);
            _allCards = JsonSerializer.Deserialize<List<CardInfo>>(allCardsJson);

            string superstarCardsPath = Path.Combine("data", "superstar.json");
            string allSuperstarCardsJson = File.ReadAllText(superstarCardsPath);
            _allSuperstarCards = JsonSerializer.Deserialize<List<SuperstarCardInfo>>(allSuperstarCardsJson);
        }

        private CardInfo GetCardDataByTitle(string title)
        {
            return _allCards.FirstOrDefault(x => x.Title == title);
        }

        private SuperstarCardInfo GetSuperstarCardDataByName(string name)
        {
            return _allSuperstarCards.FirstOrDefault(x => x.Name == name);
        }

        private bool CheckValidDeck(string deckPath)
        {
            string[] cards = File.ReadAllLines(deckPath);
            string[] superstarCardsLogos = {"StoneCold", "Undertaker", "Mankind", "HHH", "TheRock", "Kane", "Jericho"};
            string mySuperstarLogo = "";

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
                    string superstarName = card.Substring(0, cardNewLength);
                    SuperstarCardInfo superstarCardInfo = GetSuperstarCardDataByName(superstarName);
                    mySuperstarLogo = superstarCardInfo.Logo;
                    skipFirst = false;
                    continue;
                }

                CardInfo cardInfo = GetCardDataByTitle(card);

                // Vemos si hay solo hay Heel o Face
                if (cardInfo.Subtypes.Contains("Face"))
                {
                    isFace = true;
                }
                if (cardInfo.Subtypes.Contains("Heel"))
                {
                    isHeel = true;
                }

                if (isHeel && isFace)
                {
                    _view.SayThatDeckIsInvalid();
                    return false;
                }

                // Vemos si está repetida más de 3 veces, cumpliendo Unique y Setup
                int count = cards.Count(str => str == card);

                if ((cardInfo.Subtypes.Contains("Unique") && count > 1) || (count > 3 && !cardInfo.Subtypes.Contains("SetUp")))
                {
                    _view.SayThatDeckIsInvalid();
                    return false;
                }

                // Vemos si tiene el logo de algún otro superstar
                foreach (var logo in superstarCardsLogos)
                {
                    if (cardInfo.Subtypes.Contains(logo) && logo != mySuperstarLogo)
                    {
                        _view.SayThatDeckIsInvalid();
                        return false;
                    }
                }
            }

            return true;
        }

        private void RegisterDeck(string deckPath, string player)
        {
            string[] cards = File.ReadAllLines(deckPath);
            int cardNewLength = cards[0].Length - 17;
            string superstar = cards[0].Substring(0, cardNewLength);

            if (player == "P1")
            {
                _p1Superstar = superstar;
                _p1Cards = cards.Skip(1).ToArray();
            }
            else if (player == "P2")
            {
                _p2Superstar = superstar;
                _p2Cards = cards.Skip(1).ToArray();
            }
        }

        private void RegisterPlayers()
        {
            SuperstarCardInfo sp1 = GetSuperstarCardDataByName(_p1Superstar);
            SuperstarCardInfo sp2 = GetSuperstarCardDataByName(_p2Superstar);
            _playerInfo1 = new PlayerInfo(_p1Superstar, 0, sp1.HandSize, 60 - sp1.HandSize);
            _playerInfo2 = new PlayerInfo(_p2Superstar, 0, sp2.HandSize, 60 - sp2.HandSize);
        }

        public void StartGame()
        {
            SuperstarCardInfo sp1 = GetSuperstarCardDataByName(_p1Superstar);
            SuperstarCardInfo sp2 = GetSuperstarCardDataByName(_p2Superstar);

            if (sp2.SuperstarValue > sp1.SuperstarValue)
            {
                _view.SayThatATurnBegins(_p2Superstar);
                _playerInfo2.NumberOfCardsInHand++;
                _playerInfo2.NumberOfCardsInArsenal--;
                _view.ShowGameInfo(_playerInfo2, _playerInfo1);

                string selectedOption = _view.ShowPlayerOptions();
                if (selectedOption == "4")
                {
                    _view.CongratulateWinner(_p1Superstar);
                }
            }
            else
            {
                _view.SayThatATurnBegins(_p1Superstar);
                _playerInfo1.NumberOfCardsInHand++;
                _playerInfo1.NumberOfCardsInArsenal--;
                _view.ShowGameInfo(_playerInfo1, _playerInfo2);

                string selectedOption = _view.ShowPlayerOptions();
                if (selectedOption == "4")
                {
                    _view.CongratulateWinner(_p2Superstar);
                }
            }
        }

        public void Play()
        {
            SaveAllCards();
            string deckPath1 = _view.AskUserToSelectDeck(_deckFolder);
            bool validDeck1 = CheckValidDeck(deckPath1);

            if (validDeck1)
            {
                RegisterDeck(deckPath1, "P1");
                string deckPath2 = _view.AskUserToSelectDeck(_deckFolder);
                bool validDeck2 = CheckValidDeck(deckPath2);

                if (validDeck2)
                {
                    RegisterDeck(deckPath2, "P2");
                    RegisterPlayers();
                    StartGame();
                }
            }
        }
    }
}
