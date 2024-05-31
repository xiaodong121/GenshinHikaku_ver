using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Polly;
using REQ2380300Func.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace REQ2380300Func.lib
{
    public class UzipStreamToBlob
    {
        public async Task<IReadOnlyList<string>> UzipStreamToBlobAsync(ILogger log, Stream inBlobStream = null, BlobContainerClient outBlobClient = null, string tempDir = "", string addName = "")
        {
            List<string> result = new List<string>();
            try { 
                

                // NULLチェック
                if (inBlobStream == null || outBlobClient == null)
                {
                    // エラーログ
                    log.LogInformation($"method UzipStreamToBlobAsync parameter inBlobStream :{inBlobStream}， outBlobClient: {outBlobClient}");
                    return result;

                }

                // 解凍Upload
                log.LogInformation($"解凍Upload開始");
                using (ZipArchive a = new ZipArchive(inBlobStream, ZipArchiveMode.Read))
                {
                    //エントリを列挙する
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        // Debugログ掃き出し
                        //log.LogDebug($"Zip内ファイル名:{e.FullName}");

                        // Zipストーリム開く
                        using (Stream stream = e.Open())
                        {
                            // アップロード名の作成 区切り文字はアンダーバー
                            var upName = ("/" + tempDir.Trim('/') + "/").Replace("//", "") + addName + "_" + e.Name;
                            log.LogInformation($"upName：{upName}");
                            result.Add(upName);
                            // Upload実施、同じファイルがあった場合上書きする（要検討）
                            // リトライを入れる
                            // https://blog.shibayan.jp/entry/20190102/1546425897
                            // 失敗した場合には 2 秒 * リトライ回数分ずつ待ち時間を増やして 5 回まで試す（要検討）
                            await Policy
                                .Handle<Exception>()
                                .WaitAndRetryAsync(1,
                                retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                                onRetry: (response, calculatedWaitDuration) =>
                                {
                                    // リトライの際にワーニングログを出す
                                    log.LogWarning($"Failed attempt. Waited for {calculatedWaitDuration}. Retrying. {response.Message} - {response.StackTrace} FileName:{upName}");
                                })
                                .ExecuteAsync(async () => await outBlobClient.GetBlobClient(upName).UploadAsync(stream, true))
                            ;
                        }
                    }
                }

                

            }
            catch (Exception ex)
            {

                // エラーログを出力
                log.LogError(ex, "解凍処理-ERROR:以下のメッセージを参照してください:{ErrorMessage}", ex.Message);

                //throw; fuctionsを丸ごと再実行はしない
            }

            GC.Collect();

            return result;



        }
    }
}
