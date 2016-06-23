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
        /// HandPatternメンバーはコンソールに手札を表示するための要素である。
        /// </summary>
        protected Pattern handPattern;
        public Pattern HandPattern { get { return handPattern; } }
        /// <summary>
        /// HandメンバーはPlayerクラスによる手札の実装である。
        /// </summary>
        protected List<Card> hand = new List<Card>();
        public List<Card> Hand { get { return hand; } }
        public void AddHand(Card card)
        {
            // require
            if (hand == null) throw new ArgumentNullException();

            this.hand.Add(card);
            // ビットを立てる。
            handPattern |= card.CardPattern;
        }
        public void RemoveHand(Card card)
        {
            // require
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count <= 0) throw new Exception("CardSet.Handが空です。");

            hand.Remove(card);
            // ビットを落とす。
            handPattern &= ~card.CardPattern;
        }
        public Pattern GetHandPattern(int index)
        {
            return hand[index].CardPattern;
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
            AddHand(card);
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
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count <= 0) throw new Exception("CardSet.Handが空です。");

            return hand.Where(firstCard => firstCard.Manth == card.Manth).First();
        }
    }
    /// <summary>
    /// PlayerクラスのHandメンバーの開始状態はゲームのプレイ人数に影響されるため、初期化は遅延する。
    /// </summary>
    class Player : CardSet
    {
        public Player(string name, Yamafuda yamafuda, Handle playerHandle, int count, Manager manager) : base(name, yamafuda)
        {
            this.playerHandle = playerHandle;
            Manager = manager;
            if (count == 2) TakeToYamafuda(8);
            else if (count == 3) TakeToYamafuda(7);
            else
            {
                throw new Exception();
            }
            if (playerHandle == Handle.AI1)
            {
                MakeChoiceDictionary();
            }
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
        private Handle playerHandle;
        public Handle PlayerHandle { get { return playerHandle; } }
        public PlayerFlags PlayerFlag { get; set; }
        private Dictionary<Pattern, int> choiceDictionary = new Dictionary<Pattern, int>();
        public Dictionary<Pattern, int> ChoiceDictionary { get { return choiceDictionary; } }
        public Manager Manager { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="card"></param>
        public void ThrowToAuto(Bafuda bafuda, Card playerCard)
        {
            if (bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (3 == matchCount)
                    {
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCardsOnYanagiAndKaeru(playerCard)[0]));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCards(playerCard)[0]));
                        }
                    }
                    else if (1 == matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCardsOnYanagiAndKaeru(playerCard)[0]));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCards(playerCard)[0]));
                        }
                    }
                    else if (0 == matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCardsOnYanagiAndKaeru(playerCard);

                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru && matchedCards.Count > 0)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(matchedCards[0]));
                        }
                        else
                        {
                            bafuda.AddHand(playerCard);
                            RemoveHand(playerCard);
                        }
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (3 == matchCount)
                    {
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                        bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(matchedCards[0]));
                    }
                    else if (1 == matchCount)
                    {
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
            else if (!bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (1 < matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCardsOnYanagiAndKaeru(playerCard)[0]));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCards(playerCard)[0]));
                        }
                    }
                    else if (1 == matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCardsOnYanagiAndKaeru(playerCard)[0]));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(bafuda.MatchedCards(playerCard)[0]));
                        }
                    }
                    else if (0 == matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCardsOnYanagiAndKaeru(playerCard);

                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru && matchedCards.Count > 0)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(matchedCards[0]));
                        }
                        else
                        {
                            bafuda.AddHand(playerCard);
                            RemoveHand(playerCard);
                        }
                        
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (1 < matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                        bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(matchedCards[0]));
                    }
                    else if (1 == matchCount)
                    {
                        // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        // 例外処理はbafudaクラス内に閉じ込める。
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
        }
        /// <summary>
        /// 出す札の選択。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <returns></returns>
        public Card ChoiceThrowAI1(Bafuda bafuda)
        {
            Dictionary<Pattern, int> choiceDict = new Dictionary<Pattern, int>();
            choiceDict = ChoiceDictionaryOrderByDescending();

            // 役が揃っているものを候補から外す。
            foreach (KeyValuePair<Pattern, int> kvp in choiceDict)
            {
                if (TokutenfudaPattern.HasFlag(kvp.Key))
                {
                    choiceDict[kvp.Key] = 0;
                }
            }
            Pattern choicePattern = choiceDict.Keys.First();

            // 役に必要な札の集合
            Pattern requiredCards = choicePattern & ~TokutenfudaPattern;
            foreach (Card card in Hand)
            {
                if (requiredCards.HasFlag(card.CardPattern))
                {
                    return card;
                }
            }
            // 月が一致する札の内、点が高い札から切る。
            List<Card> choice = new List<Card>();
            foreach (Card card in Hand)
            {
                choice.Union(bafuda.MatchedCards(card));
            }
            choice.Distinct();
            choice = choice.OrderByDescending(card => card.Point).ToList();
            if (0 < choice.Count)
            {
                return choice[0];
            }
            // インデックス0の札から切る。
            return Hand[0];
        }
        /// <summary>
        /// 月が一致する札が存在する場合に呼び出し、切る札を選択する。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public Card ChoiceTakeAI1(Bafuda bafuda, Card playerCard)
        {
            Dictionary<Pattern, int> choiceDict = new Dictionary<Pattern, int>();
            choiceDict = ChoiceDictionaryOrderByDescending();

            // 役が揃っているものを候補から外す。
            foreach (KeyValuePair<Pattern, int> kvp in choiceDict)
            {
                if (TokutenfudaPattern.HasFlag(kvp.Key))
                {
                    choiceDict.Remove(kvp.Key);
                }
            }
            Pattern choicePattern = choiceDict.Keys.First();

            // 役に必要な札の集合
            Pattern requiredCards = choicePattern & ~TokutenfudaPattern;
            foreach (Card card in bafuda.MatchedCards(playerCard))
            {
                if (requiredCards.HasFlag(card.CardPattern))
                {
                    return card;
                }
            }
            // 月が一致する札の内、点が高い札から切る。
            List<Card> matchedCards = bafuda.MatchedCards(playerCard);
            matchedCards = matchedCards.OrderByDescending(card => card.Point).ToList();
            return matchedCards.First();
        }
        /// <summary>
        /// 柳に蛙札で切る場合の選択。候補が存在しない場合nullを返す。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public Card ChoiceTakeAI1OnYanagiAndKaeru(Bafuda bafuda, Card playerCard)
        {
            Dictionary<Pattern, int> choiceDict = new Dictionary<Pattern, int>();
            choiceDict = ChoiceDictionaryOrderByDescending();

            // 役が揃っているものを候補から外す。
            foreach (KeyValuePair<Pattern, int> kvp in choiceDict)
            {
                if (TokutenfudaPattern.HasFlag(kvp.Key))
                {
                    choiceDict.Remove(kvp.Key);
                }
            }
            Pattern choicePattern = choiceDict.Keys.First();

            // 役に必要な札の集合
            Pattern requiredCards = choicePattern & ~TokutenfudaPattern;
            foreach (Card card in bafuda.MatchedCardsOnYanagiAndKaeru(playerCard))
            {
                if (requiredCards.HasFlag(card.CardPattern))
                {
                    return card;
                }
            }
            // 月が一致する札の内、点が高い札から切る。
            List<Card> matchedCards = bafuda.MatchedCardsOnYanagiAndKaeru(playerCard);
            matchedCards = matchedCards.OrderByDescending(card => card.Point).ToList();
            if (0 < matchedCards.Count)
            {
                return matchedCards.First();
            }
            matchedCards = bafuda.MatchedCards(playerCard);
            if (0 < matchedCards.Count)
            {
                return matchedCards.First();
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="card"></param>
        public void ThrowToAI1(Bafuda bafuda, Card playerCard)
        {
            if (bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (3 == matchCount)
                    {
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard)));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                        }
                    }
                    else if (1 == matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard)));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                        }
                    }
                    else if (0 == matchCount)
                    {
                        Card choiceCard = ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard);

                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru && choiceCard != null)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(choiceCard));
                        }
                        else
                        {
                            bafuda.AddHand(playerCard);
                            RemoveHand(playerCard);
                        }
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (3 == matchCount)
                    {
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                        bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                    }
                    else if (1 == matchCount)
                    {
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
            else if (!bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (1 < matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard)));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                        }
                    }
                    else if (1 == matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard)));
                        }
                        else
                        {
                            bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                        }
                    }
                    else if (0 == matchCount)
                    {
                        Card choiceCard = ChoiceTakeAI1OnYanagiAndKaeru(bafuda, playerCard);

                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru && choiceCard != null)
                        {
                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, bafuda.Hand.IndexOf(choiceCard));
                        }
                        else
                        {
                            bafuda.AddHand(playerCard);
                            RemoveHand(playerCard);
                        }
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);
                    if (1 < matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                        bafuda.TakeToMatch(this, playerCard, bafuda.Hand.IndexOf(ChoiceTakeAI1(bafuda, playerCard)));
                    }
                    else if (1 == matchCount)
                    {
                        // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        // 例外処理はbafudaクラス内に閉じ込める。
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
        }
        /// <summary>
        /// 選択肢は、異常な入力に対してExceptionを発生させる事で閉じている必要がある。
        /// </summary>
        /// <param name="bafuda"></param>
        /// <param name="card"></param>
        public void ThrowToManual(Bafuda bafuda, Card playerCard)
        {
            if (bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);

                    if (3 == matchCount)
                    {
                        List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                        foreach (Card card in matchedCards)
                        {
                            Console.Write("{0}, ", card.CardPattern);
                        }
                        Console.Write("を切りました。\n");
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;
                            
                            if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCards(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatch(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    else if (1 == matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;

                            if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                            {
                                Card bafudaMatchedCard = bafuda.MatchedCard(playerCard);
                                Console.Write("{0} を切りました。\n", bafudaMatchedCard.CardPattern);
                                // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                                bafuda.TakeToMatch(this, playerCard);
                            }
                        }
                    }
                    else if (0 == matchCount)
                    {
                        if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                        {
                            while (true)
                            {
                                Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                                int index;
                                if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;
                                if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                    {
                                        Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                        consoleKeyInfo = Console.ReadKey(true);
                                        if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                        {
                                            Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                            bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                            break;
                                        }
                                        if (consoleKeyInfo.Key == ConsoleKey.N)
                                        {
                                            continue;
                                        }
                                        else
                                            continue;
                                    }
                                    else
                                        continue;
                                }
                                else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                                {
                                    Console.Write("{0} を場に出しました。\n", playerCard.CardPattern);

                                    // 例外処理はbafudaクラス内に閉じ込める。
                                    bafuda.AddHand(playerCard);
                                    RemoveHand(playerCard);
                                }
                            }
                        }
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);

                    if (3 == matchCount)
                    {
                        bafuda.TakeToMatcbOnThreePiecesOfTheSameKinde(this, playerCard);
                    }
                    else if (1 < matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;
                            List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                            if (matchedCards.Any(card => bafuda.Hand.IndexOf(card) == index))
                            {
                                Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                consoleKeyInfo = Console.ReadKey(true);
                                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                    bafuda.TakeToMatch(this, playerCard, index);
                                    break;
                                }
                                if (consoleKeyInfo.Key == ConsoleKey.N)
                                {
                                    continue;
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                    }
                    else if (1 == matchCount)
                    {
                        Card bafudaMatchedCard = bafuda.MatchedCard(playerCard);
                        Console.Write("{0} を切りました。\n", bafudaMatchedCard.CardPattern);
                        // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        Console.Write("{0} を場に出しました。\n", playerCard.CardPattern);

                        // 例外処理はbafudaクラス内に閉じ込める。
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
            else if (!bafuda.BafudaFlag.HasFlag(BafudaFlags.ThreePiecesOfTheSameKind))
            {
                if (PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);

                    if (1 < matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;

                            if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCards(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatch(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    else if (1 == matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;

                            if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                            {
                                Card bafudaMatchedCard = bafuda.MatchedCard(playerCard);
                                Console.Write("{0} を切りました。\n", bafudaMatchedCard.CardPattern);
                                // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                                bafuda.TakeToMatch(this, playerCard);
                            }
                        }
                    }
                    else if (0 == matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;

                            if (playerCard.CardPattern == Pattern.YanagiAndKaeru)
                            {
                                if (bafuda.MatchedCardsOnYanagiAndKaeru(playerCard).Any(card => bafuda.Hand.IndexOf(card) == index))
                                {
                                    Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                    consoleKeyInfo = Console.ReadKey(true);
                                    if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                    {
                                        Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                        bafuda.TakeToMatchOnYanagiAndKaeru(this, playerCard, index);
                                        break;
                                    }
                                    if (consoleKeyInfo.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else if (playerCard.CardPattern != Pattern.YanagiAndKaeru)
                            {
                                Console.Write("{0} を場に出しました。\n", playerCard.CardPattern);

                                // 例外処理はbafudaクラス内に閉じ込める。
                                bafuda.AddHand(playerCard);
                                RemoveHand(playerCard);
                            }
                        }
                    }
                }
                else if (!PlayerFlag.HasFlag(PlayerFlags.HandInYanagiAndKaeru))
                {
                    int matchCount = bafuda.MatchManthCount(playerCard);

                    if (1 < matchCount)
                    {
                        while (true)
                        {
                            Console.Write("どの場札を切りますか？　切りたい札のインデックスを入力して下さい。\n");
                            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                            int index;
                            if (!Int32.TryParse(consoleKeyInfo.KeyChar.ToString(), out index)) continue;
                            List<Card> matchedCards = bafuda.MatchedCards(playerCard);
                            if (matchedCards.Any(card => bafuda.Hand.IndexOf(card) == index))
                            {
                                Console.Write("{0} を切りますか？ Enter or N\n", bafuda.Hand[index].CardPattern);
                                consoleKeyInfo = Console.ReadKey(true);
                                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                                {
                                    Console.Write("{0} を切りました。\n", bafuda.Hand[index].CardPattern);
                                    bafuda.TakeToMatch(this, playerCard, index);
                                    break;
                                }
                                if (consoleKeyInfo.Key == ConsoleKey.N)
                                {
                                    continue;
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                    }
                    else if (1 == matchCount)
                    {
                        Card bafudaMatchedCard = bafuda.MatchedCard(playerCard);
                        Console.Write("{0} を切りました。\n", bafudaMatchedCard.CardPattern);
                        // 場札に一致する札が存在するため一致する札を一枚tokutenfudaに加える。
                        bafuda.TakeToMatch(this, playerCard);
                    }
                    else if (0 == matchCount)
                    {
                        Console.Write("{0} を場に出しました。\n", playerCard.CardPattern);

                        // 例外処理はbafudaクラス内に閉じ込める。
                        bafuda.AddHand(playerCard);
                        RemoveHand(playerCard);
                    }
                }
            }
        }
        public void UpdateChoiceDictionary(Pattern pattern, int count)
        {
            if (choiceDictionary == null) throw new Exception();
            if (choiceDictionary.Count < 0) throw new Exception();

            choiceDictionary[pattern] = count;
        }
        public void CheckFlags()
        {
            foreach (Card card in hand)
            {
                if (card.CardPattern == Pattern.YanagiAndKaeru) PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
            }
        }
        /// <summary>
        /// ビットカウント
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int BitCnt1(long val)
        {
            int cnt = 0;
            while (val != 0)
            {
                if ((val & 1) != 0)
                    cnt++;
                val >>= 1;
            }
            return cnt;
        }
        public Dictionary<Pattern, int> ChoiceDictionaryOrderByDescending()
        {
            return choiceDictionary.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public void MakeChoiceDictionary()
        {
            choiceDictionary.Add(Manager.Yanagi4, BitCnt1((long)(TokutenfudaPattern & Manager.Yanagi4)));
            choiceDictionary.Add(Manager.Ayame4, BitCnt1((long)(TokutenfudaPattern & Manager.Ayame4)));
            choiceDictionary.Add(Manager.Botan4, BitCnt1((long)(TokutenfudaPattern & Manager.Botan4)));
            choiceDictionary.Add(Manager.Fuji4, BitCnt1((long)(TokutenfudaPattern & Manager.Fuji4)));
            choiceDictionary.Add(Manager.Hagi4, BitCnt1((long)(TokutenfudaPattern & Manager.Hagi4)));
            choiceDictionary.Add(Manager.Kiku4, BitCnt1((long)(TokutenfudaPattern & Manager.Kiku4)));
            choiceDictionary.Add(Manager.Kiri4, BitCnt1((long)(TokutenfudaPattern & Manager.Kiri4)));
            choiceDictionary.Add(Manager.Matsu4, BitCnt1((long)(TokutenfudaPattern & Manager.Matsu4)));
            choiceDictionary.Add(Manager.Momiji4, BitCnt1((long)(TokutenfudaPattern & Manager.Momiji4)));
            choiceDictionary.Add(Manager.Sakura4, BitCnt1((long)(TokutenfudaPattern & Manager.Sakura4)));
            choiceDictionary.Add(Manager.Susuki4, BitCnt1((long)(TokutenfudaPattern & Manager.Susuki4)));
            choiceDictionary.Add(Manager.Ume4, BitCnt1((long)(TokutenfudaPattern & Manager.Ume4)));
            choiceDictionary.Add(Manager.Akatan, BitCnt1((long)(TokutenfudaPattern & Manager.Akatan)));
            choiceDictionary.Add(Manager.Aotan, BitCnt1((long)(TokutenfudaPattern & Manager.Aotan)));
            choiceDictionary.Add(Manager.Kozan, BitCnt1((long)(TokutenfudaPattern & Manager.Kozan)));
            choiceDictionary.Add(Manager.Oozan, BitCnt1((long)(TokutenfudaPattern & Manager.Oozan)));
            choiceDictionary.Add(Manager.TsukimideIppai, BitCnt1((long)(TokutenfudaPattern & Manager.TsukimideIppai)));
            choiceDictionary.Add(Manager.HanamideIppai, BitCnt1((long)(TokutenfudaPattern & Manager.HanamideIppai)));
            choiceDictionary.Add(Manager.Teppou, BitCnt1((long)(TokutenfudaPattern & Manager.Teppou)));
            choiceDictionary.Add(Manager.MatsuKiriBouzu, BitCnt1((long)(TokutenfudaPattern & Manager.MatsuKiriBouzu)));
            choiceDictionary.Add(Manager.InoShikaChou, BitCnt1((long)(TokutenfudaPattern & Manager.InoShikaChou)));
            choiceDictionary.Add(Manager.Shikou, BitCnt1((long)(TokutenfudaPattern & Manager.Shikou)));
            choiceDictionary.Add(Manager.Nanatan, BitCnt1((long)(TokutenfudaPattern & Manager.Nanatan)));
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
        public BafudaFlags BafudaFlag { get; set; }
        /// <summary>
        /// BafudaクラスのCardsのManth属性に対する一致を問い合わせる操作。
        /// </summary>
        /// <param name="card">BaufdaクラスのCardsに対し、問い合わせを行う対象。</param>
        /// <returns></returns>
        public bool isMatchManth(Card card)
        {
            // require
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count <= 0) throw new Exception("hand.Count <= 0です。");

            return hand.Any(bafudaCard => bafudaCard.Manth == card.Manth);
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
                if (card.Manth == manth)
                {
                    matchCount++;
                }
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
            if (Hand == null) throw new ArgumentNullException();
            if (Hand.Count < 0) throw new ArgumentOutOfRangeException();

            List<int> manthList = new List<int>();
            foreach (var card in Hand)
            {
                manthList.Add(card.Manth);
            }
            return manthList;
        }
        /// <summary>
        /// 場札に対し、引数Cardで、isMatchManthがtrueかつ、MacthManthCountが1の場合の操作。
        /// </summary>
        /// <param name="card"></param>
        public void TakeToMatch(Player player, Card playerCard)
        {
            // require
            if (!isMatchManth(playerCard)) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");
            Card matchedCard = MatchedCard(playerCard);
            if (!isMatchManth(playerCard, matchedCard)) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");

            player.AddTokutenfuda(playerCard);
            player.AddTokutenfuda(matchedCard);

            player.RemoveHand(playerCard);
            RemoveHand(matchedCard);
        }
        /// <summary>
        /// 場札に対し、引数Cardで、isMatchManthがtrueのかつ、MacthManthCountが複数の場合の操作。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <param name="bafuda"></param>
        /// <param name="index">引数index番目の場札を切る。</param>
        public void TakeToMatch(Player player, Card playerCard, int index)
        {
            // require
            if (!isMatchManth(playerCard, hand[index])) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");

            // cardとcount番目の場札の月が一致する。
            player.AddTokutenfuda(playerCard);
            player.AddTokutenfuda(hand[index]);

            player.RemoveHand(playerCard);
            RemoveHand(hand[index]);
        }
        /// <summary>
        /// 同種三枚の札を切る。
        /// </summary>
        /// <param name="player"></param>
        /// <param name="playerCard"></param>
        public void TakeToMatcbOnThreePiecesOfTheSameKinde(Player player, Card playerCard)
        {
            // require
            if (!isMatchManth(playerCard)) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");
            List<Card> matchedCards = MatchedCards(playerCard);

            foreach (Card matchedCard in matchedCards)
            {
                if (!isMatchManth(playerCard, matchedCard)) throw new Exception("異常な組み合わせです。select card was not match manth bafuda");
            }
            player.AddTokutenfuda(playerCard);
            player.RemoveHand(playerCard);
            foreach(Card matchedCard in matchedCards)
            {
                player.AddTokutenfuda(matchedCard);
                RemoveHand(matchedCard);
            }
            // フラグを落とす。
            BafudaFlag &= ~BafudaFlags.ThreePiecesOfTheSameKind;
        }
        /// <summary>
        /// playerCardが柳に蛙札の場合の処理。
        /// </summary>
        /// <param name="player"></param>
        /// <param name="playerCard"></param>
        /// <param name="index"></param>
        public void TakeToMatchOnYanagiAndKaeru(Player player, Card playerCard, int index)
        {
            if (hand[index].Point != 0)
            {
                player.AddTokutenfuda(playerCard);
                player.AddTokutenfuda(hand[index]);

                RemoveHand(hand[index]);
                player.RemoveHand(playerCard);
            }
            else if (isMatchManth(playerCard, hand[index]))
            {
                player.AddTokutenfuda(playerCard);
                player.AddTokutenfuda(hand[index]);

                RemoveHand(hand[index]);
                player.RemoveHand(playerCard);
            }
            else
            {
                AddHand(playerCard);
                player.RemoveHand(playerCard);
            }
            // フラグを落とす。
            player.PlayerFlag &= ~PlayerFlags.HandInYanagiAndKaeru;
        }
        /// <summary>
        /// 一致するカードを返す。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public Card MatchedCard(Card playerCard)
        {
            // require
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count < 1) throw new Exception();

            return hand.Where(bafudaCard => bafudaCard.Manth == playerCard.Manth).Single();
        }
        /// <summary>
        /// 一致するカードを返す。
        /// </summary>
        /// <param name="playerCard"></param>
        /// <returns></returns>
        public List<Card> MatchedCards(Card playerCard)
        {
            // require
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count < 1) throw new Exception();

            return hand.Where(bafudaCard => bafudaCard.Manth == playerCard.Manth).ToList();
        }
        /// <summary>
        /// 柳に蛙札と一致するカードを返す。
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public List<Card> MatchedCardsOnYanagiAndKaeru(Card playerCard)
        {
            // require
            if (hand == null) throw new ArgumentNullException();
            if (hand.Count < 1) throw new Exception();

            List<Card> matchedCards = new List<Card>();
            foreach (Card matchingCard in hand)
            {
                if (matchingCard.Point != 0)
                {
                    matchedCards.Add(matchingCard);
                }
            }
            if (0 == matchedCards.Count)
            {
                return MatchedCards(playerCard);
            }
            return matchedCards;
        } 
        public void CheckFlags()
        {
            foreach(Card card in hand)
            {
                IEnumerable<Card> cards = hand.Where(card2 => card2.Manth == card.Manth);
                if (cards.Count() == 3)
                {
                    BafudaFlag |= BafudaFlags.ThreePiecesOfTheSameKind;
                }
            }
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
        public Pattern Yanagi4 = Pattern.Yanagi | Pattern.YanagiAndKaeru | Pattern.YanagiAndTanzakuAka | Pattern.YanagiAndTsubame;
        public Pattern Ayame4 = Pattern.Ayame1 | Pattern.Ayame2 | Pattern.AyameAndHashi | Pattern.AyameAndTanzakuAka;
        public Pattern Botan4 = Pattern.Botan1 | Pattern.Botan2 | Pattern.BotanAndChou | Pattern.BotanAndTanzakuAo;
        public Pattern Fuji4 = Pattern.Fuji1 | Pattern.Fuji2 | Pattern.FujiAndHototogisu | Pattern.FujiAndTanzakuAka;
        public Pattern Hagi4 = Pattern.Hagi1 | Pattern.Hagi2 | Pattern.HagiAndInoshishi | Pattern.HagiAndTanzakuAka;
        public Pattern Kiku4 = Pattern.Kiku1 | Pattern.Kiku2 | Pattern.KikuAndOchoko | Pattern.KikuAndTanzakuAo;
        public Pattern Kiri4 = Pattern.Kiri1 | Pattern.Kiri2 | Pattern.KiriAndHouou | Pattern.KiriAndYellow;
        public Pattern Matsu4 = Pattern.Matsu1 | Pattern.Matsu2 | Pattern.MatsuAndTanzakuAka | Pattern.MatsuAndTsuru;
        public Pattern Momiji4 = Pattern.Momiji1 | Pattern.Momiji2 | Pattern.MomijiAndShika | Pattern.MomijiAndTanzakuAo;
        public Pattern Sakura4 = Pattern.Sakura1 | Pattern.Sakura2 | Pattern.SakuraAndMaku | Pattern.SakuraAndTanzakuAka;
        public Pattern Susuki4 = Pattern.Susuki1 | Pattern.Susuki2 | Pattern.SusukiAndGan | Pattern.SusukiAndTsuki;
        public Pattern Ume4 = Pattern.Ume1 | Pattern.Ume2 | Pattern.UmeAndTanzakuAka | Pattern.UmeAndUguisu;
        public Pattern Akatan = Pattern.FujiAndTanzakuAka | Pattern.AyameAndTanzakuAka | Pattern.HagiAndTanzakuAka;
        public Pattern Aotan = Pattern.BotanAndTanzakuAo | Pattern.KikuAndTanzakuAo | Pattern.MomijiAndTanzakuAo;
        public Pattern Kozan = Pattern.UmeAndTanzakuAka | Pattern.MatsuAndTanzakuAka | Pattern.SakuraAndTanzakuAka;
        public Pattern Oozan = Pattern.UmeAndUguisu | Pattern.MatsuAndTsuru | Pattern.SakuraAndMaku;
        public Pattern TsukimideIppai = Pattern.SusukiAndTsuki | Pattern.KikuAndOchoko;
        public Pattern HanamideIppai = Pattern.SakuraAndMaku | Pattern.KikuAndOchoko;
        public Pattern Teppou = Pattern.SusukiAndTsuki | Pattern.SakuraAndMaku | Pattern.KikuAndOchoko;
        public Pattern MatsuKiriBouzu = Pattern.MatsuAndTsuru | Pattern.KiriAndHouou | Pattern.SusukiAndTsuki;
        public Pattern InoShikaChou = Pattern.HagiAndInoshishi | Pattern.MomijiAndShika | Pattern.BotanAndChou;
        public Pattern Shikou = Pattern.MatsuAndTsuru | Pattern.KiriAndHouou | Pattern.SusukiAndTsuki | Pattern.SakuraAndMaku;
        public Pattern Nanatan = Pattern.FujiAndTanzakuAka | Pattern.AyameAndTanzakuAka | Pattern.HagiAndTanzakuAka | Pattern.UmeAndTanzakuAka | Pattern.MatsuAndTanzakuAka | Pattern.SakuraAndTanzakuAka | Pattern.BotanAndTanzakuAo | Pattern.KikuAndTanzakuAo | Pattern.MomijiAndTanzakuAo;
        private void Build(int count)
        {
            bafuda = new Bafuda("東1棟1F席1", yamafuda);
            if (count == 2)
            {
                bafuda.TakeToYamafuda(8);
                bafuda.CheckFlags();
                players.Add(new Player("太郎", yamafuda, Handle.Manual, count, this));
                players.Add(new Player("和美", yamafuda, Handle.AI1, count, this));
                players = players.OrderBy(i => Guid.NewGuid()).ToList();
                players[0].PlayerState = State.Running;
                players[1].PlayerState = State.Waiting;
                Play();
            }
            if (count == 3)
            {
                bafuda.TakeToYamafuda(6);
                bafuda.CheckFlags();
                players.Add(new Player("太郎", yamafuda, Handle.AI1, count, this));
                players.Add(new Player("和美", yamafuda, Handle.AI1, count, this));
                players.Add(new Player("紗子", yamafuda, Handle.AI1, count, this));
                players = players.OrderBy(i => Guid.NewGuid()).ToList();
                players[0].PlayerState = State.Running;
                players[1].PlayerState = State.Waiting;
                players[2].PlayerState = State.Waiting;
                Play();
            }
        }
        public bool isNextPlayer()
        {
            return players.Any(player => player.Hand.Count > 0);
        }
        public void Fill()
        {
            Console.Write("\n場札：（");
            foreach (Card card in bafuda.Hand)
            {
                Console.Write("{0}({1}), ", card.CardPattern, bafuda.Hand.IndexOf(card));
            }
            Console.Write("）\n");
            foreach (Player player in players)
            {
                Console.Write("{0}さんの手札（", player.Name);
                foreach(Card card in player.Hand)
                {
                    Console.Write("{0}({1}), ", card.CardPattern, player.Hand.IndexOf(card));
                }
                Console.Write("）\n");

                Console.Write("{0}さんの得点札（", player.Name);
                foreach(Card card in player.Tokutenfuda)
                {
                    Console.Write("{0}, ", card.CardPattern);
                }
                Console.Write("）\n");

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
            if (tokutenfudaPattern.HasFlag(Yanagi4)) score += 200;
            if (tokutenfudaPattern.HasFlag(Ayame4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Botan4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Fuji4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Hagi4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Kiku4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Kiri4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Matsu4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Momiji4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Sakura4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Susuki4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Ume4)) score += 50;
            if (tokutenfudaPattern.HasFlag(Akatan)) score += 100;
            if (tokutenfudaPattern.HasFlag(Aotan)) score += 100;
            if (tokutenfudaPattern.HasFlag(Kozan)) score += 150;
            if (tokutenfudaPattern.HasFlag(Oozan)) score += 100;
            if (tokutenfudaPattern.HasFlag(TsukimideIppai)) score += 100;
            if (tokutenfudaPattern.HasFlag(HanamideIppai)) score += 100;
            if (tokutenfudaPattern.HasFlag(Teppou)) score += 300;
            if (tokutenfudaPattern.HasFlag(MatsuKiriBouzu)) score += 150;
            if (tokutenfudaPattern.HasFlag(InoShikaChou)) score += 300;
            if (tokutenfudaPattern.HasFlag(Shikou)) score += 600;
            if (BitCnt1((long)(tokutenfudaPattern & Nanatan)) >= 7) score += 600;
            player.Score = score;
        }
        /// <summary>
        /// ビットカウント
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int BitCnt1(long val)
        {
            int cnt = 0;
            while (val != 0)
            {
                if ((val & 1) != 0)
                    cnt++;
                val >>= 1;
            }
            return cnt;
        }
        /// <summary>
        /// choiceDictionaryの更新と札の選択。
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <returns></returns>
        public Card ChoiceThrowAI1(Player currentPlayer)
        {
            currentPlayer.UpdateChoiceDictionary(Yanagi4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Yanagi4)));
            currentPlayer.UpdateChoiceDictionary(Ayame4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Ayame4)));
            currentPlayer.UpdateChoiceDictionary(Botan4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Botan4)));
            currentPlayer.UpdateChoiceDictionary(Fuji4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Fuji4)));
            currentPlayer.UpdateChoiceDictionary(Hagi4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Hagi4)));
            currentPlayer.UpdateChoiceDictionary(Kiku4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Kiku4)));
            currentPlayer.UpdateChoiceDictionary(Kiri4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Kiri4)));
            currentPlayer.UpdateChoiceDictionary(Matsu4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Matsu4)));
            currentPlayer.UpdateChoiceDictionary(Momiji4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Momiji4)));
            currentPlayer.UpdateChoiceDictionary(Sakura4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Sakura4)));
            currentPlayer.UpdateChoiceDictionary(Susuki4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Susuki4)));
            currentPlayer.UpdateChoiceDictionary(Ume4, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Ume4)));
            currentPlayer.UpdateChoiceDictionary(Akatan, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Akatan)));
            currentPlayer.UpdateChoiceDictionary(Aotan, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Aotan)));
            currentPlayer.UpdateChoiceDictionary(Kozan, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Kozan)));
            currentPlayer.UpdateChoiceDictionary(Oozan, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Oozan)));
            currentPlayer.UpdateChoiceDictionary(TsukimideIppai, BitCnt1((long)(currentPlayer.TokutenfudaPattern & TsukimideIppai)));
            currentPlayer.UpdateChoiceDictionary(HanamideIppai, BitCnt1((long)(currentPlayer.TokutenfudaPattern & HanamideIppai)));
            currentPlayer.UpdateChoiceDictionary(Teppou, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Teppou)));
            currentPlayer.UpdateChoiceDictionary(MatsuKiriBouzu, BitCnt1((long)(currentPlayer.TokutenfudaPattern & MatsuKiriBouzu)));
            currentPlayer.UpdateChoiceDictionary(InoShikaChou, BitCnt1((long)(currentPlayer.TokutenfudaPattern & InoShikaChou)));
            currentPlayer.UpdateChoiceDictionary(Shikou, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Shikou)));
            currentPlayer.UpdateChoiceDictionary(Nanatan, BitCnt1((long)(currentPlayer.TokutenfudaPattern & Nanatan)));
            return currentPlayer.ChoiceThrowAI1(bafuda);
        }
        /// <summary>
        /// 六百間をプレイする。
        /// </summary>
        public void Play()
        {
            while (isNextPlayer())
            {
                Player currentPlayer = players.Single(player => player.PlayerState == State.Running);

                if (currentPlayer.PlayerHandle == Handle.Auto)
                {
                    // 手札から場札に一枚出す。
                    currentPlayer.ThrowToAuto(bafuda, currentPlayer.Hand[0]);
                    
                    // 山札から場札に一枚出す。
                    Card popCard = yamafuda.CardPop();

                    // フラグチェック
                    if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                    {
                        currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                    }
                    currentPlayer.AddHand(popCard);
                    currentPlayer.ThrowToAuto(bafuda, popCard);
                    CalculateScore(currentPlayer);
                    
                    // 現在の手番のプレイヤーの状態を待機に設定する。
                    currentPlayer.PlayerState = State.Waiting;
                    if (isNextPlayer())
                    {
                        // 次のプレイヤーのステートをRunningに変更する。
                        GetNextPlayer(currentPlayer).PlayerState = State.Running;
                    }
                    continue;
                }
                else if (currentPlayer.PlayerHandle == Handle.AI1)
                {
                    // 手札から場札に一枚出す。
                    currentPlayer.ThrowToAI1(bafuda, ChoiceThrowAI1(currentPlayer));

                    // 山札から場札に一枚出す。
                    Card popCard = yamafuda.CardPop();

                    // フラグチェック
                    if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                    {
                        currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                    }
                    currentPlayer.AddHand(popCard);
                    currentPlayer.ThrowToAI1(bafuda, popCard);
                    CalculateScore(currentPlayer);
                    
                    // 次の手番のプレイヤーを呼び出す。
                    currentPlayer.PlayerState = State.Waiting;
                    if (isNextPlayer())
                    {
                        GetNextPlayer(currentPlayer).PlayerState = State.Running;
                    }
                    continue;
                }
                else if (currentPlayer.PlayerHandle == Handle.Manual)
                {
                    Fill();
                    Console.Write("どの札を出しますか？　出したい札のインデックスを入力して下さい。\n");
                    ConsoleKey consoleKey = Console.ReadKey(true).Key;
                    switch (consoleKey)
                    {
                        case ConsoleKey.D0:
                            if (currentPlayer.Hand.Count < 1) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(0));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[0]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D1:
                            if (currentPlayer.Hand.Count < 2) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(1));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(1));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[1]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D2:
                            if (currentPlayer.Hand.Count < 3) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(2));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(2));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[2]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {

                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D3:
                            if (currentPlayer.Hand.Count < 4) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(3));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(3));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[3]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D4:
                            if (currentPlayer.Hand.Count < 5) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(4));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(4));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[4]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D5:
                            if (currentPlayer.Hand.Count < 6) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(5));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(5));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[5]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D6:
                            if (currentPlayer.Hand.Count < 7) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(6));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(6));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[6]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                        case ConsoleKey.D7:
                            if (currentPlayer.Hand.Count < 8) break;
                            Console.Write("{0} を出しますか？ Enter or N\n", currentPlayer.GetHandPattern(7));
                            consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.Enter)
                            {
                                Console.Write("{0} を出しました。\n", currentPlayer.GetHandPattern(7));
                                currentPlayer.ThrowToManual(bafuda, currentPlayer.Hand[7]);
                                // 山札から場札に一枚出す。
                                Card popCard = yamafuda.CardPop();

                                Console.Write("{0} を引きました。\n", popCard.CardPattern);

                                // フラグチェック
                                if (popCard.CardPattern == Pattern.YanagiAndKaeru)
                                {
                                    currentPlayer.PlayerFlag |= PlayerFlags.HandInYanagiAndKaeru;
                                }
                                currentPlayer.AddHand(popCard);
                                currentPlayer.ThrowToManual(bafuda, popCard);
                                CalculateScore(currentPlayer);

                                // 次の手番のプレイヤーを呼び出す。
                                currentPlayer.PlayerState = State.Waiting;
                                if (isNextPlayer())
                                {
                                    GetNextPlayer(currentPlayer).PlayerState = State.Running;
                                }
                                break;
                            }
                            else if (consoleKey == ConsoleKey.N)
                            {
                                continue;
                            }
                            else
                                continue;
                    }
                }
            }
            IEnumerable<Player> order = players.OrderByDescending(player => player.Score);
            int i = 1;
            Console.Write("\n");
            foreach (Player player in order)
            {
                Console.Write("{0}位（{1}さん）{2}点\n", i++, player.Name, player.Score);
            }
        }
        public Player GetNextPlayer(Player currenPlayer)
        {
            int currentIndex = players.IndexOf(currenPlayer);
            if (currentIndex == players.Count - 1)
            {
                return players[0];
            }
            else
            {
                return players[++currentIndex];
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
        Auto = 0x2,
        AI1 = 0x4
    }
    [Flags]
    enum BafudaFlags
    {
        ThreePiecesOfTheSameKind = 0x1
    }
    [Flags]
    enum PlayerFlags
    {
        HandInYanagiAndKaeru = 0x1
    }
}
