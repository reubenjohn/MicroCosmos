using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Genealogy.Persistence
{
    public class ScrollStenographer : IGenealogyGraphListener
    {
        private readonly Func<JsonTextWriter> writerSupplier;
        private JsonTextWriter writer;
        private readonly JsonSerializer serializer;

        private bool isScrollStarted;
        private bool isScrollEntriesStarted;

        public ScrollStenographer(Func<JsonTextWriter> writerSupplier)
        {
            this.writerSupplier = writerSupplier;
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
            if (relations.Count == 0)
            {
                writer.WritePropertyName("rootEntry");
                serializer.Serialize(writer, new GenealogyScrollRootEntry(node));

                writer.WritePropertyName("entries");
                writer.WriteStartArray();
            }
            else
            {
                isScrollEntriesStarted = true;
                serializer.Serialize(writer, new GenealogyScrollEntry(node, relations));
            }
        }

        private void StartScroll()
        {
            writer = writerSupplier.Invoke();
            writer.WriteStartObject();
            isScrollStarted = true;
        }

        public void CloseScroll()
        {
            if (isScrollStarted)
            {
                if (isScrollEntriesStarted)
                {
                    writer.WriteEndArray();
                    isScrollEntriesStarted = false;
                }

                writer.WriteEndObject();
                writer.Close();
                isScrollStarted = false;
            }
        }

        public void OnClear()
        {
            CloseScroll();
            StartScroll();
        }
    }
}