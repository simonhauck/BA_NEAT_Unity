using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class MutationLogTest
{

    private MutationLog mutationLog;

    private ConnectionGene connection1;
    private ConnectionGene connection2;

    private NodeGene node1;
    private NodeGene node2;

    [SetUp]
    public void SetUp()
    {
        mutationLog = new MutationLog();

        node1 = new NodeGene(1, NodeGeneType.HIDDEN, 0.5f);
        node2 = new NodeGene(2, NodeGeneType.OUTPUT, 1f);

        connection1 = new ConnectionGene(1, 2, 0.5f, true, 1);
        connection2 = new ConnectionGene(2, 1, 1f, false, 2);

        mutationLog.AddConnectionMutation(connection1);
        mutationLog.AddNodeMutation(connection1, node1);

    }

    [Test]
    public void AddConnectionMutation_Test()
    {
        Assert.AreEqual(1, mutationLog.ConnectionMutations.Count);

        mutationLog.AddConnectionMutation(connection2);
        Assert.AreEqual(2, mutationLog.ConnectionMutations.Count);

        //The expected key
        MutationLog.ConnectionMutationKey key = new MutationLog.ConnectionMutationKey(2, 1);
        Assert.True(mutationLog.ConnectionMutations.ContainsKey(key));
        Assert.AreEqual(2, mutationLog.ConnectionMutations[key]);
    }

    [Test]
    public void AddConnectionMutation2_Test()
    {
        Assert.AreEqual(1, mutationLog.ConnectionMutations.Count);

        mutationLog.AddConnectionMutation(connection2.InNode, connection2.OutNode, connection2.InnovationNumber);
        Assert.AreEqual(2, mutationLog.ConnectionMutations.Count);

        //The expected key
        MutationLog.ConnectionMutationKey key = new MutationLog.ConnectionMutationKey(2, 1);
        Assert.True(mutationLog.ConnectionMutations.ContainsKey(key));
        Assert.AreEqual(2, mutationLog.ConnectionMutations[key]);
    }

    [Test]
    public void AddNoteMutation_Test()
    {
        Assert.AreEqual(1, mutationLog.NodeMutations.Count);
        mutationLog.AddNodeMutation(connection2, node2);

        Assert.AreEqual(2, mutationLog.NodeMutations.Count);
        Assert.True(mutationLog.NodeMutations.ContainsKey(2));
        Assert.AreEqual(2, mutationLog.NodeMutations[2][0]);
    }

    [Test]
    public void AddNodeMutation2_Test()
    {
        Assert.AreEqual(1, mutationLog.NodeMutations.Count);
        mutationLog.AddNodeMutation(connection2.InnovationNumber, node2.ID);

        Assert.AreEqual(2, mutationLog.NodeMutations.Count);
        Assert.True(mutationLog.NodeMutations.ContainsKey(2));
        Assert.AreEqual(2, mutationLog.NodeMutations[2][0]);

        //Add a second node
        mutationLog.AddNodeMutation(connection2.InNode, 5);

        Assert.AreEqual(2, mutationLog.NodeMutations.Count);
        Assert.True(mutationLog.NodeMutations.ContainsKey(2));
        Assert.AreEqual(2, mutationLog.NodeMutations[2][0]);
        Assert.AreEqual(5, mutationLog.NodeMutations[2][1]);

    }

    /*

    [Test]
    public void GetNodeID_Test()
    {
        GeneCounter nodeCounter = new GeneCounter(10);

        Assert.AreEqual(1, mutationLog.NodeMutations.Count);

        //Insert existing node
        int existingNode = mutationLog.GetNodeID(1, nodeCounter);

        //Check the amount after getting the existing node
        Assert.AreEqual(1, mutationLog.NodeMutations.Count);
        Assert.AreEqual(1, existingNode);

        //Insert new node
        int newNode = mutationLog.GetNodeID(2, nodeCounter);

        //Check the amount after getting the new node
        Assert.AreEqual(2, mutationLog.NodeMutations.Count);
        Assert.AreEqual(10, newNode);
    }

    */

    [Test]
    public void GetNodeID_Test()
    {
        GeneCounter nodeCounter = new GeneCounter(10);
        List<int> nodesInGenome = new List<int>();

        //Get an existing node
        int result = mutationLog.GetNodeID(connection1.InnovationNumber, nodesInGenome, nodeCounter);
        Assert.AreEqual(node1.ID, result);

        //Get a new node number
        nodesInGenome.Add(result);
        int result2 = mutationLog.GetNodeID(connection1.InnovationNumber, nodesInGenome, nodeCounter);
        Assert.AreEqual(10, result2);
        Assert.AreEqual(2, mutationLog.NodeMutations[connection1.InnovationNumber].Count);

        //Get the second node again
        int result3 = mutationLog.GetNodeID(connection1.InnovationNumber, nodesInGenome, nodeCounter);
        Assert.AreEqual(10, result3);
        Assert.AreEqual(2, mutationLog.NodeMutations[connection1.InnovationNumber].Count);

    }

    [Test]
    public void IsNodeExisting_Test()
    {
        //Test not existing node
        Assert.AreEqual(0, mutationLog.IsNodeExisting(connection2).Count);

        //Test existing node
        Assert.AreEqual(1, mutationLog.IsNodeExisting(connection1).Count);
        Assert.AreEqual(1, mutationLog.IsNodeExisting(connection1)[0]);
    }

    [Test]
    public void IsNodeExisting2_Test()
    {
        //Test not existing node
        Assert.AreEqual(0, mutationLog.IsNodeExisting(connection2.InnovationNumber).Count);

        //Test existing node
        Assert.AreEqual(1, mutationLog.IsNodeExisting(connection1.InnovationNumber).Count);
        Assert.AreEqual(1, mutationLog.IsNodeExisting(connection1.InnovationNumber)[0]);
    }

    [Test]
    public void IsConnectionExisting_Test()
    {
        //Test not existing connection
        Assert.AreEqual(-1, mutationLog.IsConnectionExisting(new NodeGene(5, NodeGeneType.HIDDEN, 0.5f), new NodeGene(4, NodeGeneType.OUTPUT, 1f)));

        //Test existing connection
        Assert.AreEqual(1, mutationLog.IsConnectionExisting(new NodeGene(1, NodeGeneType.HIDDEN, 0.5f), new NodeGene(2, NodeGeneType.OUTPUT, 1f)));
    }

    [Test]
    public void IsConnectionExisting2_Test()
    {
        //Test not existing connection
        Assert.AreEqual(-1, mutationLog.IsConnectionExisting(5, 5));

        //Test existing connection
        Assert.AreEqual(1, mutationLog.IsConnectionExisting(1, 2));
    }

    [Test]
    public void GetConnectionInnovationNumber_Test()
    {
        GeneCounter connectionCounter = new GeneCounter(10);

        //Check the amount before
        Assert.AreEqual(1, mutationLog.ConnectionMutations.Count);

        //Insert an existing connection
        int existingConnection = mutationLog.GetConnectionInnovationNumber(1, 2, connectionCounter);

        //Check the amount after getting the existing connection
        Assert.AreEqual(1, mutationLog.ConnectionMutations.Count);
        Assert.AreEqual(1, existingConnection);

        //Insert new connection
        int newConnection = mutationLog.GetConnectionInnovationNumber(2, 1, connectionCounter);

        //Check the amount after the inserting the new connection
        Assert.AreEqual(2, mutationLog.ConnectionMutations.Count);
        Assert.AreEqual(10, newConnection);

        //Insert new connection again
        newConnection = mutationLog.GetConnectionInnovationNumber(2, 1, connectionCounter);

        //Check the amount after the inserting the new connection
        Assert.AreEqual(2, mutationLog.ConnectionMutations.Count);
        Assert.AreEqual(10, newConnection);
    }

}
