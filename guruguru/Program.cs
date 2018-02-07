using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guruguru
{
    class Program
    {
        static void Main(string[] args)
        {
            Guruguru guruguru = new Guruguru();

            int[] hoge = { 1, 2, 3, 4, 5 };
            int[] foo = { 5, 7, 3, 5, 2 };
            int[] fo = { 5 };
            guruguru.Sum(hoge);
            guruguru.Max(hoge);
            guruguru.Max(foo);
            guruguru.Min(foo);

            // スゲーてきとーに書いたFunc<T,bool>のテストコード
            Func<int, bool> match = m =>
             {
                 return m % 2 == 0;
             };

            Func<string, bool> matchString = m =>
            {
                return m == "0";
            };

            guruguru.ForAll(foo, match);

            guruguru.Exists(foo, match);

            // null非許容と格闘
            //var bar = foo.Select(x => x.ToString()).ToArray();
            //var a = Enumerable.Range(1,10).Select(x => new Nina { Chan = x });
            //var xxx = a.Select(x => x.);

            guruguru.Skip(foo, 2);

            guruguru.KumaSkip(foo, 2);

            // できてない...
            guruguru.Take(foo, 2);

        }

        class Nina
        {
            public int? Chan { get; set; }
        }
    }

    /// <summary>
    /// 再帰は終了条件に気を付けること！
    /// http://bleis-tift.hatenablog.com/entry/20120119/1326944722
    /// ここの凄いとこは難しい内容なのに使うものが少ないから自然に書けてしまうとこ
    /// </summary>
    class Guruguru
    {
        /// <summary>
        /// 各値を合計した int の値を返す関数
        /// </summary>
        /// <param name="xs"></param>
        /// <returns></returns>
        public int Sum(int[] xs)
        {
            //列が空の場合
            if (xs.Count() == 0)
                // 配列が空の場合は0を返し、これ以上再帰しない
                return 0;

            //列が空でない場合
            else
            {
                // 先頭の要素とそれ以降の要素を持ってくる
                var y_ys = HeadTail(xs);
                // 先頭の要素　+　それ以外の要素で再帰
                // Item2に要素が無くなったら上のif文の「列が空の場合」で抜ける
                return y_ys.Item1 + Sum(y_ys.Item2);
            }
        }

        /// <summary>
        /// 任意の型の列の長さを返す
        /// 当たり前だけど、xs.Lengthとかそういうのは求めてないです
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs"></param>
        /// <returns></returns>
        public int Length<T>(T[] xs)
        {
            if (xs.Length == 0)
                return 0;
            else
            {
                var y_ys = HeadTail(xs);
                // ただ一づつ増やしていって、要素一つ減らして再帰してくだけ
                return 1 + Length(y_ys.Item2);
            }
        }

        /// <summary>
        /// int の列の最大値を返す関数
        /// </summary>
        /// <param name="xs"></param>
        /// <returns></returns>
        public int Max(int[] xs)
        {
            if (xs.Count() == 1)
            {
                return xs.First();
            }
            else
            {
                var x_xs = HeadTail(xs);
                var ret = Max(x_xs.Item2);

                return x_xs.Item1 < ret ? ret : x_xs.Item1;
            }
        }

        /// <summary>
        /// 列の最小値を返す min 関数
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public int Min(int[] aa)
        {
            if (aa.Count() == 1)
            {
                return aa.First();
            }
            else
            {
                var xxx = HeadTail(aa);
                var yyy = Min(xxx.Item2);

                return xxx.Item1 < yyy ? xxx.Item1 : yyy;
            }
        }

        /// <summary>
        /// 列の全要素が条件を満たす場合は true を、一つでも条件を満たさない要素がある場合は false を返す関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool ForAll<T>(T[] xs, Func<T, bool> pred)
        {
            // 「すべての要素が条件を満たす」というのは、言い換えれば「条件を満たさない要素が無い」となります。
            // 空の列が渡された場合に条件を満たさない要素はもちろんありませんので、true を返せばよい
            if (xs.Count() == 0)
                return true;
            else
            {
                var xxx = HeadTail(xs);

                return pred(xxx.Item1) && ForAll(xxx.Item2, pred);
            }
        }

        /// <summary>
        /// 列の要素のうち一つでも条件を満たす要素があれば true を、そうでない場合は false を返す関数 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xa"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool Exists<T>(T[] xa, Func<T, bool> pred)
        {
            if (xa.Count() == 1)
                // 終了条件ミスったtrueじゃないｓ
                return false;
            else
            {
                var dadada = HeadTail(xa);
                return pred(dadada.Item1) || Exists(dadada.Item2, pred);
            }
        }

        /// <summary>
        /// 条件が true を返す一番初めの要素を返す関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public T Find<T>(T[] xs, Func<T, bool> pred)
        {
            if (xs.Length == 0)
                // 引数に指定した型が参照型である場合にはnullを、数値型の場合には0を返す
                //return null;
                return default(T);
            else
            {
                var x_xs = HeadTail(xs);
                // falseなら再帰 わかりやすい
                return pred(x_xs.Item1) ? x_xs.Item1
                                : Find(x_xs.Item2, pred);
            }
        }

        /// <summary>
        /// 先頭から n 個捨て、n + 1 以降の列を返す関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public T[] Skip<T>(T[] xs, int n)
        {
            if (xs.Length == 0)
                return new T[0];
            else
            {
                var x_xs = HeadTail(xs);
                // n-- != 0 にしてて間違えたけどそれっぽくない
                return n-- != 1 ? Skip(x_xs.Item2, n--) : x_xs.Item2;
            }
        }

        /// <summary>
        /// Skip
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public IEnumerable<T> KumaSkip<T>(IEnumerable<T> xs, int n)
        {
            if (xs.Count() == 0)
                return Enumerable.Empty<T>();
            else
                return n > 0 ? Skip(HeadTail(xs.ToArray()).Item2, n - 1) : xs;
        }

        /// <summary>
        /// Take
        /// 空の列の扱いや、負数を与えられた場合の挙動は自由とします
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tt"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public T[] Take<T>(T[] tt, int c)
        {
            if (tt.Length == 0)
                return new T[0];
            else
            {
                if (c == 0)
                    return new T[0];
                else
                    // だんだん何やってるかわかんなくなってきた
                    return Cons(HeadTail(tt).Item1, Take(HeadTail(tt).Item2, c - 1));
            }
        }


        /// <summary>
        /// 引数xsを、先頭の要素とそれ以降の要素のタプルとして返す
        /// 先頭の要素がItem1
        /// それ以降の要素Item2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs"></param>
        /// <returns></returns>
        public static Tuple<T, T[]> HeadTail<T>(T[] xs)
        {
            Func<T[], T[]> tail = a =>
            {
                T[] res = new T[a.Length - 1];
                Array.Copy(a, 1, res, 0, res.Length);
                return res;
            };
            return Tuple.Create(xs[0], tail(xs));
        }

        /// <summary>
        /// 配列の先頭に要素を追加した新しい配列を返す関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="xs"></param>
        /// <returns></returns>
        public static T[] Cons<T>(T x, T[] xs)
        {
            T[] res = new T[xs.Length + 1];
            res[0] = x;
            Array.Copy(xs, 0, res, 1, xs.Length);
            return res;
        }

        /// <summary>
        /// Caseを2個投げるといい感じになる奴
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="xs"></param>
        /// <param name="emptyCase"></param>
        /// <param name="notEmptyCase"></param>
        /// <returns></returns>
        public static U Match<T, U>(this T[] xs, Func<U> emptyCase, Func<T, T[], U> notEmptyCase)
        {
            if (xs.Length == 0)
                return emptyCase();
            else
            {
                var x_xs = HeadTail(xs);
                return notEmptyCase(x_xs.Item1, x_xs.Item2);
            }
        }
    }
}
