using System;
using System.Collections.Generic;
using System.IO;
using Genealogy.Graph;
using Newtonsoft.Json;
using UnityEngine;

namespace Genealogy.Persistence
{
    public class ScrollStenographer : IGenealogyGraphListener
    {
        private readonly JsonSerializer serializer;
        private JsonTextWriter writer;
        private string writerPath;

        public ScrollStenographer()
        {
            serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new NodeAsGuidContract()
            };

            StartScroll();
        }

        public void OnTransactionComplete(GenealogyGraph genealogyGraph, Node node, List<Relation> relations)
        {
            lock (writer)
            {
                if (relations.Count == 0)
                {
                    writer.WritePropertyName("rootEntry");
                    serializer.Serialize(writer, new GenealogyScrollRootEntry(node));

                    writer.WritePropertyName("entries");
                    writer.WriteStartArray();
                }
                else
                {
                    serializer.Serialize(writer, new GenealogyScrollEntry(node, relations));
                }
            }
        }

        public void OnClear()
        {
            CloseScroll();
            StartScroll();
        }

        private void StartScroll()
        {
            if (writer != null)
                throw new InvalidOperationException("Cannot start a scroll that is already open");
            var saveDir = $"{Application.temporaryCachePath}/ScrollStenographer";
            Directory.CreateDirectory(saveDir);
            writerPath = $"{saveDir}/scroll1.json";
            writer = new JsonTextWriter(new StreamWriter(writerPath));
            lock (writer)
                writer.WriteStartObject();
        }

        public void SaveCopy(string filePath)
        {
            lock (writer)
            {
                writer.Flush();
                Directory.GetParent(filePath).Create();
                File.Copy(writerPath, filePath, true);
                using (var copyWriter = new StreamWriter(filePath, true))
                {
                    if (writer.Path == "" && writer.WriteState == WriteState.Object)
                        copyWriter.Write("}");
                    else if (writer.WriteState == WriteState.Array)
                        copyWriter.Write($"]{System.Environment.NewLine}}}");
                    else
                        throw new InvalidOperationException(
                            $"Unable to close scroll from path '{writer.Path}' and state '{writer.WriteState}'");
                }
            }
        }

        public void CloseScroll()
        {
            lock (writer)
            {
                writer.Close();
                writer = null;
            }
        }
    }
}