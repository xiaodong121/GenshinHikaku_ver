using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using REQ2380300Func.Models;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.WebJobs;
using Microsoft.Identity.Client;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using Azure;
using Polly;

namespace REQ2380300Func.lib
{
    public class ApiEndpointConnection
    {
        public async Task<String> ApiEndpointConnectionAsync(ILogger log, String url, HttpContent bodyContent,  IHttpClientFactory httpClientFactory, ExecutionContext context)
        {

            var content = "";

            if (bodyContent == null) {
                // エラーログ
                log.LogInformation($"method ApiEndpointConnectionAsync parameter bodyContent :{bodyContent}");
                throw new Exception($"bodyContentはNULLです : {bodyContent}"); //呼び出し元に例外が飛ぶ
            }

            // Azure AD アプリケーションのクライアント ID とクライアント シークレット

            HttpClient client = httpClientFactory.CreateClient("signed");

            // 検証環境
            string clientId = Environment.GetEnvironmentVariable("clientId");
            // string clientId = "93196218-e939-4ea2-915d-d5320c883caa";

            string tenantId = Environment.GetEnvironmentVariable("tenantId");
            //string tenantId = "86adf02b-3885-41ce-9d93-7cb423e50745";

            string resource = Environment.GetEnvironmentVariable("resource");
            // string resource = "api://e56d8bec-4997-4292-b961-af488271f3d6/.default";

            // 本番環境
            // string clientId = "354109a3-65d9-4380-bd2f-d379833fa0ae";

            // string tenantId = "86adf02b-3885-41ce-9d93-7cb423e50745";

            // string resource = "api://60631684-4334-47a2-b78d-facb9ee79155/.default";


            try {
                log.LogInformation($"X509Certificate2(File.ReadAllText 開始");
                string parentPath = Directory.GetParent(context.FunctionDirectory).FullName;
                log.LogInformation($"context.allpath parentPath :{parentPath}");
                // 検証環境
                string certificateName = Environment.GetEnvironmentVariable("certificateName");
                log.LogInformation($"context.allpath value1 :{System.IO.Path.Combine(parentPath, "data", certificateName)}");
                var certificate = X509Certificate2.CreateFromPemFile(System.IO.Path.Combine(parentPath, "data", certificateName));

                // 本番環境
                // log.LogInformation($"context.allpath value1 :{System.IO.Path.Combine(parentPath, "data", "key-KaikeiDLP-ac-urishukei01-sp.pem")}");
                // var certificate = X509Certificate2.CreateFromPemFile(System.IO.Path.Combine(parentPath, "data", "key-KaikeiDLP-ac-urishukei01-sp.pem"));

                // X509Certificate2 cert = new X509Certificate2(File.ReadAllText(System.IO.Path.Combine(parentPath, "data", "key-kaikeidlp-sp02.pem")));
                log.LogInformation($"X509Certificate2(File.ReadAllText 終了");

                log.LogInformation($"DefaultRequestHeaders.Add(Authorization 開始");
                var bearer = await GetAccessTokenAsync(clientId, tenantId, certificate, resource, log);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearer}");
                log.LogInformation($"DefaultRequestHeaders.Add(Authorization 終了");

                
                log.LogInformation($"client.GetAsync({url}/statuses) 開始");
                // TODO START
                // return await client.PostAsync(path, json);

                // 失敗した場合には 2 秒 * リトライ回数分ずつ待ち時間を増やして １ 回まで試す（要検討）
                var responseMessege = await Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(1,
                            retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                            onRetry: (response, calculatedWaitDuration) =>
                            {
                                // リトライの際にワーニングログを出す
                                log.LogWarning($"Failed attempt. Waited for {calculatedWaitDuration}. Retrying. {response.Message} - {response.StackTrace} postURL:{url}");
                            })
                            .ExecuteAsync(async () => await client.PostAsync(url, bodyContent))
                        ;

                // var response = await client.GetAsync("https://v-jpe-kaikeidlp-api01-app.azurewebsites.net/statuses/29" + "?catalogId=TST_0001_sample_zenken&inOut=0&status=1");

                // TODO END
                // var response = client.GetAsync("https://v-jpe-kaikeidlp-api01-app.azurewebsites.net/statuses").Result;
                log.LogInformation($"client.GetAsync({url}/statuses) 終了");

                log.LogInformation($".Content.ReadAsStringAsync().Result{responseMessege.Content.ReadAsStringAsync().Result}");

                content = responseMessege.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {

                // エラーログを出力
                log.LogError(ex, "ステータスAPI-WARN:以下のエラーメッセージを参照してください:{ErrorMessage}", ex.Message);

                //throw; fuctionsを丸ごと再実行はしない
            }
            finally
            {
                //上記で定義された処理に該当する場合、何が起こっても実行する処理を入れられる→絶対やらないといけないことを書く eg.解放処理など
                //解放処理はいつやる？　newして、自動解放されない場合がある。newしたオブジェクトを開放する等の命令をよく使う
                GC.Collect(); //メモリ解放処理　処理が止まる場合があるので確実に開放したい場合だけに使う（影響のない場合のみ使用）
                log.LogInformation($"処理完了");
            }

            

            return content;
        }
        static async Task<string> GetAccessTokenAsync(string clientId, string tenantId, X509Certificate2 cert, string resource, ILogger log)
        {

            // Load the certificate into an X509Certificate object.
            // X509Certificate2 cert = new X509Certificate2(_path, _password);

            try
            {
                var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}/v2.0")
                .WithCertificate(cert)
                .Build();
                var scopes = new string[] { $"{resource}" };

                var authResult = await confidentialClient
                     .AcquireTokenForClient(scopes)
                     .ExecuteAsync();

                log.LogInformation($"authResult:{authResult.AccessToken}");


                string o365Token = authResult.AccessToken;
                log.LogInformation($"o365Token {o365Token}");


                return o365Token;

            }
            catch
            {
                throw;
            }


        }

    }
}
