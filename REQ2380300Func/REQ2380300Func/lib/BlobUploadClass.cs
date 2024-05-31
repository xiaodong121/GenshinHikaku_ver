using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Polly;
using REQ2380300Func.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace REQ2380300Func.lib
{
    public class BlobUploadClass
    {
        public async Task outBlobUploadAsync(ILogger log, BlobContainerClient tempBlob = null, BlobContainerClient outBlobClient = null, IReadOnlyList<string> tempFileList = null, IReadOnlyList<ListFileNameModel> outFileList = null, BlobContainerClient creditBlob = null, IReadOnlyList<ListFileNameModel> creditFileList = null)
        {

            try { 
            
                    // NULLチェック
                    if (tempBlob == null || outBlobClient == null || tempFileList == null || outFileList == null)
                    {
                        // エラーログ
                        log.LogInformation($"method outBlobUploadAsync parameter tempBlob :{tempBlob}， outBlobClient: {outBlobClient}, tempFileList：{tempFileList}, outFileList : {outFileList}");
                        return;
                
                    }

                    foreach (var item in tempFileList)
                    {

                        var tempStream = await tempBlob.GetBlobClient(item).OpenReadAsync();
                        // tempStreamのNullチェックとitemの内容をログに記録
                        if (tempStream == null)
                        {
                            log.LogError($"tempstreamのファイルがnullとなっています。: {item}");
                            continue; // Nullの場合は次のファイルに進む
                        }

                        //log.LogInformation($"処理中のファイル名前: {item}"); //item名前をログで出力
                        // ストリームの位置をデータの先頭に戻す


                        foreach (var outItem in outFileList.OrderBy(x => x.sauceFileName == item).ToList())
                        {
                            //   log.LogInformation($"アップロード対象のファイル名: {outItem.newFileName}"); //outItem名前をログで出力
                                                                                // ストリームの位置をデータの先頭に戻す
                            tempStream.Position = 0;
                            await Policy
                                .Handle<Exception>()
                                .WaitAndRetryAsync(1,
                                retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                                onRetry: (response, calculatedWaitDuration) =>
                                {
                                    // リトライの際にワーニングログを出す
                                    log.LogWarning($"Failed attempt. Waited for {calculatedWaitDuration}. Retrying. {response.Message} - {response.StackTrace}");
                                })
                                .ExecuteAsync(async () => await outBlobClient.GetBlobClient(outItem.newFileName).UploadAsync(tempStream, true));
                        }

                        String config = Environment.GetEnvironmentVariable("config");
                        if (config != null && config.Equals("True")) {
                            foreach (var creditItem in creditFileList.OrderBy(x => x.sauceFileName == item).ToList())
                            {
                                //   log.LogInformation($"アップロード対象のファイル名: {outItem.newFileName}"); //outItem名前をログで出力
                                // ストリームの位置をデータの先頭に戻す
                                tempStream.Position = 0;
                                await Policy
                                    .Handle<Exception>()
                                    .WaitAndRetryAsync(1,
                                    retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                                    onRetry: (response, calculatedWaitDuration) =>
                                    {
                                        // リトライの際にワーニングログを出す
                                        log.LogWarning($"Failed attempt. Waited for {calculatedWaitDuration}. Retrying. {response.Message} - {response.StackTrace}");
                                    })
                                    .ExecuteAsync(async () => await outBlobClient.GetBlobClient(creditItem.newFileName).UploadAsync(tempStream, true));
                            }
                        }

                    }

            }
            catch (Exception ex)
            {

                // エラーログを出力
                log.LogError(ex, "アップロード エラー:以下のエラーメッセージを参照してください:{ErrorMessage}", ex.Message);

                //throw; fuctionsを丸ごと再実行はしない
            }

        }
    }
}
