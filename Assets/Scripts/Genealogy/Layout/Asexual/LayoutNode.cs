using System;
using System.Collections.Generic;
using System.IO;
using Genealogy.Graph;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Genealogy.Layout.Asexual
{
    public class LayoutNode
    {
        [JsonIgnore] public readonly List<LayoutNode> children;

        private readonly List<ILayoutChangeListener<LayoutNode>> listeners;

        public LayoutNode(List<ILayoutChangeListener<LayoutNode>> layoutChangeListeners,
            Node node, [CanBeNull] LayoutNode parent)
        {
            listeners = layoutChangeListeners;
            Node = node;
            Parent = parent;
            children = new List<LayoutNode>();
            if (parent != null)
            {
                Generation = parent.Generation + 1;
                parent.AddChild(this);
            }
            else
            {
                Generation = 0;
                NotifyListenersOfCreate();
            }
        }

        public Node Node { get; }
        public LayoutNode Parent { get; }

        public int Generation { get; }

        private int SiblingIndex => Parent?.children.IndexOf(this) ?? 0;

        public Vector2 Center { get; private set; } = Vector2.zero;

        private void AddChild(LayoutNode child)
        {
            children.Add(child);
            child.NotifyListenersOfCreate();
        }

        public void Remove()
        {
            if (Parent != null && !Parent.children.Remove(this))
                throw new ArgumentException($"'{Node}' is not a child of {Parent.Node}");
            NotifyListenersOfRemove();
        }

        public void RecalculateLayout() => RecalculateLayout(Vector2.zero);

        private Vector2 RecalculateLayout(Vector2 topLeft)
        {
            var nextChildTopRight = topLeft + Vector2.down;
            foreach (var child in children)
                nextChildTopRight = child.RecalculateLayout(nextChildTopRight);

            var topRight = new Vector2(Mathf.Max(topLeft.x + 1, nextChildTopRight.x), topLeft.y);
            Center = new Vector2((topLeft.x + topRight.x) / 2f, topLeft.y - .5f);
            NotifyListenersOfUpdate();
            return topRight;
        }

        public IEnumerable<LayoutNode> NodesOfGeneration(int n)
        {
            var myGen = Generation;
            if (myGen == n)
                yield return this;
            else if (myGen < n)
                foreach (var child in children)
                foreach (var node in child.NodesOfGeneration(n))
                    yield return node;
        }

        public string GetHierarchyDisplayString(string newLine = "\r\n", short nodeWidth = 3)
        {
            var stringWriter = new StringWriter {NewLine = newLine};
            PrintHierarchyTo(stringWriter, nodeWidth);
            return stringWriter.ToString();
        }

        public void PrintHierarchyTo(StringWriter sw, short nodeWidth = 3)
        {
            var nodeSep = nodeWidth * 2;
            var format = $"D{nodeWidth}";
            var currGen = Generation;
            while (true)
            {
                var cursorPos = 0;
                var enumerator = NodesOfGeneration(currGen).GetEnumerator();
                if (!enumerator.MoveNext() || enumerator.Current == null)
                {
                    enumerator.Dispose();
                    return;
                }

                sw.WriteLine();

                do
                {
                    if (enumerator.Current == null) break;
                    var currentSiblingIndex = enumerator.Current.SiblingIndex.ToString(format);
                    // var currentSiblingIndex = (enumerator.Current.Center.x * nodeSep).ToString("0.0");
                    var offset =
                        Mathf.RoundToInt(enumerator.Current.Center.x * nodeSep - nodeWidth + .1f)
                        - cursorPos;
                    sw.Write(new string(' ', offset));
                    sw.Write(currentSiblingIndex);
                    cursorPos += offset + currentSiblingIndex.Length;
                } while (enumerator.MoveNext());

                enumerator.Dispose();

                currGen++;
            }
        }

        private void NotifyListenersOfUpdate()
        {
            foreach (var listener in listeners)
                listener.OnUpdateNode(this);
        }

        private void NotifyListenersOfCreate()
        {
            foreach (var listener in listeners)
                listener.OnAddNode(this);
        }

        private void NotifyListenersOfRemove()
        {
            foreach (var listener in listeners)
                listener.OnRemoveNode(this);
        }
    }
}