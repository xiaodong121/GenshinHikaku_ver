using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REQ2380300Func.lib
{
    /*
     Azureの環境変数で、 filename = HULFT_WAON_MHTA,HULFT_DENSHI_KINADD,...,HULFT_SEND_CHEK_CS　を設定
    functionsで環境変数を呼び出す
    文字列をsplitして、リストにする。
    文字列をsplitしてリストにする方法
    https://qiita.com/turupon/items/0f8bc187640018f63aea#:~:text=%E3%83%97%E3%83%AD%E3%82%B0%E3%83%A9%E3%83%A0%E5%81%B4%E3%81%A7%E7%92%B0%E5%A2%83%E5%A4%89%E6%95%B0%E3%82%92%E5%8F%96%E5%BE%97%E5%BE%8C%E3%81%AB%E3%80%81%E3%81%9D%E3%81%AE%E5%8C%BA%E5%88%87%E3%82%8A%E6%96%87%E5%AD%97%E3%80%8C%2C%E3%80%8D%E3%82%92%E5%88%A9%E7%94%A8%E3%81%97%E3%81%A6%E3%83%AA%E3%82%B9%E3%83%88%E5%8C%96%E3%82%92%E8%A1%8C%E3%81%84%E3%81%BE%E3%81%99%E3%80%82%20split_env.py%20import%20os%20env_list%20%3D%20os.getenv%28%27TEST_ENV%27%29.split%28%27%2C%27%29%20print%28f%22env_list%3A%7Benv_list%7D%22%29,is%20%27%2C%20len%28env_list%29%29%20for%20env%20in%20env_list%3A%20print%28env%29
    環境変数の設定と呼び出し方法　https://playfab-master.com/azure-environment-variable
     カタログIdの環境変数
   　name（条件を判定する)の環境変数
     */
    public class CreateCatelogidClass
    {
        public static String CreateCatelogidClassAsync(ILogger log, String name) {

            // NULLチェック
            if (name == null) {
                // エラーログ
                log.LogInformation($"method IReadOnlyList parameter name :{name}");
                throw new Exception($"method IReadOnlyList parameter name :{name}"); //呼び出し元に例外が飛ぶ
            }
            string[] fileNameParts = name.Split(".");

            String catalogId = "";
            // fileNameParts
            if (fileNameParts.Length >= 2) {
                catalogId = Environment.GetEnvironmentVariable(fileNameParts[0]);

                // NULLチェック
                if (catalogId == null)
                {
                    // エラーログ
                    log.LogInformation($"method IReadOnlyList parameter catalogId :{catalogId}");
                    throw new Exception($"catelogIDはNULLです : {catalogId}"); //呼び出し元に例外が飛ぶ

                }
            }

            return catalogId;


        }
    }
}
