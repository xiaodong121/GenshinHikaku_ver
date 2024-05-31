using Azure.Storage.Blobs; //内結で動作を確認する
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using REQ2380300Func.lib;
using REQ2380300Func.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REQ2380300Func.lib.Tests
{
/*    [TestClass()]
    public class BlobUploadClassTests
    {
        [TestMethod()]
        public async Task OutBlobUploadAsyncTest()
        {
            // ILogger のモックを作成
            var loggerMock = new Mock<ILogger>();
            // BlobContainerClient のモックを作成
            var tempBlobMock = new Mock<BlobContainerClient>();
            var outBlobClientMock = new Mock<BlobContainerClient>();

            var outUpload = new BlobUploadClass();

            // テスト用のデータをモックとして設定 正常なファイル名×２
            var tempFileNameList = new List<string> { "123_S01_0000000000_20001231.dat", "123_S01_0000000001_20001231.dat" }; // 一時保管BLOBに格納されたファイル定義
            //正常なファイルパスを作成                                                                                                                  // テスト用のデータをモックとして設定 正常なファイル名×２
            var outFilePathList = new IReadOnlyList<ListFileNameModel>{ "/HULFT_OWNER_NEGA_MHTA/0000000000/nega/", "/HULFT_OWNER_NEGA_MHTA/0000000001/nega/" }; //かくのうさきでぃれくとり

            // outFilePathList のモックを作成
            //  var outFilePathListMock = new Mock<List<ListFileNameModel>>();
            // outFilePathListMock.Setup(mock => mock.Count).Returns(2); // 格納先のファイルパスも生で書いてしまってOK　モックが必要ないなら使わない

            // モックを使用してテストデータを設定
            //outFilePathListMock.Setup(mock => mock[0]).Returns(new ListFileNameModel
            //{
            //    newFileName = "123_20001231.dat",
            //    sauceFileName = "123_S01_0000000000_20001231.dat"
            //});
            //outFilePathListMock.Setup(mock => mock[1]).Returns(new ListFileNameModel
            //{
            //    newFileName = "123_20001231.dat",
            //    sauceFileName = "123_S01_0000000001_20001231.dat"
            //});
            //var outFilePathList = outFilePathListMock.Object; // モックを実際のオブジェクトに変換

            // モックデータを関数に渡してテスト
            await outUpload.outBlobUploadAsync(loggerMock.Object, tempBlobMock.Object, outBlobClientMock.Object, tempFileNameList, outFilePathList);

            // テスト用のアサーションを追加して、テストの成功または失敗を判定
            // アップロードが成功したかの結果を取りたいけど、この形式では取れない（やりたいけどわからない）
                // outBlobClientMock.Verify(mock => mock.(It.IsAny<Stream>(), true), Times.Exactly(tempFileNameList.Count));

            // ロギングに成功のメッセージが出力されるかどうかを確認
            loggerMock.Verify(mock => mock.LogInformation(" アップロード正常終了"), Times.AtLeastOnce());

        }
    }*/
}
