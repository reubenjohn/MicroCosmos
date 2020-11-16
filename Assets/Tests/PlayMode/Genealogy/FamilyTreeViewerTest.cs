using System.Collections;
using System.Linq;
using Genealogy;
using Genealogy.AsexualFamilyTree;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Genealogy
{
    public class FamilyTreeViewerTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Tests/PlayMode/Genealogy/FamilyTreeViewerTestScene");
        }

        [UnityTest]
        public IEnumerator FamilyTreeViewerTestSimplePasses()
        {
            var tree = new FamilyTree();
            var layoutManager = new LayoutManager();
            tree.AddListener(layoutManager);
            var viewer = GameObject.Find("Family Tree Content").GetComponent<FamilyTreeViewer>();
            layoutManager.AddListener(viewer);

            var rootNode = new CellNode();
            tree.RegisterRootNode(rootNode);
            var cell0Node = new CellNode(); // {CellObj = GameObject.Find("Cell1[0]")};
            tree.RegisterReproduction(new Node[] {rootNode}, cell0Node);
            var cell1Node = new CellNode(); // {CellObj = GameObject.Find("Cell1[1]")};
            tree.RegisterReproduction(new Node[] {rootNode}, cell1Node);

            Assert.AreEqual(new Vector3(60, 20, 0), LocalPositionOf(rootNode));
            
            var rep0 = AsexualReproductionNodeOf(tree, cell0Node);
            Assert.AreEqual(new Vector3(30, -20, 0), LocalPositionOf(rep0));
            Assert.AreEqual(new Vector3(30, -60, 0), LocalPositionOf(cell0Node));

            var rep1 = AsexualReproductionNodeOf(tree, cell1Node);
            Assert.AreEqual(new Vector3(90, -20, 0), LocalPositionOf(rep1));
            Assert.AreEqual(new Vector3(90, -60, 0), LocalPositionOf(cell1Node));

            yield return null;
        }

        private static Vector3 LocalPositionOf(Node node) =>
            GameObject.Find(node.ToString()).GetComponent<RectTransform>().localPosition;
        
        private static Node AsexualReproductionNodeOf(FamilyTree tree, CellNode node) =>
            tree.GetRelationsTo(node.Guid)[0].From;
    }
}