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
        protected Pattern handPattern;
        public Pattern HandPattern { get { return handPattern; } }
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
            handPattern |= card.CardPattern;
        }
        public void RemoveCard(Card card)
        {
            // require
            if (cards == null) throw new ArgumentNullException();
            if (cards.Count <= 0) throw new Exception("CardSet.Cardsが空です。");

            cards.Remove(card);
            // ビットを落とす。
            handPattern &= ~card.CardPattern;
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
        private List<Card> tokutenfuda = new List<Card>();
        private Pattern tokutenfudaPattern;
        public List<Card> Tokutenfuda { get { return tokutenfuda; } }
        public Pattern TokutenfudaPattern { get { return tokutenfudaPattern; } }
        public void AddTokutenfuda(Card card)
        {
            // require
            if (tokutenfuda == null) throw new ArgumentNullException();

            tokutenfuda.Add(card);
            tokutenfudaPattern |= card.CardPattern;
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
        /// 一致する1枚のCardとhaguriCardをPlayerクラスのtokutenfudaに加える。
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
            // Playerのtokutenfudaに一致する2枚を追加する。
            Addtokutenfuda(haguriCard);
            Addtokutenfuda(getCard);

        }
        /// <summary>
        /// haguriCardのManth属性が、BafudaクラスのCardsのManth属性と一致する場合（isMatchManth == trueの場合）、
        /// 一致する複数枚のCardから選ばれた一枚のCardと、haguriCardをPlayerクラスのtokutenfudaに加える。
        /// </summary>
        /// <param name="haguriCard"></param>
        /// <param name="bafuda"></param>
        /// <param name="getCard">Manth属性において、複数枚の一致するCardのから選ばれたCard</param>
        public void GetToBafuda(Card haguriCard, Bafuda bafuda, Card getCard)
        {
            // BafudaクラスのCardsからリムーブする。
            bafuda.RemoveCard(getCard);
            // Playerのtokutenfudaに一致する2枚を追加する。
            Addtokutenfuda(haguriCard);
            Addtokutenfuda(getCard);
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

            // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
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

                    // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
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
                    while (true)
                    {
                        Console.Write("どの場札を切りますか？ Enter or N\n");
                        ConsoleKey consoleKey = Console.ReadKey(true).Key;
                        int index;
                        if (!Int32.TryParse(consoleKey.ToString(), out index)) throw new Exception();
                        Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Cards[index].CardPattern);
                        consoleKey = Console.ReadKey(true).Key;
                        if (consoleKey == ConsoleKey.Enter)
                        {
                            Console.Write("{0} を切りました。\n", bafuda.Cards[index].CardPattern);
                            List<Card> matchedCards = bafuda.MatchedCards(playerCard);

                            // require
                            if (!matchedCards.Any(card => matchedCards.IndexOf(card) == index)) throw new Exception();

                            ThrowTo(bafuda, playerCard, index);
                            break;
                        }
                        if (consoleKey == ConsoleKey.N)
                        {
                            continue;
                        }
                        else
                            throw new Exception("異常な入力が読み込まれました。\n");
                    }
                }
                else if (1 == matchCount)
                {
                    // 例外処理はbafudaクラス内に閉じ込める。
                    bafuda.RemoveCard(bafuda.MatchedCard(playerCard));
                    RemoveCard(playerCard);

                    // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
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
        public Card(Pattern pattern, int manth, int point)
        {
            this.cardPattern = pattern;
            this.manth = manth;
            this.point = point;
        }
        private readonly Pattern cardPattern;
        public Pattern CardPattern { get { return cardPattern; } }
        private readonly int manth;
        public int Manth { get { return manth; } }
        private readonly int point;
        public int Point { get { return point; } }
    }
    class Yamafuda
    {
        public Yamafuda()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Pattern.Matsu1, 1, 0));
            cards.Add(new Card(Pattern.Matsu2, 1, 0));
            cards.Add(new Card(Pattern.MatsuAndTanzakuAka, 1, 10));
            cards.Add(new Card(Pattern.MatsuAndTsuru, 1, 50));
            cards.Add(new Card(Pattern.Ume1, 2, 0));
            cards.Add(new Card(Pattern.Ume2, 2, 0));
            cards.Add(new Card(Pattern.UmeAndTanzakuAka, 2, 10));
            cards.Add(new Card(Pattern.UmeAndUguisu, 2, 50));
            cards.Add(new Card(Pattern.Sakura1, 3, 0));
            cards.Add(new Card(Pattern.Sakura2, 3, 0));
            cards.Add(new Card(Pattern.SakuraAndMaku, 3, 50));
            cards.Add(new Card(Pattern.SakuraAndTanzakuAka, 3, 10));
            cards.Add(new Card(Pattern.Fuji1, 4, 0));
            cards.Add(new Card(Pattern.Fuji2, 4, 0));
            cards.Add(new Card(Pattern.FujiAndHototogisu, 4, 50));
            cards.Add(new Card(Pattern.FujiAndTanzakuAka, 4, 10));
            cards.Add(new Card(Pattern.Ayame1, 5, 0));
            cards.Add(new Card(Pattern.Ayame2, 5, 0));
            cards.Add(new Card(Pattern.AyameAndHashi, 5, 10));
            cards.Add(new Card(Pattern.AyameAndTanzakuAka, 5, 10));
            cards.Add(new Card(Pattern.Botan1, 6, 0));
            cards.Add(new Card(Pattern.Botan2, 6, 0));
            cards.Add(new Card(Pattern.BotanAndChou, 6, 10));
            cards.Add(new Card(Pattern.BotanAndTanzakuAo, 6, 10));
            cards.Add(new Card(Pattern.Hagi1, 7, 0));
            cards.Add(new Card(Pattern.Hagi2, 7, 0));
            cards.Add(new Card(Pattern.HagiAndInoshishi, 7, 10));
            cards.Add(new Card(Pattern.HagiAndTanzakuAka, 7, 10));
            cards.Add(new Card(Pattern.Susuki1, 8, 0));
            cards.Add(new Card(Pattern.Susuki2, 8, 0));
            cards.Add(new Card(Pattern.SusukiAndGan, 8, 10));
            cards.Add(new Card(Pattern.SusukiAndTsuki, 8, 50));
            cards.Add(new Card(Pattern.Kiku1, 9, 0));
            cards.Add(new Card(Pattern.Kiku2, 9, 0));
            cards.Add(new Card(Pattern.KikuAndOchoko, 9, 10));
            cards.Add(new Card(Pattern.KikuAndTanzakuAo, 9, 10));
            cards.Add(new Card(Pattern.Momiji1, 10, 0));
            cards.Add(new Card(Pattern.Momiji2, 10, 0));
            cards.Add(new Card(Pattern.MomijiAndShika, 10, 50));
            cards.Add(new Card(Pattern.MomijiAndTanzakuAo, 10, 10));
            cards.Add(new Card(Pattern.Yanagi, 11, 0));
            cards.Add(new Card(Pattern.YanagiAndKaeru, 11, 50));
            cards.Add(new Card(Pattern.YanagiAndTanzakuAka, 11, 10));
            cards.Add(new Card(Pattern.YanagiAndTsubame, 11, 10));
            cards.Add(new Card(Pattern.Kiri1, 12, 0));
            cards.Add(new Card(Pattern.Kiri2, 12, 0));
            cards.Add(new Card(Pattern.KiriAndHouou, 12, 50));
            cards.Add(new Card(Pattern.KiriAndYellow, 12, 10));
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

            player.AddTokutenfuda(playerCard);
            player.AddTokutenfuda(MatchedCard(playerCard));
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
            player.AddTokutenfuda(playerCard);
            player.AddTokutenfuda(cards[count]);
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
        public void Fill()
        {
            Console.Write("場札：{0}\n", bafuda.HandPattern);
            foreach (Player player in players)
            {
                Console.Write("{0}さんの手札（{1}）\n", player.Name, player.HandPattern);
                Console.Write("{0}さんの得点札（{1}）\n", player.Name, player.TokutenfudaPattern);
                Console.Write("{0}さんの得点（{1}）\n", player.Name, player.Score);
            }
        }
        public void CalculateScore(Player player)
        {
            int score = 0;
            foreach (Card card in player.Tokutenfuda)
            {
                score += card.Point;
            }
            Pattern tokutenfudaPattern = player.TokutenfudaPattern;
            if (tokutenfudaPattern.HasFlag(Pattern.Yanagi) && tokutenfudaPattern.HasFlag(Pattern.YanagiAndKaeru) && tokutenfudaPattern.HasFlag(Pattern.YanagiAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.YanagiAndTsubame))
                score += 200;
            if (tokutenfudaPattern.HasFlag(Pattern.Ayame1) && tokutenfudaPattern.HasFlag(Pattern.Ayame2) && tokutenfudaPattern.HasFlag(Pattern.AyameAndHashi) && tokutenfudaPattern.HasFlag(Pattern.AyameAndTanzakuAka))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Botan1) && tokutenfudaPattern.HasFlag(Pattern.Botan2) && tokutenfudaPattern.HasFlag(Pattern.BotanAndChou) && tokutenfudaPattern.HasFlag(Pattern.BotanAndTanzakuAo))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Fuji1) && tokutenfudaPattern.HasFlag(Pattern.Fuji2) && tokutenfudaPattern.HasFlag(Pattern.FujiAndHototogisu) && tokutenfudaPattern.HasFlag(Pattern.FujiAndTanzakuAka))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Hagi1) && tokutenfudaPattern.HasFlag(Pattern.Hagi2) && tokutenfudaPattern.HasFlag(Pattern.HagiAndInoshishi) && tokutenfudaPattern.HasFlag(Pattern.HagiAndTanzakuAka))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Kiku1) && tokutenfudaPattern.HasFlag(Pattern.Kiku2) && tokutenfudaPattern.HasFlag(Pattern.KikuAndOchoko) && tokutenfudaPattern.HasFlag(Pattern.KikuAndTanzakuAo))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Kiri1) && tokutenfudaPattern.HasFlag(Pattern.Kiri2) && tokutenfudaPattern.HasFlag(Pattern.KiriAndHouou) && tokutenfudaPattern.HasFlag(Pattern.KiriAndYellow))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Matsu1) && tokutenfudaPattern.HasFlag(Pattern.Matsu2) && tokutenfudaPattern.HasFlag(Pattern.MatsuAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.MatsuAndTsuru))
                score += 50; ;
            if (tokutenfudaPattern.HasFlag(Pattern.Momiji1) && tokutenfudaPattern.HasFlag(Pattern.Momiji2) && tokutenfudaPattern.HasFlag(Pattern.MomijiAndShika) && tokutenfudaPattern.HasFlag(Pattern.MomijiAndTanzakuAo))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Sakura1) && tokutenfudaPattern.HasFlag(Pattern.Sakura2) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndMaku) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndTanzakuAka))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Susuki1) && tokutenfudaPattern.HasFlag(Pattern.Susuki2) && tokutenfudaPattern.HasFlag(Pattern.SusukiAndGan) && tokutenfudaPattern.HasFlag(Pattern.SusukiAndTsuki))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.Ume1) && tokutenfudaPattern.HasFlag(Pattern.Ume2) && tokutenfudaPattern.HasFlag(Pattern.UmeAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.UmeAndUguisu))
                score += 50;
            if (tokutenfudaPattern.HasFlag(Pattern.FujiAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.AyameAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.HagiAndTanzakuAka))
                score += 100;
            if (tokutenfudaPattern.HasFlag(Pattern.BotanAndTanzakuAo) && tokutenfudaPattern.HasFlag(Pattern.KikuAndTanzakuAo) && tokutenfudaPattern.HasFlag(Pattern.MomijiAndTanzakuAo))
                score += 100;
            if (tokutenfudaPattern.HasFlag(Pattern.UmeAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.MatsuAndTanzakuAka) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndTanzakuAka))
                score += 150;
            if (tokutenfudaPattern.HasFlag(Pattern.UmeAndUguisu) && tokutenfudaPattern.HasFlag(Pattern.MatsuAndTsuru) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndMaku))
                score += 100;
            if (tokutenfudaPattern.HasFlag(Pattern.SusukiAndTsuki) && tokutenfudaPattern.HasFlag(Pattern.KikuAndOchoko))
                score += 100;
            if (tokutenfudaPattern.HasFlag(Pattern.SakuraAndMaku) && tokutenfudaPattern.HasFlag(Pattern.KikuAndOchoko))
                score += 100;
            if (tokutenfudaPattern.HasFlag(Pattern.SusukiAndTsuki) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndMaku) && tokutenfudaPattern.HasFlag(Pattern.KikuAndOchoko))
                score += 300;
            if (tokutenfudaPattern.HasFlag(Pattern.MatsuAndTsuru) && tokutenfudaPattern.HasFlag(Pattern.KiriAndHouou) && tokutenfudaPattern.HasFlag(Pattern.SusukiAndTsuki))
                score += 150;
            if (tokutenfudaPattern.HasFlag(Pattern.HagiAndInoshishi) && tokutenfudaPattern.HasFlag(Pattern.MomijiAndShika) && tokutenfudaPattern.HasFlag(Pattern.BotanAndChou))
                score += 300;
            if (tokutenfudaPattern.HasFlag(Pattern.MatsuAndTsuru) && tokutenfudaPattern.HasFlag(Pattern.KiriAndHouou) && tokutenfudaPattern.HasFlag(Pattern.SusukiAndTsuki) && tokutenfudaPattern.HasFlag(Pattern.SakuraAndMaku))
                score += 600;
            Pattern nanatan = Pattern.FujiAndTanzakuAka | Pattern.AyameAndTanzakuAka | Pattern.HagiAndTanzakuAka | Pattern.UmeAndTanzakuAka | Pattern.MatsuAndTanzakuAka | Pattern.SakuraAndTanzakuAka | Pattern.BotanAndTanzakuAo | Pattern.KikuAndTanzakuAo | Pattern.MomijiAndTanzakuAo;
            if (BitCnt1((long)(tokutenfudaPattern & nanatan)) >= 7)
                score += 600;

            player.Score = score;
        }
        int BitCnt1(long val)
        {
            int cnt = 0;
            while (val != 0)
            {
                if ((val & 1) != 0)
                    cnt++;
                val >>= 1;
            }
            return cnt
        }
        public void Play()
        {
            Player currentPlayer = players.Single(player => player.PlayerState == State.Running);

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
                    CalculateScore(currentPlayer);
                    Fill();
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
                        Fill();
                        Console.Write("どの札を出しますか？\n");
                        ConsoleKey consoleKey = Console.ReadKey(true).Key;
                        switch (consoleKey)
                        {
                            case ConsoleKey.D0:
                                if (currentPlayer.Cards.Count < 1) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(0));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(0));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[0]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 2) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(1));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(1));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[1]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 3) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(2));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(2));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[2]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 4) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(3));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(3));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[3]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 5) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(4));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(4));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[4]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 6) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(5));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(5));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[5]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 7) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(6));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(6));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[6]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
                                if (currentPlayer.Cards.Count < 8) break;
                                Console.Write("{0} を切りますか？ Enter or N\n", currentPlayer.GetCardPattern(7));
                                consoleKey = Console.ReadKey(true).Key;
                                if (consoleKey == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", currentPlayer.GetCardPattern(7));
                                    currentPlayer.ThrowToManual(bafuda, currentPlayer.Cards[7]);
                                    // 山札から場札に一枚出す。
                                    Card popCard = yamafuda.CardPop();
                                    currentPlayer.AddCard(popCard);
                                    currentPlayer.ThrowToManual(bafuda, popCard);
                                    CalculateScore(currentPlayer);
                                    Fill();
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
