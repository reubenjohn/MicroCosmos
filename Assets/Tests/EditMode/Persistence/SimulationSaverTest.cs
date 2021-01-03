using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Persistence;
using UnityEngine;
using Util;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests.EditMode.Persistence
{
    public class SimulationSaverTest
    {
        private string saveDir;

        [SetUp]
        public void Setup()
        {
            saveDir = $"{Application.temporaryCachePath}/testing/{nameof(SimulationSaverTest)}";
            try
            {
                Directory.Delete(saveDir, true);
            }
            catch (DirectoryNotFoundException) { }
        }

        [Test]
        public void TestSaveDirectoryPath()
        {
            var saver = new SubsystemsPersistence(new ISavableSubsystem[0], saveDir);
            Assert.AreEqual(saveDir, saver.SaveDirectory);
            saver.SaveDirectory = saveDir + "2";
            Assert.AreEqual(saveDir + "2", saver.SaveDirectory);
        }

        [Test]
        public void TestEmptyDirectoryCreation()
        {
            var saver = new SubsystemsPersistence(new ISavableSubsystem[0], saveDir);
            saver.Save();
            Assert.IsTrue(Directory.Exists(saveDir));
            Assert.AreEqual(0, Directory.GetFiles(saveDir).Length);
        }

        [Test]
        public void TestPersistence()
        {
            var savable = new TestSavableSubsystem();
            var saver = new SubsystemsPersistence(new ISavableSubsystem[] {savable}, saveDir);

            var json = @"[{""x"":1.0,""y"":2.0},{""x"":3.0,""y"":4.0}]";

            saver.Save();
            Assert.IsTrue(Directory.Exists(saveDir));
            Assert.AreEqual(1, Directory.GetFiles(saveDir).Length);
            Assert.AreEqual($"{saveDir}\\abc-3.json", Directory.GetFiles(saveDir)[0]);
            Assert.AreEqual(json, Serialization.ReadAllCompressedText($"{saveDir}/abc-3.json"));

            saver.Load();
            Assert.AreEqual(json, JsonConvert.SerializeObject(savable.loaded));
        }
    }

    public class TestSavableSubsystem : ISavableSubsystem<Dictionary<string, float>>
    {
        public Dictionary<string, float>[] loaded { get; private set; }

        public string GetID() => "abc";

        public int GetPersistenceVersion() => 3;

        public Type GetSavableType() => typeof(Dictionary<string, float>);
        IEnumerable ISavableSubsystem.Save() => Save();

        public IEnumerable<Dictionary<string, float>> Save() =>
            new[]
            {
                new Dictionary<string, float> {{"x", 1f}, {"y", 2f}},
                new Dictionary<string, float> {{"x", 3f}, {"y", 4f}}
            };

        public void Load(IEnumerable save) => Load(save.Cast<Dictionary<string, float>>());
        public JsonSerializer GetSerializer() => new JsonSerializer();

        public void Load(IEnumerable<Dictionary<string, float>> save) => loaded = save.ToArray();
    }
}