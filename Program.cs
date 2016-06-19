using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoppyakkenApplication
{
    [Flags]
    enum Pattern : long
    {
        MatsuAndTsuru = 0x1,
        MatsuAndTanzakuAka = 0x2,
        Matsu1 = 0x4,
        Matsu2 = 0x8,
        UmeAndUguisu = 0x10,
        UmeAndTanzakuAka = 0x20,
        Ume1 = 0x40,
        Ume2 = 0x80,
        SakuraAndMaku = 0x100,
        SakuraAndTanzakuAka = 0x200,
        Sakura1 = 0x400,
        Sakura2 = 0x800,
        FujiAndHototogisu = 0x1000,
        FujiAndTanzakuAka = 0x2000,
        Fuji1 = 0x4000,
        Fuji2 = 0x8000,
        AyameAndHashi = 0x10000,
        AyameAndTanzakuAka = 0x20000,
        Ayame1 = 0x40000,
        Ayame2 = 0x80000,
        BotanAndChou = 0x100000,
        BotanAndTanzakuAo = 0x200000,
        Botan1 = 0x400000,
        Botan2 = 0x800000,
        HagiAndInoshishi = 0x1000000,
        HagiAndTanzakuAka = 0x2000000,
        Hagi1 = 0x4000000,
        Hagi2 = 0x8000000,
        SusukiAndTsuki = 0x10000000,
        SusukiAndGan = 0x20000000,
        Susuki1 = 0x40000000,
        Susuki2 = 0x80000000,
        KikuAndOchoko = 0x100000000,
        KikuAndTanzakuAo = 0x200000000,
        Kiku1 = 0x400000000,
        Kiku2 = 0x800000000,
        MomijiAndShika = 0x1000000000,
        MomijiAndTanzakuAo = 0x2000000000,
        Momiji1 = 0x4000000000,
        Momiji2 = 0x8000000000,
        YanagiAndKaeru = 0x10000000000,
        YanagiAndTsubame = 0x20000000000,
        YanagiAndTanzakuAka = 0x40000000000,
        Yanagi = 0x80000000000,
        KiriAndHouou = 0x100000000000,
        KiriAndYellow = 0x200000000000,
        Kiri1 = 0x400000000000,
        Kiri2 = 0x800000000000
    }
    class MainProgram
    {
        static void Main(string[] args)
        {
            // プレイ人数2人。
            new Manager(2);

            Console.ReadKey(true);
        }
    }
    /// <summary>
    /// Cardメンバーの集合を現す抽象クラス。
    /// </summary>
    abstract class CardSet
    {
        public CardSet(string name, Yamafuda yamafuda)
        {
            Name = name;
            this.yamafuda = yamafuda;
        }
        protected readonly Yamafuda yamafuda;
        public string Name { get; set; }
        /// <summary>
        /// Handメンバーはコンソールに手札を表示するための要素である。
        /// </summary>
        protected Pattern hand;
        public Pattern Hand { get { return hand; } }
        /// <summary>
        /// CardsメンバーはPlayerクラスによる手札の実装である。
        /// </summary>
        protected List<Card> cards = new List<Card>();
        public List<Card> Cards { get { return cards; } }
        public void AddCard(Card card)
        {
            // require
            if (cards == null) throw new ArgumentNullException();

            this.cards.Add(card);
            // ビットを立てる。
            hand |= card.CardPattern;
        }
        public void RemoveCard(Card card)
        {
            // require
            if (cards == null) throw new ArgumentNullException();
            if (cards.Count <= 0) throw new Exception("CardSet.Cardsが空です。");

            cards.Remove(card);
            // ビットを落とす。
            hand &= ~card.CardPattern;
        }
        public Pattern GetCardPattern(int index)
        {
            return cards[index].CardPattern;
        }
        /// <summary>
        /// Playerクラスの開始状態の構築。呼び出し回数はプレイ人数によって決定する。
        /// </summary>
        /// <param name="yamafuda"></param>
        public void TakeToYamafuda()
        {
            // require
            if (yamafuda.CardStack == null) throw new ArgumentNullException();
            if (yamafuda.CardStack.Count == 0) throw new Exception("CardStackが空です。");

            Card card = yamafuda.CardPop();
            AddCard(card);
        }
        /// <summary>
        /// Playerクラスの開始状態の構築。count回数だけCardをPopする。呼び出し回数はプレイ人数によって決定する。
        /// </summary>
        /// <param name="yamafuda"></param>
        /// <param name="count">Popする回数</param>
        public void TakeToYamafuda(int count)
        {
            // requie
            if (count <= 0) throw new ArgumentOutOfRangeException();
            if (yamafuda.CardStack == null) throw new ArgumentNullException();
            if (yamafuda.CardStack.Count == 0) throw new Exception("CardStackが空です。");

            while (count-- != 0)
                TakeToYamafuda();
        }
        /// <summary>
        /// 引数のカードに対し、Manth属性が一致する一枚のCardを返す。
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Card WhereFristToManth(Card card)
        {
            // require
            if (cards == null) throw new ArgumentNullException();
            if (cards.Count <= 0) throw new Exception("CardSet.Cardsが空です。");

            return cards.Where(firstCard => firstCard.Manth == card.Manth).First();
        }
    }
    /// <summary>
    /// PlayerクラスのCardsメンバーの開始状態はゲームのプレイ人数に影響されるため、初期化は遅延する。
    /// </summary>
    class Player : CardSet
    {
        public Player(string name, Yamafuda yamafuda) : base(name, yamafuda)
        {
        }
        /// <summary>
        /// 得点となる札。
        /// </summary>
        private List<Card> kirifuda = new List<Card>();
        public List<Card> Kirifuda { get { return kirifuda; } }
        public void AddKirifuda(Card card)
        {
            // require
            if (kirifuda == null) throw new ArgumentNullException();

            kirifuda.Add(card);
        }
        /// <summary>
        /// 現在の得点。
        /// </summary>
        public int Score { get; set; }
        public State PlayerState { get; set; }
        public Handle PlayerHandle { get; set; }
        /*
        /// <summary>
        /// haguriCardのManth属性が、BafudaクラスのCardsのManth属性と一致しない場合、haguriCardをBafudaのCardsに追加する。
        /// </summary>
        /// <param name="haguriCard">山札から引いたCard。</param>
        /// <param name="bafuda"></param>
        public void PutToBafuda(Card haguriCard, Bafuda bafuda)
        {
            // requre
            if (bafuda.Cards == null) throw new ArgumentNullException();

            bafuda.AddCard(haguriCard);
        }
        /// <summary>
        /// haguriCardのManth属性が、BafudaクラスのCardsのManth属性と一致する場合（isMatchManth == trueの場合）、
        /// 一致する1枚のCardとhaguriCardをPlayerクラスのkirifudaに加える。
        /// </summary>
        /// <param name="haguriCard">山札から引いたCard。</param>
        /// <param name="bafuda"></param>
        public void GetToBafuda(Card haguriCard, Bafuda bafuda)
        {
            // 以下はBafudaクラス内で実行されるべき。
            // require
            //if (bafuda.Cards == null) throw new ArgumentNullException();
            //if (bafuda.Cards.Count <= 0) throw new Exception();

            Card getCard = bafuda.WhereFristToManth(haguriCard);

            // BafudaクラスのCardsからリムーブする。
            bafuda.RemoveCard(getCard);
            // Playerのkirifudaに一致する2枚を追加する。
            AddKirifuda(haguriCard);
            AddKirifuda(getCard);

        }
        /// <summary>
        /// haguriCardのManth属性が、BafudaクラスのCardsのManth属性と一致する場合（isMatchManth == trueの場合）、
        /// 一致する複数枚のCardから選ばれた一枚のCardと、haguriCardをPlayerクラスのkirifudaに加える。
        /// </summary>
        /// <param name="haguriCard"></param>
        /// <param name="bafuda"></param>
        /// <param name="getCard">Manth属性において、複数枚の一致するCardのから選ばれたCard</param>
        public void GetToBafuda(Card haguriCard, Bafuda bafuda, Card getCard)
        {
            // BafudaクラスのCardsからリムーブする。
            bafuda.RemoveCard(getCard);
            // Playerのkirifudaに一致する2枚を追加する。
            AddKirifuda(haguriCard);
            AddKirifuda(getCard);
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="playerCard"></param>
        /// <param name="index">場札に対するインデックス</param>
        public void ThrowTo(Bafuda bafuda, Card playerCard, int index)
        {
            // 例外処理はbafudaクラス内に閉じ込める。
            bafuda.RemoveCard(bafuda.Cards[index]);
            RemoveCard(cards[index]);

            // 場札に一致する札が存在するため一致する札を一枚kirifudaに加える。
            bafuda.ThrowToMatch(this, cards[index], index);
        }
        /// <summary>
        /// 選択肢は、異常な入力に対してExceptionを発生させる事で閉じている必要がある。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="card"></param>
        public void ThrowToAuto(Bafuda bafuda, Card playerCard)
        {
            if (bafuda.isMatchManth(playerCard))
            {
                int matchCount = bafuda.MatchManthCount(playerCard);
                if (1 > matchCount)
                {
                    List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                    ThrowTo(bafuda, playerCard, matchedCards.IndexOf(matchedCards[0]));
                }
                else if (1 == matchCount)
                {
                    // 例外処理はbafudaクラス内に閉じ込める。
                    bafuda.RemoveCard(bafuda.MatchedCard(playerCard));
                    RemoveCard(playerCard);

                    // 場札に一致する札が存在するため一致する札を一枚kirifudaに加える。
                    bafuda.ThrowToMatch(this, playerCard);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // 例外処理はbafudaクラス内に閉じ込める。
                bafuda.AddCard(playerCard);
                RemoveCard(playerCard);
            }
        }
        /// <summary>
        /// 選択肢は、異常な入力に対してExceptionを発生させる事で閉じている必要がある。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="card"></param>
        public void ThrowToManual(Bafuda bafuda, Card playerCard)
        {
            if (bafuda.isMatchManth(playerCard))
            {
                int matchCount = bafuda.MatchManthCount(playerCard);
                if (1 > matchCount)
                {
                    Console.Write("どの場札を切りますか？\n");
                    ConsoleKey consoleKey = Console.ReadKey(true).Key;
                    int index;
                    if (!Int32.TryParse(consoleKey.ToString(), out index)) throw new Exception();
                    Console.Write("{0} を切りました。", bafuda.Cards[index].CardPattern);

                    List<Card> matchedCards = bafuda.MatchedCards(playerCard);

                    // require
                    if (!matchedCards.Any(card => matchedCards.IndexOf(card) == index)) throw new Exception();

                    ThrowTo(bafuda, playerCard, index);
                }
                else if (1 == matchCount)
                {
                    // 例外処理はbafudaクラス内に閉じ込める。
                    bafuda.RemoveCard(bafuda.MatchedCard(playerCard));
                    RemoveCard(playerCard);

                    // 場札に一致する札が存在するため一致する札を一枚kirifudaに加える。
                    bafuda.ThrowToMatch(this, playerCard);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // 例外処理はbafudaクラス内に閉じ込める。
                bafuda.AddCard(playerCard);
                RemoveCard(playerCard);
            }
        }
    }
    class Card
    {
        public Card(Pattern pattern, int manth)
        {
            this.cardPattern = pattern;
            this.manth = manth;
        }
        private readonly Pattern cardPattern;
        public Pattern CardPattern { get { return cardPattern; } }
        private readonly int manth;
        public int Manth { get { return manth; } }
    }
    class Yamafuda
    {
        public Yamafuda()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Pattern.Matsu1, 1));
            cards.Add(new Card(Pattern.Matsu2, 1));
            cards.Add(new Card(Pattern.MatsuAndTanzakuAka, 1));
            cards.Add(new Card(Pattern.MatsuAndTsuru, 1));
            cards.Add(new Card(Pattern.Ume1, 2));
            cards.Add(new Card(Pattern.Ume2, 2));
            cards.Add(new Card(Pattern.UmeAndTanzakuAka, 2));
            cards.Add(new Card(Pattern.UmeAndUguisu, 2));
            cards.Add(new Card(Pattern.Sakura1, 3));
            cards.Add(new Card(Pattern.Sakura2, 3));
            cards.Add(new Card(Pattern.SakuraAndMaku, 3));
            cards.Add(new Card(Pattern.SakuraAndTanzakuAka, 3));
            cards.Add(new Card(Pattern.Fuji1, 4));
            cards.Add(new Card(Pattern.Fuji2, 4));
            cards.Add(new Card(Pattern.FujiAndHototogisu, 4));
            cards.Add(new Card(Pattern.FujiAndTanzakuAka, 4));
            cards.Add(new Card(Pattern.Ayame1, 5));
            cards.Add(new Card(Pattern.Ayame2, 5));
            cards.Add(new Card(Pattern.AyameAndHashi, 5));
            cards.Add(new Card(Pattern.AyameAndTanzakuAka, 5));
            cards.Add(new Card(Pattern.Botan1, 6));
            cards.Add(new Card(Pattern.Botan2, 6));
            cards.Add(new Card(Pattern.BotanAndChou, 6));
            cards.Add(new Card(Pattern.BotanAndTanzakuAo, 6));
            cards.Add(new Card(Pattern.Hagi1, 7));
            cards.Add(new Card(Pattern.Hagi2, 7));
            cards.Add(new Card(Pattern.HagiAndInoshishi, 7));
            cards.Add(new Card(Pattern.HagiAndTanzakuAka, 7));
            cards.Add(new Card(Pattern.Sakura1, 8));
            cards.Add(new Card(Pattern.Sakura2, 8));
            cards.Add(new Card(Pattern.SakuraAndMaku, 8));
            cards.Add(new Card(Pattern.SakuraAndTanzakuAka, 8));
            cards.Add(new Card(Pattern.Kiku1, 9));
            cards.Add(new Card(Pattern.Kiku2, 9));
            cards.Add(new Card(Pattern.KikuAndOchoko, 9));
            cards.Add(new Card(Pattern.KikuAndTanzakuAo, 9));
            cards.Add(new Card(Pattern.Momiji1, 10));
            cards.Add(new Card(Pattern.Momiji2, 10));
            cards.Add(new Card(Pattern.MomijiAndShika, 10));
            cards.Add(new Card(Pattern.MomijiAndTanzakuAo, 10));
            cards.Add(new Card(Pattern.Yanagi, 11));
            cards.Add(new Card(Pattern.YanagiAndKaeru, 11));
            cards.Add(new Card(Pattern.YanagiAndTanzakuAka, 11));
            cards.Add(new Card(Pattern.YanagiAndTsubame, 11));
            cards.Add(new Card(Pattern.Kiri1, 12));
            cards.Add(new Card(Pattern.Kiri2, 12));
            cards.Add(new Card(Pattern.KiriAndHouou, 12));
            cards.Add(new Card(Pattern.KiriAndYellow, 12));
            cards = cards.OrderBy(i => Guid.NewGuid()).ToList();
            foreach (Card card in cards)
            {
                cardStack.Push(card);
            }
        }
        private readonly Stack<Card> cardStack = new Stack<Card>();
        public Stack<Card> CardStack { get { return cardStack; } }
        public Card CardPop()
        {
            //requie
            if (cardStack == null) throw new ArgumentNullException();
            if (cardStack.Count <= 0) throw new Exception();

            return cardStack.Pop();
        }
    }
    class Bafuda : CardSet
    {
        public Bafuda(string name, Yamafuda yamafuda) : base(name, yamafuda)
        {
        }
        /// <summary>
        /// BafudaクラスのCardsのManth属性に対する一致を問い合わせる操作。
        /// </summary>
        /// <param name="card">BaufdaクラスのCardsに対し、問い合わせを行う対象。</param>
        /// <returns></returns>
        public bool isMatchManth(Card card)
        {
            // require
            if (cards == null) throw new ArgumentNullException();
            if (cards.Count <= 0) throw new Exception("cards.Count <= 0です。");

            return cards.Any(bafudaCard => bafudaCard.Manth == card.Manth);
        }
        /// <summary>
        /// BafudaクラスのCardsのManth属性に対する一致を問い合わせる操作。
        /// </summary>
        /// <param name="matchingCard">BaufdaクラスのCardsに対し、問い合わせを行う集合。</param>
        /// <returns></returns>
        public bool isMatchManth(List<Card> matchingCards)
        {
            bool isMatch = false;
            foreach (var manth in ManthList())
            {
                isMatch = matchingCards.Any(card => card.Manth == manth);
                if (isMatch) break;
            }
            return isMatch;
        }
        /// <summary>
        /// 引数にあるカード同士の一致を問い合わせる操作。
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <returns></returns>
        public bool isMatchManth(Card card1, Card card2)
        {
            return card1.Manth == card2.Manth;
        }
        /// <summary>
        /// BafudaクラスのCardsのManth属性に対する一致件数を問い合わせる操作。
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int MatchManthCount(Card card)
        {
            int matchCount = 0;
            foreach (var manth in ManthList())
            {
                if(card.Manth == manth)
                    matchCount++;
            }
            return matchCount;
        }
        /// <summary>
        /// BafudaクラスのCardsからManth属性だけを取り出して返す。
        /// </summary>
        /// <returns>int属性のリスト。</returns>
        public List<int> ManthList()
        {
            // require
            if (Cards == null) throw new ArgumentNullException();
            if (Cards.Count < 0) throw new ArgumentOutOfRangeException();

            List<int> manthList = new List<int>();
            foreach (var card in Cards)
            {
                manthList.Add(card.Manth);
            }
            return manthList;
        }
        /// <summary>
        /// 場札に対し、引数Cardで、isMatchManthがtrueかつ、MacthManthCountが1の場合の操作。
        /// </summary>
        /// <param name="card"></param>
        public void ThrowToMatch(Player player, Card playerCard)
        {
            // require
            if (!isMatchManth(playerCard)) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");
            if (!isMatchManth(playerCard, MatchedCard(playerCard))) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");

            player.AddKirifuda(playerCard);
            player.AddKirifuda(MatchedCard(playerCard));
        }
        /// <summary>
        /// 場札に対し、引数Cardで、isMatchManthがtrueのかつ、MacthManthCountが複数の場合の操作。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <param name="bafuda"></param>
        /// <param name="count">引数count番目の場札を切る。</param>
        public void ThrowToMatch(Player player, Card playerCard, int count)
        {
            // require
            if (!isMatchManth(playerCard, MatchedCard(playerCard))) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");
            if (!isMatchManth(playerCard, cards[count])) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");

            // cardとcount番目の場札の月が一致する。
            player.AddKirifuda(playerCard);
            player.AddKirifuda(cards[count]);
        }
        /// <summary>
        /// 一致するカードを返す。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public Card MatchedCard(Card playerCard)
        {
            return cards.Where(bafudaCard => bafudaCard.Manth == playerCard.Manth).Single();
        }
        /// <summary>
        /// 一致するカードを返す。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public List<Card> MatchedCards(Card playerCard)
        {
            return cards.Where(bafudaCard => bafudaCard.Manth == playerCard.Manth).ToList();
        }
    }
    class Manager
    {
        public Manager(int count)
        {
            playCount = count;
            Build(count);
        }
        private int playCount;
        private Yamafuda yamafuda = new Yamafuda();
        private List<Player> players = new List<Player>();
        private Bafuda bafuda;
        private void Build(int count)
        {
            bafuda = new Bafuda("東1棟1F席1", yamafuda);
            if (count == 2)
            {
                bafuda.TakeToYamafuda(8);
                players.Add(new Player("太郎", yamafuda));
                players.Add(new Player("和美", yamafuda));
                players[0].TakeToYamafuda(8);
                players[1].TakeToYamafuda(8);
                players = players.OrderBy(i => Guid.NewGuid()).ToList();
                players[0].PlayerState = State.Running;
                players[1].PlayerState = State.Waiting;
                players[0].PlayerHandle = Handle.Auto;
                players[1].PlayerHandle = Handle.Auto;
            }
            if (count == 3)
            {
                bafuda.TakeToYamafuda(6);
                players.Add(new Player("太郎", yamafuda));
                players.Add(new Player("和美", yamafuda));
                players.Add(new Player("紗子", yamafuda));
                players[0].TakeToYamafuda(7);
                players[1].TakeToYamafuda(7);
                players[2].TakeToYamafuda(7);
                players = players.OrderBy(i => Guid.NewGuid()).ToList();
                players[0].PlayerState = State.Running;
                players[1].PlayerState = State.Waiting;
                players[2].PlayerState = State.Waiting;
                players[0].PlayerHandle = Handle.Auto;
                players[1].PlayerHandle = Handle.Auto;
                players[2].PlayerHandle = Handle.Auto;
            }
        }
        public Player GetNextPlayer(Player currentPlayer)
        {
            int index = players.IndexOf(currentPlayer);
            if (index == players.Count - 1)
                return players[0];
            else
                return players[++index];
        }
        public bool isNextPlayer()
        {
            return players.Any(player => player.Cards.Count > 0);
        }
        public void Play()
        {
            Player currentPlayer = players.Single(player => player.PlayerState == State.Running);
            int current = players.IndexOf(currentPlayer);
            while (isNextPlayer())
            {
                if (currentPlayer.PlayerHandle == Handle.Auto)
                {
                    // 手札から場札に一枚出す。
                    currentPlayer.ThrowToAuto(bafuda, currentPlayer.Cards[0]);
                    
                    // 山札から場札に一枚出す。
                    Card popCard = yamafuda.CardPop();
                    currentPlayer.AddCard(popCard);
                    currentPlayer.ThrowToAuto(bafuda, popCard);

                    // 次の手番のプレイヤーを呼び出す。
                    currentPlayer.PlayerState = State.Waiting;
                    if (isNextPlayer())
                    {
                        currentPlayer = GetNextPlayer(currentPlayer);
                        currentPlayer.PlayerState = State.Running;
                    }
                }
                else if (currentPlayer.PlayerHandle == Handle.Manual)
                {
                    while (true)
                    {
                        ConsoleKey consoleKey = Console.ReadKey(true).Key;
                        switch (consoleKey)
                        {
                            case ConsoleKey.D0:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(0));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(0));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[0]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D1:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(1));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(1));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[1]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D2:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(2));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(2));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[2]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D3:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(3));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(3));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[3]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D4:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(4));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(4));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[4]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D5:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(5));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(5));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[5]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D6:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(6));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(6));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[6]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                            case ConsoleKey.D7:
                                Console.Write("{0} を切りますか？\n", currentPlayer.GetCardPattern(7));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(7));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[7]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);

                                    // 次の手番のプレイヤーを呼び出す。
                                    currentPlayer.PlayerState = State.Waiting;
                                    if (isNextPlayer())
                                    {
                                        currentPlayer = GetNextPlayer(currentPlayer);
                                        currentPlayer.PlayerState = State.Running;
                                    }
                                    break;
                                }
                                else if (consoleKey == ConsoleKey.N)
                                {
                                    break;
                                }
                                else
                                    throw new Exception("異常な入力が読み込まれました。\n");
                        }
                    }
                }
            }
        }
    }
    [Flags]
    enum State
    {
        Waiting = 0x1,
        Running = 0x2
    }
    [Flags]
    enum Handle
    {
        Manual = 0x1,
        Auto = 0x2
    }
}
