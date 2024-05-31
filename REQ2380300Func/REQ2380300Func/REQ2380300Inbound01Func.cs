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
            //if��zip check������
            if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // .zip�`���ł͂Ȃ��ꍇ�A�G���[���O���o��
                log.LogError($"�𓀂ł��Ȃ��`���̃t�@�C���ł�: {name}");
                return; // �����𒆒f
            }*/



            //log.LogInformation($"C# Blob trigger function Processed blob Name:{name} Size: {myBlob.Length} Bytes");
            //if��zip check������
            //�R�l�N�V�����ƃN���C�A���g�쐬
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
                 // .zip�`���ł͂Ȃ��ꍇ�A�G���[���O���o��
                 log.LogError($"�𓀂ł��Ȃ��`���̃t�@�C���ł�: {name}");
                 return; // �����𒆒f
             }
            */

            //inbound����temp�ɓW�J


            //From myBlob Stream
            // To TempBlob
            // �ڑ��������Conf���玝���Ă����悤�ɂ���K�v������
            // �֐��ɂ�LogObj�͓n���Ƃ��āA�|���o������BlobObj�Ƃ��ēn�����A�ڑ�������n��������
            log.LogInformation($"�����J�n");

            /* ��������擾����startwith�ŏ������s��
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
            // InboundBlob�ڑ�������擾

//�ڑ�������擾
            log.LogInformation($"InboundBlob�ڑ�������擾");
            // string InConnection = _conf.GetValue<string>("REQ2380300inbound01");
            // BlobContainerClient containerClient = new BlobContainerClient(new Uri(InConnection), new DefaultAzureCredential());
            //BlobServiceClient _inblobServiceClient = new BlobServiceClient(InConnection);
            // Get a credential and create a client object for the blob container.

            // BlobContainerClient containerClient = new BlobContainerClient(new Uri(InConnection), new DefaultAzureCredential());



            // log.LogInformation($"�擾����Inbound�X�g���[�W�A�J�E���g�T�[�r�X: {InConnection}");
            // TempBlob�ڑ�������擾
            log.LogInformation($"�ꎞ�ۊ�BLOB�ڑ�������擾");
 
            string Connection = _conf.GetValue<string>("REQ2380300Temp");
            //BlobServiceClient _blobServiceClient = new BlobServiceClient(Connection);
            BlobContainerClient TempcontainerClient = new BlobContainerClient(new Uri(Connection), new DefaultAzureCredential());
            log.LogInformation($"�擾����Inbound�X�g���[�W�A�J�E���g�T�[�r�X: {Connection}");
            // OutboundBlob�ڑ�������擾
            string outConnection = _conf.GetValue<string>("REQ2380300outbound01");
            // �X�g���[�WAccount�T�[�r�X�쐬
            //BlobServiceClient _outblobServiceClient = new BlobServiceClient(outConnection);
            BlobContainerClient OutcontainerClient = new BlobContainerClient(new Uri(outConnection), new DefaultAzureCredential());
            log.LogInformation($"�擾����outbound�X�g���[�W�A�J�E���g�T�[�r�X: {outConnection}");
            // �X�g���[�WAccount�T�[�r�X�쐬




            try
            {
                //�V�A�W��Փ��̑z��Ŏb��A�W��Ղւ̐ڑ�(��肪�ǉ�)
                // config���g����on/off��؂�ւ���
                log.LogInformation($"�b��A�W��Ձi�V�j�ڑ�������擾");
                string creditConnection = _conf.GetValue<string>("REQ2380300creditout01");
                BlobContainerClient CredtcontainerClient = new BlobContainerClient(new Uri(creditConnection), new DefaultAzureCredential());
                log.LogInformation($"�擾�����b��A�W���outbound�X�g���[�W�A�J�E���g�T�[�r�X: {creditConnection}");
                //string blobtestname = "test.txt";
                //log.LogInformation($"�z��Ώۃt�@�C����: {blobtestname}");
                //var CreditBlob = await CredtcontainerClient.GetBlobClient(blobtestname).OpenReadAsync();
                //log.LogInformation($"�V�A�W��Ղ̎b��A�W���BLOB���t�@�C����{blobtestname}��ǂݍ��ݐ���");
                /*�@�{�ԉғ��̍ۃe�X�g
                //�b��A�W��Րڑ�������擾(�{�ԉғ��̍ۂɗ��p����j�@https://pjpecreditsysoutbounddfs.blob.core.windows.net/credit-system-outbound
                log.LogInformation($"�b��A�W��Րڑ�������擾");
                string creditConnection = _conf.GetValue<string>("REQ2380300creditout01");
                BlobContainerClient CredtcontainerClient = new BlobContainerClient(new Uri(creditConnection), new DefaultAzureCredential());
                log.LogInformation($"�擾�����b��A�W���outbound�X�g���[�W�A�J�E���g�T�[�r�X: {creditConnection}");
                string creditname = "0000000020/primary/nega/S010_0000000020_20231124080053.dat";
                //   string creditname = "0000000020/primary/nega/S010_0000000020_20231124080053.dat";
                log.LogInformation($"�z��t�@�C����{creditname}");
                var CreditBlob = await CredtcontainerClient.GetBlobClient(creditname).OpenReadAsync();
                log.LogInformation($"�b��A�W���BLOB���t�@�C����{creditname}��ǂݍ��ݐ���");
                //var _inBlobContainer = _inblobServiceClient.GetBlobContainerClient("system01");
                //log.LogInformation($"Inbound�R���e�i�擾");
                */

                //�b��A�W��Ճe�X�g���I�������߂��͈�
                // string name = "inbound/HULFT_DENSHI_KINADD.zip";
                log.LogInformation($"�z��Ώۃt�@�C����: {name}");
                // var myBlob = await containerClient.GetBlobClient(name).OpenReadAsync();
                 log.LogInformation($"Inbound�R���e�i�I�[�v���A�t�@�C����{name}");
                // Container�T�[�r�X�쐬
                // var _tempBlobContainer = _blobServiceClient.GetBlobContainerClient("temp");
                //var _tempBlobContainer = _blobServiceClient.GetBlobContainerClient("tmp");
                // Container�̑��ݗL�����m�F���Ė�����΍쐬


                // TODO START �o�^API�ɌĂ�
                ApiEndpointConnection apiEndpointConnection = new ApiEndpointConnection();

                //�J�^���OID�֐��̌Ăяo��
                String [] fileNameParts = name.Split("/");
                String catelogID = CreateCatelogidClass.CreateCatelogidClassAsync(log, fileNameParts[1]);

                //TODO appID�͂܂��킩��Ȃ�
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

                //�o�^API�̏�������
                // �o�^API�ɌĂ�
                String loginContent = await apiEndpointConnection.ApiEndpointConnectionAsync(log, loginUrl, loginJson, httpClientFactory, context);

                log.LogInformation($"C# loginContent: {loginContent}");
                // �o�^API�̃��X�|���X�����
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

                //�𓀏���

                await TempcontainerClient.CreateIfNotExistsAsync();
                log.LogInformation($"tmp�m�F�ρA�𓀏����J�n");
                UzipStreamToBlob uzipStreamToBlob = new UzipStreamToBlob();


                // �ۑ���f�B���N�g���̔���Ȃǂ͕ʓr���{����
                var tempFileNameList = await uzipStreamToBlob.UzipStreamToBlobAsync(log, myBlob, TempcontainerClient, "", loginMyModel.businessData.body.processId);
                //�G�N�Z�v�V�������o��
                log.LogInformation($"�𓀏�������");
                //var outFilePathList = CreateUploadFileNamePath.CreateFilePath(log, tempFileNameList);
                if (tempFileNameList == null) {
                    throw new Exception($"tempFileNameList��NULL�ł� : {tempFileNameList}"); //�Ăяo�����ɗ�O�����
                }

                //�t�@�C���p�X�쐬

                //outbound�ɃA�b�v���[�h               

                var outUpload = new BlobUploadClass();


                String config = Environment.GetEnvironmentVariable("config");

                //�b��A�W��Ռ����t�@�C���p�X�쐬(��肪�V�K�ǉ�)
                log.LogInformation($"�b��A�W��Ռ����t�@�C���p�X�쐬");
                var creditFilePathList = CreateCreditFileNamePath.CreateCreditFilePath(log, tempFileNameList, System.IO.Path.GetFileName(name));
                //�V�A�W��Ռ����t�@�C���p�X�쐬
                log.LogInformation($"�V�A�W��Ռ����t�@�C���p�X�쐬");
                var outFilePathList = CreateUploadFileNamePath.CreateFilePath(log, tempFileNameList, System.IO.Path.GetFileName(name), catelogID);



                //�b��A�W��Ռ����A�b�v���[�h���� outfilepathlist��creditFilePathList�ɕύX����(��肪�V�K�ǉ�)
                // log.LogInformation($"�b��A�W��ՃA�b�v���[�h�����J�n");
                // await outUpload.outBlobUploadAsync(log, TempcontainerClient, CredtcontainerClient, tempFileNameList, creditFilePathList);
                

                
                
                //var outboundContainer = _outblobServiceClient.GetBlobContainerClient("system01");
               
                //�V�A�W��Ռ����A�b�v���[�h����
                log.LogInformation($"�V�A�W��ՃA�b�v���[�h�����J�n");
                await outUpload.outBlobUploadAsync(log, TempcontainerClient, OutcontainerClient, tempFileNameList, outFilePathList, CredtcontainerClient, creditFilePathList);
               
                log.LogInformation($"�����I��");

                // TODO STRAT �X�VAPI�ɌĂ�
                UpdateRequestModel updateRequestModel = new UpdateRequestModel
                {
                    status = "2"
                };
                string updatejsonString = System.Text.Json.JsonSerializer.Serialize(updateRequestModel);

                HttpContent updateJson = new StringContent(updatejsonString, Encoding.UTF8, contentType);                

                var updateUrl = $"{apiURL}/statuses/" + loginMyModel.businessData.body.processId;

                log.LogInformation($"updateUrl: {updateUrl}");

                // ���N�G�X�g���f����JSON�ɕϊ�����
                var updateMyJson = JsonConvert.SerializeObject(updateRequestModel);

                // ���N�G�X�gJSON��StringContent�ɕϊ�����
                var updateMyString = new StringContent(updateMyJson, Encoding.UTF8, "application/json");

                // �X�VAPI�ɌĂ�
                String updateContent = await apiEndpointConnection.ApiEndpointConnectionAsync(log, updateUrl, updateJson, httpClientFactory, context);

                // TODO END



            }
            catch (Exception ex)
            {

                // �G���[���O���o��
                _logger.LogError(ex, "�G���[:�ȉ��̃G���[���b�Z�[�W���Q�Ƃ��Ă�������:{ErrorMessage}", ex.Message);

                //throw; fuctions���ۂ��ƍĎ��s�͂��Ȃ�
            }
            finally
            {
                //��L�Œ�`���ꂽ�����ɊY������ꍇ�A�����N�����Ă����s���鏈���������遨��΂��Ȃ��Ƃ����Ȃ����Ƃ����� eg.��������Ȃ�
                //��������͂����H�@new���āA�����������Ȃ��ꍇ������Bnew�����I�u�W�F�N�g���J�����铙�̖��߂��悭�g��
                GC.Collect(); //��������������@�������~�܂�ꍇ������̂Ŋm���ɊJ���������ꍇ�����Ɏg���i�e���̂Ȃ��ꍇ�̂ݎg�p�j
                log.LogInformation($"FUNCTION����������");
            }

        }
    }
}