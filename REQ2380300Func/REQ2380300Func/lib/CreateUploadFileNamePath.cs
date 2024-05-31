using Microsoft.Extensions.Logging;
using REQ2380300Func.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace REQ2380300Func.lib
{
    public class CreateUploadFileNamePath
    {
        public static IReadOnlyList<ListFileNameModel> CreateFilePath(ILogger log, IReadOnlyList<string> sauceNameList, string inFilename, string catelogID)
        {
            List<ListFileNameModel> files = new List<ListFileNameModel>();

            // NULLチェック
            if (sauceNameList == null)
            {
                // エラーログ
                log.LogInformation($"method IReadOnlyList parameter sauceNameList :{sauceNameList}");
                return files;

            }

            foreach (var item in sauceNameList)
            {
                string[] fileNameParts = item.Split("_");
                //if (fileNameParts.Length >= 4) //                if (fileNameParts.Length >= 3)
                if (fileNameParts.Length >= 4) //                if (fileNameParts.Length >= 3)
                {
                    // 3番目の要素をディレクトリ名として抽出
                    string directoryName = fileNameParts[2]; //add_S000_storeno_yyyy.dat
                    string subDirectory = "";
                    string newFileName = $"";
                    if (inFilename.ToUpper().StartsWith("HULFT_SEND_CHEK_CS".ToUpper()))
                    {
                        string[] checkNameParts = item.Split(".");
                        string newtempFileName = $"{checkNameParts[0]}.zip"; //ID_SEND_CHEK_CS_4_0000011720_20231219175453.dat-> .zip
                        string[] newfileparts = newtempFileName.Split("_");  //ID SEND CHEK CS 4 0000011720 20231219175453.zip
                        newFileName = $"{newfileparts[1]}_{newfileparts[2]}_{newfileparts[3]}_{newfileparts[4]}_{newfileparts[5]}_{newfileparts[6]}"; //SEND CHEK CS 4 0000011720 20231219175453.zip
                    }
                    else
                    {
                        // newFileName = item;
                        newFileName = $"{fileNameParts[1]}_{fileNameParts[2]}_{fileNameParts[3]}";
                    }


                    // 新しいファイル名add_S000_storeno_yyyy.dat
                    //string newFileName = $"{fileNameParts[0]}_{fileNameParts[3]}";

                    //ファイル確認用ログ
                    //log.LogDebug($"inFilename : {inFilename}");

                    // 新しいファイル名に基づいてサブディレクトリを決定 //条件分岐はinFilenameで行う
                    //listかswitchで行うよう改修する
                    //if (item.StartsWith("S010")) //inFilename == "HULFT_WAON_MHTA.zip" 

                    if (inFilename.ToUpper().StartsWith("HULFT_SEND_CHEK_CS".ToUpper())) {

                        string checkdirectoryName = fileNameParts[5];

                        //subDirectory = $"credit-system-outbound/{checkdirectoryName}/check";
                        subDirectory = $"/outbound/{catelogID}/{checkdirectoryName}/chek";
                    } else {
                        subDirectory = $"/outbound/{catelogID}/{directoryName}/nega";
                    }

                    // ディレクトリの作成パスを指定
                    string directoryPath = subDirectory.Trim('/') + "/";
                    var primary = new ListFileNameModel();
                    primary.newFileName = (directoryPath + "primary/" + newFileName).Replace("//", "/"); //directoryPathで/を後ろに入れる
                    primary.sauceFileName = item;

                    var secondary = new ListFileNameModel();
                    secondary.newFileName = (directoryPath + "secondary/" + newFileName).Replace("//", "/"); ;
                    secondary.sauceFileName = item;

                    files.Add(primary);
                    files.Add(secondary);
                }
                else
                {
                    log.LogError($"解凍後のファイル名が正しくありません。 :  {item}");// エラーログかつエクセプションを記載する
                    throw new Exception($"サブディレクトリ生成失敗 : {item}"); //呼び出し元に例外が飛ぶ
                }
            }

            return files;
        }
    }
}