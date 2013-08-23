using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using KTF.Proxy.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KTF.Proxy.Test
{
    [TestClass]
    public class StorageTest
    {
        List<WebProxy> list = new List<WebProxy>() { 
            new WebProxy("118.97.95.174:80"), 
            new WebProxy("9005.hostingsharedbox.com:8089"),
            null
        };

        const string customPath = "pr.txt";

        [TestMethod]
        public void Save()
        {
            FileStorage storage = new FileStorage();
            storage.SaveToFile(list);
            Assert.IsTrue(System.IO.File.Exists(FileStorage.DefaultFilename));
        }

        [TestMethod]
        public void SaveWithCustomName()
        {
            FileStorage storage = new FileStorage(customPath);
            storage.SaveToFile(list);
            Assert.IsTrue(System.IO.File.Exists(customPath));
        }

        [TestMethod]
        public void Load()
        {
            FileStorage storage = new FileStorage();
            storage.SaveToFile(list);
            Assert.AreEqual(2, storage.LoadFromFile().Count());
            System.IO.File.Delete(FileStorage.DefaultFilename);
        }

        [TestMethod]
        public void LoadWithCustomName()
        {
            FileStorage storage = new FileStorage(customPath);
            storage.SaveToFile(list);
            Assert.AreEqual(2, storage.LoadFromFile().Count());
            System.IO.File.Delete(customPath);
        }

        [ClassCleanup]
        public static void Clean()
        {
            if (System.IO.File.Exists(FileStorage.DefaultFilename))
                System.IO.File.Delete(FileStorage.DefaultFilename);

            if (System.IO.File.Exists(customPath))
                System.IO.File.Delete(customPath);
        }
    }
}
