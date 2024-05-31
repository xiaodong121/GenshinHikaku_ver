﻿using Microsoft.Extensions.Logging;
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
    public class CreateCreditFileNamePath
    {
        public static IReadOnlyList<ListFileNameModel> CreateCreditFilePath(ILogger log, IReadOnlyList<string> sauceNameList, string inFilename)
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
                    // 3番目の要素をディレクトリ名として抽出　修正点２
                    string directoryName = fileNameParts[2]; //ID_S000_storeno_yyyy.dat
                    string subDirectory = $"";
                    string newFileName = $"";

                    // ファイル名形式S000_storeno_yyyy.dat（暫定連係基盤ではIDを除く
                    //checkデータでは.dat→.zipへ変更
                    if (inFilename.ToUpper().StartsWith("HULFT_SEND_CHEK_CS".ToUpper()))
                    {
                        string[] checkNameParts = item.Split(".");
                        string newtempFileName = $"{checkNameParts[0]}.zip"; //ID_SEND_CHEK_CS_4_0000011720_20231219175453.dat-> .zip
                        string[] newfileparts = newtempFileName.Split("_");  //ID SEND CHEK CS 4 0000011720 20231219175453.zip
                        newFileName = $"{newfileparts[1]}_{newfileparts[2]}_{newfileparts[3]}_{newfileparts[4]}_{newfileparts[5]}_{newfileparts[6]}"; //SEND CHEK CS 4 0000011720 20231219175453.zip
                    }
                    else
                    {
                        newFileName = $"{fileNameParts[1]}_{fileNameParts[2]}_{fileNameParts[3]}";
                    }

                    //ファイル確認用ログ
                    //log.LogDebug($"inFilename : {inFilename}");

                    // 新しいファイル名に基づいてサブディレクトリを決定 //条件分岐はinFilenameで行う
                    //listかswitchで行うよう改修する
                    //if (item.StartsWith("S010")) //inFilename == "HULFT_WAON_MHTA.zip" 
                    if (inFilename.ToUpper().StartsWith("HULFT_WAON_MHTA".ToUpper())) //必ず両方大文字として判定する
                    {
                        subDirectory = $"credit-system-outbound/{directoryName}/nega";
                    }
                    else if (inFilename.ToUpper().StartsWith("HULFT_DENSHI_KINADD".ToUpper())) //必ず両方大文字として判定する
                    {
                        subDirectory = $"credit-system-outbound/{directoryName}/nega";
                    }
                    else if (inFilename.ToUpper().StartsWith("HULFT_DENSHI_KINDEL".ToUpper())) //必ず両方大文字として判定する

                    {
                        subDirectory = $"credit-system-outbound/{directoryName}/nega";
                    }
                    else if (inFilename.ToUpper().StartsWith("HULFT_DENSI_IPPAN_MHTA".ToUpper())) //必ず両方大文字として判定する

                    {
                        subDirectory = $"credit-system-outbound/{directoryName}/nega";
                    }
                    else if (inFilename.ToUpper().StartsWith("HULFT_OWNER_NEGA_MHTA".ToUpper())) //必ず両方大文字として判定する

                    {
                        subDirectory = $"credit-system-outbound/{directoryName}/nega";
                    }
                    else if (inFilename.ToUpper().StartsWith("HULFT_SEND_CHEK_CS".ToUpper())) //必ず両方大文字として判定する　修正点２
                    {
                        //checkfileのみディレクトリパスの_のロジックが違う
                        //checkNameParts[0]=SEND_CHEK_CS_4_0000011720_20231219175453 checkNameParts[1]=.dat
                        string chekdirectoryName = fileNameParts[5];

                        //subDirectory = $"credit-system-outbound/{checkdirectoryName}/check";
                        subDirectory = $"credit-system-outbound/{chekdirectoryName}/chek";
                    }
                    //elseでerrorを書く
                    else
                    {
                        // どの条件にも一致しない場合エラーログを表示する
                        log.LogError($"サブディレクトリ生成失敗 : {item}");
                        throw new Exception($"サブディレクトリ生成失敗 : {item}"); //呼び出し元に例外が飛ぶ
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