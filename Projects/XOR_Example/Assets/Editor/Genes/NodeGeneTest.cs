using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class NodeGeneTest
{

    private NodeGene node1;

    [SetUp]
    public void SetUp()
    {
        node1 = new NodeGene(5, NodeGeneType.HIDDEN, 0.5f);
    }

    [Test]
    public void CopyNodeGene_Test()
    {
        NodeGene copiedNode = new NodeGene(node1);

        //Test that the copied object is not the same one
        Assert.AreNotEqual(copiedNode, node1);

        //Test the fields
        Assert.AreEqual(node1.ID, copiedNode.ID);
        Assert.AreEqual(node1.Type, copiedNode.Type);

    }

    [Test]
    public void CalculateValue_Test()
    {
        //The nodes
        Dictionary<int, NodeGene> nodesInGenome = new Dictionary<int, NodeGene>();
        NodeGene inputNode1 = new NodeGene(6, NodeGeneType.HIDDEN, 0.5f);
        NodeGene inputNode2 = new NodeGene(7, NodeGeneType.INPUT, 0f);
        NodeGene inputNode3 = new NodeGene(8, NodeGeneType.HIDDEN, 0.5f);
        nodesInGenome.Add(6, inputNode1);
        nodesInGenome.Add(7, inputNode2);
        nodesInGenome.Add(8, inputNode3);

        inputNode1.SetCurrentVal(10);
        inputNode2.SetCurrentVal(20);
        inputNode3.SetCurrentVal(-18);

        //The connections
        List<ConnectionGene> connections = new List<ConnectionGene>();
        connections.Add(new ConnectionGene(6, 5, 1, true, 1));
        connections.Add(new ConnectionGene(7, 5, 0.5, false, 2));
        connections.Add(new ConnectionGene(8, 5, 0.5, true, 3));

        //The stack
        Stack<int> nodeStack = new Stack<int>();

        //Set the connections
        node1.Inputs = connections;

        double result1 = node1.CalculateValue(nodesInGenome, nodeStack);
        Assert.AreEqual(0.7310, result1, 0.0001);
        Assert.AreEqual(true, node1.CurrentValCalculatedFlag);
        Assert.AreEqual(0, nodeStack.Count);

        //Check if the last value will be returned
        nodeStack.Push(5);
        double result2 = node1.CalculateValue(nodesInGenome, nodeStack);
        Assert.AreEqual(0, result2);
        Assert.AreEqual(true, node1.CurrentValCalculatedFlag);
        Assert.AreEqual(1, nodeStack.Count);
    }

    [Test]
    public void ResetNode()
    {
        node1.Inputs = new List<ConnectionGene>();

        Assert.AreEqual(false, node1.CurrentValCalculatedFlag);
        node1.CalculateValue(new Dictionary<int, NodeGene>(), new Stack<int>());

        //Test if flag was set to true
        Assert.AreEqual(true, node1.CurrentValCalculatedFlag);

        //Reset flag
        node1.ResetNode();
        Assert.AreEqual(false, node1.CurrentValCalculatedFlag);
    }
}
