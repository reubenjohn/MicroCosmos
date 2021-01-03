using System.Collections;
using Genealogy.Graph;
using Genealogy.Layout.Asexual;
using Genealogy.Visualization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Genealogy
{
    public class GenealogyGraphViewerTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Tests/PlayMode/Genealogy/GenealogyGraphViewerTestScene");
        }

        [UnityTest]
        public IEnumerator NodesRenderInCorrectPositions()
        {
            var tree = new GenealogyGraph();
            var layoutManager = new LayoutManager {LiveLayoutEnabled = true};
            tree.AddListener(layoutManager);
            var viewer = GameObject.Find("_Genealogy Graph Canvas").GetComponent<GenealogyGraphViewer>();
            layoutManager.AddListener(viewer);

            var rootNode = new CellNode();
            tree.RegisterRootNode(rootNode);
            var cell0Node = new CellNode(); // {CellObj = GameObject.Find("Cell1[0]")};
            tree.RegisterReproductionAndOffspring(new Node[] {rootNode}, cell0Node);
            var cell1Node = new CellNode(); // {CellObj = GameObject.Find("Cell1[1]")};
            tree.RegisterReproductionAndOffspring(new Node[] {rootNode}, cell1Node);
            layoutManager.RecalculateLayout();

            Assert.AreEqual(new Vector3(60, -10, 0), LocalPositionOf(rootNode));

            var rep0 = AsexualReproductionNodeOf(tree, cell0Node);
            Assert.AreEqual(new Vector3(30, -30, 0), LocalPositionOf(rep0));
            Assert.AreEqual(new Vector3(30, -50, 0), LocalPositionOf(cell0Node));

            var rep1 = AsexualReproductionNodeOf(tree, cell1Node);
            Assert.AreEqual(new Vector3(90, -30, 0), LocalPositionOf(rep1));
            Assert.AreEqual(new Vector3(90, -50, 0), LocalPositionOf(cell1Node));

            yield return null;
        }

        private static Vector3 LocalPositionOf(Node node) =>
            GameObject.Find(node.ToString()).GetComponent<RectTransform>().localPosition;

        private static Node AsexualReproductionNodeOf(GenealogyGraph tree, CellNode node) =>
            tree.GetRelationsTo(node.Guid)[0].From;
    }
}