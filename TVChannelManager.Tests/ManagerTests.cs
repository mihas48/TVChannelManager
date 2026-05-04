using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Channels;
using TVChannelManager.Library;
using TVChannelManager.Library.Models;
using TVChannelManager.Library.Services;
using Xunit;

namespace TVChannelManager.Tests
{
    public class DataManagerTests
    {
        [Fact]
        public void Add_ValidChannel_ShouldIncreaseCount()
        {
            var mgr = new DataManager();
            mgr.Add(new TVChannel { Name = "Test", Rating = 50, MedianViewersAge = 30 });
            Assert.Single(mgr.GetData());
        }

        [Fact]
        public void Add_InvalidRating_ShouldThrowArgumentException()
        {
            var mgr = new DataManager();
            var ch = new TVChannel { Name = "Bad", Rating = 150 };
            Assert.Throws<ArgumentException>(() => mgr.Add(ch));
        }

        [Fact]
        public void Remove_ExistingIndex_ShouldDecreaseCount()
        {
            var mgr = new DataManager();
            mgr.Add(new TVChannel { Name = "One" });
            mgr.Add(new TVChannel { Name = "Two" });
            mgr.Remove(1);
            Assert.Single(mgr.GetData());
            Assert.Equal("Two", mgr.GetData()[0].Name);
        }

        [Fact]
        public void Remove_InvalidIndex_ShouldThrowArgumentException()
        {
            var mgr = new DataManager();
            mgr.Add(new TVChannel { Name = "One" });
            Assert.Throws<ArgumentException>(() => mgr.Remove(5));
        }

        [Fact]
        public void Update_ValidData_ShouldChangeChannel()
        {
            var mgr = new DataManager();
            mgr.Add(new TVChannel { Name = "Old", Rating = 10 });
            mgr.Update(1, new TVChannel { Name = "New", Rating = 80 });
            var ch = mgr.GetData()[0];
            Assert.Equal("New", ch.Name);
            Assert.Equal(80, ch.Rating);
        }

        [Fact]
        public void FileManager_SaveAndLoad_ShouldPreserveData()
        {
            var list = new List<TVChannel>
            {
                new TVChannel { Name = "Temp", Rating = 42 }
            };
            string tmp = Path.GetTempFileName() + ".json";
            try
            {
                FileManager.SaveData(list, tmp);
                var loaded = FileManager.LoadData(tmp);
                Assert.Single(loaded);
                Assert.Equal("Temp", loaded[0].Name);
            }
            finally
            {
                if (File.Exists(tmp)) File.Delete(tmp);
            }
        }
    }
}