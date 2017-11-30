using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MatrixMakerToolFromCSV
{
    class Program
    {
        private static string OUTPATH = "";

        static void Main(string[] args)
        {

            //引数チェック
            if (!AnalysisArgs(args)) return;

            //ファイル開いてCSVをリストにする。
            string csvPath = args[0].ToString();
            OUTPATH = csvPath + "_out.csv";
            //内部で処理するデータ作り
            Dictionary<string, List<string>> csvDataList = new Dictionary<string, List<string>>();
            using (StreamReader sr = new StreamReader(csvPath, Encoding.GetEncoding("shift_jis")))
            {
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    Console.WriteLine(line);
                    string[] values = line.Split(',');
                    //１つ目はキー、２つ目以降はCSVデータとして扱う
                    csvDataList.Add(values[0], values.Skip(1).ToList<string>());
                }
            }

            //csvDataList.Reverse();//反対から処理したい場合
            //時間がないし、こんがらがるのでイテレータ使うのやめた。再帰処理むずい、、、
            string outputLine = "";
            Console.WriteLine("出力中");
            writeMx(csvDataList, outputLine);
            Console.WriteLine("出力終わり");
        }

        /// <summary>
        /// 再帰処理用
        /// </summary>
        /// <param name="elements"></param>
        private static void writeMx ( Dictionary<string, List<string>> dic , string line )
        {
            if (dic.Count == 1)
            {
                using (StreamWriter writer = new StreamWriter(OUTPATH, true, Encoding.GetEncoding("shift_jis")))
                {
                    int lc = 0;
                    foreach (var data in dic.Values.First())
                    {
                        var s = line + "".PadLeft(lc, '\t') + data;
                        writer.WriteLine(s);
                        lc++;
                    }
                }
            }
            else
            {
                //要素にあるcsvデータ毎に、さらにまだ掘る
                //Excel用に貼り付けしたいので、データが変わったらタブ区切りを仕込んでおく
                int loopCount = 0;
                foreach (var data in dic.Values.First())
                {
                    //貼り付けたいExcelが項目ごとに列を持っているので、出力項目の左右に必要数分tabを差し込んでいる。
                    string untilNow = line + "".PadLeft(loopCount, '\t') + data + "\t".PadLeft(dic.Values.First().Count - loopCount, '\t');
                    writeMx(dic.Skip(1).ToDictionary(p => p.Key, p => p.Value), untilNow);
                    loopCount++;

                    Console.Write("★");
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("☆");


                }
            }
        }
        /// <summary>
        /// 引数チェック
        /// </summary>
        private static bool AnalysisArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("引数がありません。");
                return false;
            }

            if ( ! File.Exists(args[0]))
            {
                Console.WriteLine("引数に指定されたファイルがありません。 path:{0}", args[0]);
                return false;
            }

            return true;
        }
    }
}
