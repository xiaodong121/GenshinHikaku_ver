using System;
using System.IO;
using System.Net.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.IO.Compression;
using Polly;
using System.Xml.Linq;
using REQ2380300Func.lib;
using Azure.Identity;
using Newtonsoft.Json;
using REQ2380300Func.Models;
using static Azure.Core.HttpHeader;
using System.Collections;
using System.Collections.Generic;

namespace REQ2380300Func
{
    public class REQ2380300Inbound01Func
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration _conf;
        private readonly ILogger<REQ2380300Inbound01Func> _logger;

        public REQ2380300Inbound01Func(IConfiguration config, IHttpClientFactory httpClientFactory, ILogger<REQ2380300Inbound01Func> log)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.httpClientFactory = httpClientFactory;
            _conf = config;
            _logger = log;
        }
        [Singleton]
        [FunctionName("REQ2380300Func")]
        //public async Task Run([BlobTrigger("inbound01/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log)
        //public async Task Run([BlobTrigger("system01/inbound/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log)
        // public async Task Run([BlobTrigger("system01/inbound/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log)
        // public async Task Run([BlobTrigger("inbound/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log, ExecutionContext context)
        // public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        // public async Task Run([BlobTrigger("system01/inbound/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log, ExecutionContext context)
        public async Task Run([BlobTrigger("ac-urishukei/inbound/{name}", Connection = "REQ2380300Inboud01")] Stream myBlob, string name, ILogger log, ExecutionContext context)
        {


           /* log.LogInformation($"C# Blob trigger function Processed blob Name:{name} Size: {myBlob.Length} Bytes");
            //ifでzip checkを書く
            if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // .zip形式ではない場合、エラーログを出力
                log.LogError($"解凍できない形式のファイルです: {name}");
                return; // 処理を中断
            }*/



            //log.LogInformation($"C# Blob trigger function Processed blob Name:{name} Size: {myBlob.Length} Bytes");
            //ifでzip checkを書く
            //コネクションとクライアント作成
            /*
            string name = "inbound/HULFT_WAON_MHTA_8.zip";
            string InConnection = _conf.GetValue<string>("REQ2380300Inboud01");
            BlobServiceClient _inblobServiceClient = new BlobServiceClient(InConnection);

            var _inBlobContainer = _inblobServiceClient.GetBlobContainerClient("system01");
            var myBlob = await _inBlobContainer.GetBlobClient(name).OpenReadAsync();
           */
            /* 
             * if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
             {
                 // .zip形式ではない場合、エラーログを出力
                 log.LogError($"解凍できない形式のファイルです: {name}");
                 return; // 処理を中断
             }
            */

            //inboundからtempに展開


            //From myBlob Stream
            // To TempBlob
            // 接続文字列をConfから持ってこれるようにする必要がある
            // 関数にはLogObjは渡すとして、掃き出し側をBlobObjとして渡すか、接続文字列渡すか検討
            log.LogInformation($"処理開始");

            /* 文字列を取得してstartwithで処理を行う
            var connectionString = "YourBlobStorageConnectionString";
            var containerName = "YourContainerName";
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                if (blobItem.Name.StartsWith("HULFT"))
                {

                }
            */
            // InboundBlob接続文字列取得

//接続文字列取得
            log.LogInformation($"InboundBlob接続文字列取得");
            // string InConnection = _conf.GetValue<string>("REQ2380300inbound01");
            // BlobContainerClient containerClient = new BlobContainerClient(new Uri(InConnection), new DefaultAzureCredential());
            //BlobServiceClient _inblobServiceClient = new BlobServiceClient(InConnection);
            // Get a credential and create a client object for the blob container.

            // BlobContainerClient containerClient = new BlobContainerClient(new Uri(InConnection), new DefaultAzureCredential());



            // log.LogInformation($"取得したInboundストレージアカウントサービス: {InConnection}");
            // TempBlob接続文字列取得
            log.LogInformation($"一時保管BLOB接続文字列取得");
 
            string Connection = _conf.GetValue<string>("REQ2380300Temp");
            //BlobServiceClient _blobServiceClient = new BlobServiceClient(Connection);
            BlobContainerClient TempcontainerClient = new BlobContainerClient(new Uri(Connection), new DefaultAzureCredential());
            log.LogInformation($"取得したInboundストレージアカウントサービス: {Connection}");
            // OutboundBlob接続文字列取得
            string outConnection = _conf.GetValue<string>("REQ2380300outbound01");
            // ストレージAccountサービス作成
            //BlobServiceClient _outblobServiceClient = new BlobServiceClient(outConnection);
            BlobContainerClient OutcontainerClient = new BlobContainerClient(new Uri(outConnection), new DefaultAzureCredential());
            log.LogInformation($"取得したoutboundストレージアカウントサービス: {outConnection}");
            // ストレージAccountサービス作成




            try
            {
                //新連係基盤内の想定版暫定連係基盤への接続(川崎が追加)
                // configを使ってon/offを切り替える
                log.LogInformation($"暫定連係基盤（新）接続文字列取得");
                string creditConnection = _conf.GetValue<string>("REQ2380300creditout01");
                BlobContainerClient CredtcontainerClient = new BlobContainerClient(new Uri(creditConnection), new DefaultAzureCredential());
                log.LogInformation($"取得した暫定連係基盤outboundストレージアカウントサービス: {creditConnection}");
                //string blobtestname = "test.txt";
                //log.LogInformation($"想定対象ファイル名: {blobtestname}");
                //var CreditBlob = await CredtcontainerClient.GetBlobClient(blobtestname).OpenReadAsync();
                //log.LogInformation($"新連係基盤の暫定連係基盤BLOB内ファイル名{blobtestname}を読み込み成功");
                /*　本番稼働の際テスト
                //暫定連係基盤接続文字列取得(本番稼働の際に利用する）　https://pjpecreditsysoutbounddfs.blob.core.windows.net/credit-system-outbound
                log.LogInformation($"暫定連係基盤接続文字列取得");
                string creditConnection = _conf.GetValue<string>("REQ2380300creditout01");
                BlobContainerClient CredtcontainerClient = new BlobContainerClient(new Uri(creditConnection), new DefaultAzureCredential());
                log.LogInformation($"取得した暫定連係基盤outboundストレージアカウントサービス: {creditConnection}");
                string creditname = "0000000020/primary/nega/S010_0000000020_20231124080053.dat";
                //   string creditname = "0000000020/primary/nega/S010_0000000020_20231124080053.dat";
                log.LogInformation($"想定ファイル名{creditname}");
                var CreditBlob = await CredtcontainerClient.GetBlobClient(creditname).OpenReadAsync();
                log.LogInformation($"暫定連係基盤BLOB内ファイル名{creditname}を読み込み成功");
                //var _inBlobContainer = _inblobServiceClient.GetBlobContainerClient("system01");
                //log.LogInformation($"Inboundコンテナ取得");
                */

                //暫定連係基盤テストが終わったら戻す範囲
                // string name = "inbound/HULFT_DENSHI_KINADD.zip";
                log.LogInformation($"想定対象ファイル名: {name}");
                // var myBlob = await containerClient.GetBlobClient(name).OpenReadAsync();
                 log.LogInformation($"Inboundコンテナオープン、ファイル名{name}");
                // Containerサービス作成
                // var _tempBlobContainer = _blobServiceClient.GetBlobContainerClient("temp");
                //var _tempBlobContainer = _blobServiceClient.GetBlobContainerClient("tmp");
                // Containerの存在有無を確認して無ければ作成


                // TODO START 登録APIに呼ぶ
                ApiEndpointConnection apiEndpointConnection = new ApiEndpointConnection();

                //カタログID関数の呼び出し
                String [] fileNameParts = name.Split("/");
                String catelogID = CreateCatelogidClass.CreateCatelogidClassAsync(log, fileNameParts[1]);

                //TODO appIDはまだわからない
                ApiPostModel apiPostModel = new ApiPostModel();
                apiPostModel.catalogId = catelogID;
                apiPostModel.inOut = "0";
                apiPostModel.status = "1";

                string jsonString = System.Text.Json.JsonSerializer.Serialize(apiPostModel);

                var contentType = "application/json";

                HttpContent loginJson = new StringContent(jsonString, Encoding.UTF8, contentType);

                string apiURL = Environment.GetEnvironmentVariable("apiURL");

                // var loginUrl = "https://v-jpe-kaikeidlp-api01-app.azurewebsites.net/statuses/";
                var loginUrl = $"{apiURL}/statuses/";

                //登録APIの処理部分
                // 登録APIに呼ぶ
                String loginContent = await apiEndpointConnection.ApiEndpointConnectionAsync(log, loginUrl, loginJson, httpClientFactory, context);

                log.LogInformation($"C# loginContent: {loginContent}");
                // 登録APIのレスポンスを取る
                ResponseModel loginMyModel = new ResponseModel
                {
                    apiVersion = new Apiversion(),
                    businessData = new Businessdata
                    {
                        body = new Body()
                    },
                    destSystem = new Destsystem
                    {
                        result = new Result()
                    }
                };
                loginMyModel = JsonConvert.DeserializeObject<ResponseModel>(loginContent);
                log.LogInformation($"C# loginMyModel: {loginMyModel}");

                // TODO END

                //解凍処理

                await TempcontainerClient.CreateIfNotExistsAsync();
                log.LogInformation($"tmp確認済、解凍処理開始");
                UzipStreamToBlob uzipStreamToBlob = new UzipStreamToBlob();


                // 保存先ディレクトリの判定などは別途実施する
                var tempFileNameList = await uzipStreamToBlob.UzipStreamToBlobAsync(log, myBlob, TempcontainerClient, "", loginMyModel.businessData.body.processId);
                //エクセプションが出る
                log.LogInformation($"解凍処理完了");
                //var outFilePathList = CreateUploadFileNamePath.CreateFilePath(log, tempFileNameList);
                if (tempFileNameList == null) {
                    throw new Exception($"tempFileNameListはNULLです : {tempFileNameList}"); //呼び出し元に例外が飛ぶ
                }

                //ファイルパス作成

                //outboundにアップロード               

                var outUpload = new BlobUploadClass();


                String config = Environment.GetEnvironmentVariable("config");

                //暫定連係基盤向けファイルパス作成(川崎が新規追加)
                log.LogInformation($"暫定連係基盤向けファイルパス作成");
                var creditFilePathList = CreateCreditFileNamePath.CreateCreditFilePath(log, tempFileNameList, System.IO.Path.GetFileName(name));
                //新連係基盤向けファイルパス作成
                log.LogInformation($"新連係基盤向けファイルパス作成");
                var outFilePathList = CreateUploadFileNamePath.CreateFilePath(log, tempFileNameList, System.IO.Path.GetFileName(name), catelogID);



                //暫定連係基盤向けアップロード処理 outfilepathlistをcreditFilePathListに変更する(川崎が新規追加)
                // log.LogInformation($"暫定連係基盤アップロード処理開始");
                // await outUpload.outBlobUploadAsync(log, TempcontainerClient, CredtcontainerClient, tempFileNameList, creditFilePathList);
                

                
                
                //var outboundContainer = _outblobServiceClient.GetBlobContainerClient("system01");
               
                //新連係基盤向けアップロード処理
                log.LogInformation($"新連係基盤アップロード処理開始");
                await outUpload.outBlobUploadAsync(log, TempcontainerClient, OutcontainerClient, tempFileNameList, outFilePathList, CredtcontainerClient, creditFilePathList);
               
                log.LogInformation($"処理終了");

                // TODO STRAT 更新APIに呼ぶ
                UpdateRequestModel updateRequestModel = new UpdateRequestModel
                {
                    status = "2"
                };
                string updatejsonString = System.Text.Json.JsonSerializer.Serialize(updateRequestModel);

                HttpContent updateJson = new StringContent(updatejsonString, Encoding.UTF8, contentType);                

                var updateUrl = $"{apiURL}/statuses/" + loginMyModel.businessData.body.processId;

                log.LogInformation($"updateUrl: {updateUrl}");

                // リクエストモデルはJSONに変換する
                var updateMyJson = JsonConvert.SerializeObject(updateRequestModel);

                // リクエストJSONはStringContentに変換する
                var updateMyString = new StringContent(updateMyJson, Encoding.UTF8, "application/json");

                // 更新APIに呼ぶ
                String updateContent = await apiEndpointConnection.ApiEndpointConnectionAsync(log, updateUrl, updateJson, httpClientFactory, context);

                // TODO END



            }
            catch (Exception ex)
            {

                // エラーログを出力
                _logger.LogError(ex, "エラー:以下のエラーメッセージを参照してください:{ErrorMessage}", ex.Message);

                //throw; fuctionsを丸ごと再実行はしない
            }
            finally
            {
                //上記で定義された処理に該当する場合、何が起こっても実行する処理を入れられる→絶対やらないといけないことを書く eg.解放処理など
                //解放処理はいつやる？　newして、自動解放されない場合がある。newしたオブジェクトを開放する等の命令をよく使う
                GC.Collect(); //メモリ解放処理　処理が止まる場合があるので確実に開放したい場合だけに使う（影響のない場合のみ使用）
                log.LogInformation($"FUNCTIONが処理完了");
            }

        }
    }
}