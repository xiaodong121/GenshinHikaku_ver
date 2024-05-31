using Microsoft.VisualStudio.TestTools.UnitTesting;
using REQ2380300Func.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // ILoggerの名前空間を追加する
using Moq;

namespace REQ2380300Func.lib.Tests
{
    [TestClass()]
    public class CreateUploadFileNamePathTests
    {
        [TestMethod()] //正常系　S010から送信されるデータ
        public void CreateFilePathTest()
        {
            // ILoggerのモックを作成
            var loggerMock = new Mock<ILogger>();

            IReadOnlyList<string> files = new List<string>() { "123_S010_0000000200_20220718123043.dat", "123_S010_0000000250_20220718123022.dat" };

            // ILoggerモックをCreateFilePathメソッドに渡して呼び出す
            var outFilePathList = CreateUploadFileNamePath.CreateFilePath(loggerMock.Object, files, "null");

            if (outFilePathList.Count != 4)
            {
                Assert.Fail();
            }
        }


        [TestMethod()] //異常系 VM番号がない場合
        public void CreateFilePathTest2()　
        {
            // ILoggerのモックを作成
            var loggerMock = new Mock<ILogger>();

            IReadOnlyList<string> files = new List<string>() { "S099_0000000200_20220718123043.dat", "S010_0000000250_20220718123022.dat" };

            // ILoggerモックをCreateFilePathメソッドに渡して呼び出す
            var outFilePathList = CreateUploadFileNamePath.CreateFilePath(loggerMock.Object, files,"null");

            if (outFilePathList.Count == 4)　//4だったらFAIL
            {
                Assert.Fail(); 
            }
        }

        [TestMethod()] //正常系　
        public void CreateFilePathTest3()
        {
            // ILoggerのモックを作成
            var loggerMock = new Mock<ILogger>();

            IReadOnlyList<string> files = new List<string>() { "S010_0000000250_20220718123022.dat" };

            // ILoggerモックをCreateFilePathメソッドに渡して呼び出す
            var outFilePathList = CreateUploadFileNamePath.CreateFilePath(loggerMock.Object, files, "null");

            if (outFilePathList.Count != 2)　//4だったらFAIL
            {
                Assert.Fail();
            }
        }

    }
}
