﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Genealogy
{
    public class LayoutNode
    {
        public Node Node { get; }
        public LayoutNode Parent { get; }

        private readonly List<LayoutNode> children;
        private Rect bounds;

        public LayoutNode(Node node, [CanBeNull] LayoutNode parent)
        {
            Node = node;
            Parent = parent;
            children = new List<LayoutNode>();
            if (parent != null)
            {
                bounds = parent.NextChildBounds();
                parent.AddChild(this);
            }
            else
            {
                bounds = new Rect(0, 0, 1, 1);
            }
        }

        public int Generation => Mathf.RoundToInt(-bounds.yMin);

        public void AddChild(LayoutNode child)
        {
            XMax = Mathf.Max(bounds.xMax, child.bounds.xMax);
            children.Add(child);
        }

        public float XMax
        {
            get => bounds.xMax;
            private set
            {
                if (!Mathf.Approximately(bounds.xMax, value))
                {
                    bounds.xMax = value;

                    if (Parent == null) return;

                    var siblingIndex = SiblingIndex;
                    if (siblingIndex + 1 < Parent.children.Count)
                        Parent.children[siblingIndex + 1].X = XMax;

                    Parent.XMax = Mathf.Max(Parent.XMax, Parent.children.Last().XMax);
                }
            }
        }

        public int SiblingIndex => Parent?.children.IndexOf(this) ?? 0;

        [CanBeNull]
        private LayoutNode NearestRelationToRight()
        {
            if (Parent == null) return null;

            var myIndex = SiblingIndex;
            if (Parent.children.Count > myIndex + 1) return Parent.children[myIndex + 1];
            else return Parent.NearestRelationToRight();
        }

        public float X
        {
            get => bounds.x;
            private set
            {
                var diff = value - bounds.x;
                if (Mathf.Approximately(diff, 0)) return;
                bounds.x = value;
                foreach (var child in children)
                    child.X += diff;
            }
        }

        public Vector2 Center => bounds.center;

        private Rect NextChildBounds()
        {
            var lastChild = children.LastOrDefault();
            if (lastChild != null)
                return new Rect(lastChild.bounds.xMin + 1, lastChild.bounds.yMin, 1, 1);
            else
                return new Rect(bounds.xMin, bounds.yMin - 1, 1, 1);
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
            var stringWriter = new StringWriter() {NewLine = newLine};
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
    }
}