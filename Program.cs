using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roppyakken
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
        YanagiAndAme = 0x10000000000,
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
            Console.ReadKey(true);
        }
    }
    /// <summary>
    /// Cardメンバーの集合を現す抽象クラス。
    /// </summary>
    abstract class CardSet
    {
        public CardSet(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        /// <summary>
        /// Handメンバーはコンソールに手札を表示するための要素である。
        /// </summary>
        private Pattern hand;
        public Pattern Hand { get { return hand; } }
        /// <summary>
        /// CardsメンバーはPlayerクラスによる手札の実装である。
        /// </summary>
        private List<Card> cards = new List<Card>();
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
        public Player(string name) : base(name)
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
        /// <summary>
        /// Playerクラスの開始状態の構築。呼び出し回数はプレイ人数によって決定する。
        /// </summary>
        /// <param name="yamafuda"></param>
        public void TakeToYamafuda(Yamafuda yamafuda)
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
        public void TakeToYamafuda(Yamafuda yamafuda, int count)
        {
            // requie
            if (count <= 0) throw new ArgumentOutOfRangeException();
            if (yamafuda.CardStack == null) throw new ArgumentNullException();
            if (yamafuda.CardStack.Count == 0) throw new Exception("CardStackが空です。");

            while (count-- != 0)
                TakeToYamafuda(yamafuda);
        }
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
        /// <param name="getCard">Manth属性において、複数枚の一致するCardのから選ばれたCard</param>
        public void GetToBafuda(Card haguriCard, Card getCard)
        {
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
        public Bafuda(string name) : base(name)
        {
        }
        /// <summary>
        /// BafudaクラスのCardsのManth属性に対する一致を問い合わせる操作。
        /// </summary>
        /// <param name="matchingCard">BaufdaクラスのCardsに対し、問い合わせを行う対象。</param>
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
    }

}
