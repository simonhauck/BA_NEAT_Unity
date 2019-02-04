using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class GenomeTest
{

    private Genome parent1Genome;
    private Genome parent2Genome;

    //A feed forward network with 2 inputs and 2 outputs
    //Exmaple values s.h. test:
    private Genome feedForwardNetwork;

    //Recurrent network with 2 inputs and 2 outputs
    private Genome recurrentNetwork;

    [SetUp]
    public void SetUp()
    {

        //-----------------------------------------------------------------------------------
        // Parent 1
        //-----------------------------------------------------------------------------------

        parent1Genome = new Genome();

        //Create the input nodes
        parent1Genome.AddNodeGene(new NodeGene(1, NodeGeneType.INPUT, 0f));
        parent1Genome.AddNodeGene(new NodeGene(2, NodeGeneType.INPUT, 0f));
        parent1Genome.AddNodeGene(new NodeGene(3, NodeGeneType.INPUT, 0f));

        //Create the output node
        parent1Genome.AddNodeGene(new NodeGene(4, NodeGeneType.OUTPUT, 1f));

        //Create hidden node
        parent1Genome.AddNodeGene(new NodeGene(5, NodeGeneType.HIDDEN, 0.5f));

        //Create connections
        parent1Genome.AddConnectionGene(new ConnectionGene(1, 4, 1.0, true, 1));
        parent1Genome.AddConnectionGene(new ConnectionGene(2, 4, 1.0, false, 2));
        parent1Genome.AddConnectionGene(new ConnectionGene(3, 4, 1.0, true, 3));
        parent1Genome.AddConnectionGene(new ConnectionGene(2, 5, 1.0, true, 4));
        parent1Genome.AddConnectionGene(new ConnectionGene(5, 4, 1.0, true, 5));
        parent1Genome.AddConnectionGene(new ConnectionGene(1, 5, 1.0, true, 8));

        //-----------------------------------------------------------------------------------
        // Parent 2
        //-----------------------------------------------------------------------------------

        parent2Genome = new Genome();

        //Create the input nodes
        parent2Genome.AddNodeGene(new NodeGene(1, NodeGeneType.INPUT, 0f));
        parent2Genome.AddNodeGene(new NodeGene(2, NodeGeneType.INPUT, 0f));
        parent2Genome.AddNodeGene(new NodeGene(3, NodeGeneType.INPUT, 0f));

        //Create the output node
        parent2Genome.AddNodeGene(new NodeGene(4, NodeGeneType.OUTPUT, 1f));

        //Create hidden node
        parent2Genome.AddNodeGene(new NodeGene(5, NodeGeneType.HIDDEN, 0.4f));
        parent2Genome.AddNodeGene(new NodeGene(6, NodeGeneType.HIDDEN, 0.6f));

        //Create connections
        parent2Genome.AddConnectionGene(new ConnectionGene(1, 4, 1.0, true, 1));
        parent2Genome.AddConnectionGene(new ConnectionGene(2, 4, 1.0, false, 2));
        parent2Genome.AddConnectionGene(new ConnectionGene(3, 4, 1.0, true, 3));
        parent2Genome.AddConnectionGene(new ConnectionGene(2, 5, 1.0, true, 4));
        parent2Genome.AddConnectionGene(new ConnectionGene(5, 4, 1.0, false, 5));
        parent2Genome.AddConnectionGene(new ConnectionGene(5, 6, 1.0, true, 6));
        parent2Genome.AddConnectionGene(new ConnectionGene(6, 4, 1.0, true, 7));
        parent2Genome.AddConnectionGene(new ConnectionGene(3, 5, 1.0, true, 9));
        parent2Genome.AddConnectionGene(new ConnectionGene(1, 6, 1.0, true, 10));

        //-----------------------------------------------------------------------------------
        // Feed forward network
        //-----------------------------------------------------------------------------------

        //Nodes
        feedForwardNetwork = new Genome();
        feedForwardNetwork.AddNodeGene(new NodeGene(0, NodeGeneType.INPUT, 0f));
        feedForwardNetwork.AddNodeGene(new NodeGene(1, NodeGeneType.INPUT, 0f));

        feedForwardNetwork.AddNodeGene(new NodeGene(2, NodeGeneType.OUTPUT, 1f));
        feedForwardNetwork.AddNodeGene(new NodeGene(3, NodeGeneType.OUTPUT, 1f));

        feedForwardNetwork.AddNodeGene(new NodeGene(4, NodeGeneType.HIDDEN, 0.5f));
        feedForwardNetwork.AddNodeGene(new NodeGene(5, NodeGeneType.HIDDEN, 0.5f));
        feedForwardNetwork.AddNodeGene(new NodeGene(6, NodeGeneType.HIDDEN, 0.5f));

        //Connections
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(0, 4, 0.2, true, 0));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(0, 5, 0.3, true, 1));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(1, 5, 0.4, true, 2));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(1, 6, 0.5, true, 3));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(4, 2, 0.6, true, 4));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(5, 2, 0.7, true, 5));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(5, 3, 0.8, true, 6));
        feedForwardNetwork.AddConnectionGene(new ConnectionGene(6, 3, 0.9, true, 7));

        //-----------------------------------------------------------------------------------
        // Recurrent network
        //-----------------------------------------------------------------------------------

        recurrentNetwork = new Genome(feedForwardNetwork);
        recurrentNetwork.AddConnectionGene(new ConnectionGene(2, 4, -2, true, 8));
        recurrentNetwork.AddConnectionGene(new ConnectionGene(6, 6, -2, true, 9));

    }

    #region Neat test methods

    [Test]
    public void CopyGenome_Test()
    {
        Genome copiedGenome = new Genome(parent1Genome);

        //Check for different objects
        Assert.AreNotEqual(copiedGenome, parent1Genome);

        //Check for same
        Assert.AreEqual(parent1Genome.Nodes.Count, copiedGenome.Nodes.Count);
        Assert.AreEqual(parent1Genome.Connections.Count, copiedGenome.Connections.Count);

        //Compare nodes
        foreach (int i in parent1Genome.Nodes.Keys)
        {
            Assert.True(copiedGenome.Nodes.ContainsKey(i));
            Assert.True(CompareNodeGenes(parent1Genome.Nodes[i], copiedGenome.Nodes[i]));
        }

        foreach (int i in parent1Genome.Connections.Keys)
        {
            Assert.True(copiedGenome.Connections.ContainsKey(i));
            Assert.True(CompareConnectionGenes(parent1Genome.Connections[i], copiedGenome.Connections[i]));
        }
    }

    [Test]
    public void AddConnectionMutation_Test()
    {
        NodeGene node1 = parent1Genome.Nodes[3];
        NodeGene node2 = parent1Genome.Nodes[5];

        //Test list size before modifying
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);

        //Add the connection
        ConnectionGene newConnection = parent1Genome.AddConnectionMutation(node1, node2, 11);

        //Test if a connection was added to the list
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 7);
        Assert.NotNull(newConnection);

        //Test if connection is added to the list
        Assert.AreEqual(newConnection, parent1Genome.Connections[newConnection.InnovationNumber]);

        //Test if connection has correct input node
        Assert.AreEqual(newConnection.InNode, node1.ID);
        Assert.AreEqual(newConnection.OutNode, node2.ID);
        Assert.GreaterOrEqual(newConnection.Weight, -1.0f);
        Assert.LessOrEqual(newConnection.Weight, 1.0f);
        Assert.AreEqual(11, newConnection.InnovationNumber);
    }

    [Test]
    public void AddConnectionMutationInvalid_ExistingConnection_Test()
    {
        NodeGene node1 = parent1Genome.Nodes[1];
        NodeGene node2 = parent1Genome.Nodes[4];

        //Test list size before modifying
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);

        ConnectionGene newConnectionGene = parent1Genome.AddConnectionMutation(node1, node2, 11);

        //Test if the returned connection is null and not added to the connection list
        Assert.IsNull(newConnectionGene);
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);
    }

    [Test]
    public void AddConnectionMutation_SameInput_Test()
    {
        NodeGene node1 = parent1Genome.Nodes[5];
        NodeGene node2 = parent1Genome.Nodes[5];

        //Test list size before modifying
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);

        ConnectionGene newConnectionGene = parent1Genome.AddConnectionMutation(node1, node2, 11);

        //Test if the returned connection is not null and added to the connection list
        Assert.Null(newConnectionGene);
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);
    }

    [Test]
    public void IsConnectionPossible_Test()
    {
        NodeGene inputNode = parent1Genome.Nodes[3];
        NodeGene inputNode2 = parent1Genome.Nodes[1];
        NodeGene hiddenNode = parent1Genome.Nodes[5];
        NodeGene outputNode = parent1Genome.Nodes[4];

        Assert.False(parent1Genome.IsConnectionPossible(hiddenNode, inputNode));
        Assert.False(parent1Genome.IsConnectionPossible(outputNode, inputNode));
        Assert.False(parent1Genome.IsConnectionPossible(inputNode2, hiddenNode));

        Assert.True(parent1Genome.IsConnectionPossible(inputNode, hiddenNode));
    }

    [Test]
    public void AddNoteMutation_Test()
    {
        //Test list size before modifying
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);
        Assert.AreEqual(parent1Genome.Nodes.Values.Count, 5);

        ConnectionGene splittedConnection = parent1Genome.Connections[3];
        object[] result = parent1Genome.AddNodeMutation(splittedConnection, 7, 11, 12);

        //Test if a node and connections were added
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 8);
        Assert.AreEqual(parent1Genome.Nodes.Values.Count, 6);

        //Test the amount of results
        Assert.AreEqual(result.Length, 4);

        //Compare the disabled connection with the connection from the result
        Assert.AreEqual(splittedConnection, (ConnectionGene)result[0]);
        Assert.AreEqual(false, splittedConnection.Expressed);

        //Check ne newly created node
        NodeGene newNode = (NodeGene)result[1];
        Assert.AreEqual(NodeGeneType.HIDDEN, newNode.Type);
        Assert.AreEqual(newNode, parent1Genome.Nodes[newNode.ID]);
        Assert.AreEqual(7, newNode.ID);

        //Check the new connections
        ConnectionGene inToNew = (ConnectionGene)result[2];
        ConnectionGene newToOut = (ConnectionGene)result[3];

        //Check the in to new connection
        Assert.AreEqual(splittedConnection.InNode, inToNew.InNode);
        Assert.AreEqual(1.0, inToNew.Weight);
        Assert.AreEqual(newNode.ID, inToNew.OutNode);
        Assert.AreEqual(11, inToNew.InnovationNumber);
        Assert.AreEqual(inToNew, parent1Genome.Connections[inToNew.InnovationNumber]);

        //Check the new to out connection
        Assert.AreEqual(newNode.ID, newToOut.InNode);
        Assert.AreEqual(splittedConnection.Weight, newToOut.Weight);
        Assert.AreEqual(splittedConnection.OutNode, newToOut.OutNode);
        Assert.AreEqual(12, newToOut.InnovationNumber);
        Assert.AreEqual(newToOut, parent1Genome.Connections[newToOut.InnovationNumber]);
    }

    [Test]
    public void AddNoteMutationInvalid_NotExpressedConnection_Test()
    {
        //Test list size before modifying
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);
        Assert.AreEqual(parent1Genome.Nodes.Values.Count, 5);

        ConnectionGene splittedConnection = parent1Genome.Connections[2];
        object[] result = parent1Genome.AddNodeMutation(splittedConnection, 7, 11, 12);

        Assert.IsNull(result);
        Assert.AreEqual(parent1Genome.Connections.Values.Count, 6);
        Assert.AreEqual(parent1Genome.Nodes.Values.Count, 5);
    }

    [Test]
    public void IsNodePossible_Test()
    {
        ConnectionGene expressedGene = parent1Genome.Connections[1];
        ConnectionGene notExpressedGene = parent1Genome.Connections[2];

        Assert.False(parent1Genome.IsNodePossible(notExpressedGene));
        Assert.True(parent1Genome.IsNodePossible(expressedGene));
    }

    [Test]
    public void CrossOver_Test()
    {
        //Test Crossover with parent2 as the more fit parent

        //Crossover parents
        Genome child1 = Genome.CrossOver(parent2Genome, parent1Genome);

        //Test result
        Assert.IsNotNull(child1);
        Assert.AreEqual(6, child1.Nodes.Count);
        Assert.AreEqual(9, child1.Connections.Count);

        //Test matching genes
        for (int i = 1; i <= 5; i++)
        {
            Assert.True(
                child1.Connections[i].Weight == parent1Genome.Connections[i].Weight ||
                child1.Connections[i].Weight == parent2Genome.Connections[i].Weight);

            Assert.True(child1.Connections[i].InNode == parent1Genome.Connections[i].InNode);
            Assert.True(child1.Connections[i].OutNode == parent1Genome.Connections[i].OutNode);

            Assert.True(
                child1.Connections[i].Expressed == parent1Genome.Connections[i].Expressed ||
                child1.Connections[i].Expressed == parent2Genome.Connections[i].Expressed);
        }

        //Test disjoint or excess genes
        Assert.True(CompareConnectionGenes(child1.Connections[6], parent2Genome.Connections[6]));
        Assert.True(CompareConnectionGenes(child1.Connections[7], parent2Genome.Connections[7]));
        Assert.True(!child1.Connections.ContainsKey(8));
        Assert.True(CompareConnectionGenes(child1.Connections[9], parent2Genome.Connections[9]));
        Assert.True(CompareConnectionGenes(child1.Connections[10], parent2Genome.Connections[10]));

    }

    [Test]
    public void MutateConnetions_Test()
    {
        System.Collections.Generic.Dictionary<int, ConnectionGene> connections = parent1Genome.Connections;

        System.Collections.Generic.Dictionary<int, double> connectionWeights = new System.Collections.Generic.Dictionary<int, double>();

        //Store old weights
        foreach (int key in connectionWeights.Keys)
        {
            connectionWeights.Add(key, connections[key].Weight);
        }

        //Mutate connections
        parent1Genome.MutateConnectionWeight();

        //Compare old values
        foreach (int key in connectionWeights.Values)
        {
            Assert.AreNotEqual(connectionWeights[key], parent1Genome.Connections[key].Weight);
        }


    }

    [Test]
    public void CalculateCompabilityDistanceValues_Test()
    {
        object[] resultValues1 = Genome.CalculateCompabilityDistanceValues(parent1Genome, parent2Genome);
        object[] resultValues2 = Genome.CalculateCompabilityDistanceValues(parent2Genome, parent1Genome);

        //Values should be indepented of which parent goes first
        Assert.AreEqual((int)resultValues1[0], (int)resultValues2[0]);
        Assert.AreEqual((float)resultValues1[1], (float)resultValues2[1]);
        Assert.AreEqual((int)resultValues1[2], (int)resultValues2[2]);
        Assert.AreEqual((int)resultValues1[3], (int)resultValues2[3]);
        Assert.AreEqual((int)resultValues1[4], (int)resultValues2[4]);

        //Test amoutn matching Genes
        int amountMatchingGenes = (int)resultValues1[0];
        Assert.AreEqual(10, amountMatchingGenes);

        //Test amount average weight
        float averagWeightDif = (float)resultValues1[1];
        Assert.AreEqual(0, averagWeightDif);

        //Test Disjoint genes
        int disjoinGenes = (int)resultValues1[2];
        Assert.AreEqual(3, disjoinGenes);

        //Test excess Genes
        int excessGenes = (int)resultValues1[3];
        Assert.AreEqual(3, excessGenes);

        //Test amount genomes in the larger geene
        int amountGenomes = (int)resultValues1[4];
        Assert.AreEqual(15, amountGenomes);
    }

    [Test]
    public void CompabilityDistanceFunction_Test()
    {
        float result = Genome.CompabilityDistanceFunction(parent1Genome, parent2Genome, 1f, 1, 0.4f, 20);
        Assert.AreEqual(6, result);

        result = Genome.CompabilityDistanceFunction(parent1Genome, parent2Genome, 5f, 4, 0.4f, 1);
        Assert.AreEqual(1.8f, result, 0.01f);
    }

    #endregion

    #region Neuronal net methods

    [Test]
    public void Init_Test()
    {
        Assert.Null(parent2Genome.InputNodes);
        Assert.Null(parent2Genome.OutputNodes);

        foreach (NodeGene node in parent2Genome.Nodes.Values)
        {
            Assert.IsEmpty(node.Inputs);
        }

        //Init
        parent2Genome.Init();

        Assert.AreEqual(3, parent2Genome.InputNodes.Count);
        Assert.AreEqual(1, parent2Genome.OutputNodes.Count);

        foreach (NodeGene node in parent2Genome.Nodes.Values)
        {
            foreach (ConnectionGene input in node.Inputs)
            {
                Assert.AreEqual(node.ID, input.OutNode);
            }
        }

    }

    [Test]
    public void InitInputOutputNodes_Test()
    {
        Assert.Null(parent2Genome.InputNodes);
        Assert.Null(parent2Genome.OutputNodes);

        //Init nodes
        parent2Genome.InitInputOutputNodes();

        Assert.AreEqual(3, parent2Genome.InputNodes.Count);
        Assert.AreEqual(1, parent2Genome.OutputNodes.Count);

        foreach (NodeGene node in parent2Genome.InputNodes)
        {
            Assert.AreEqual(NodeGeneType.INPUT, node.Type);
        }

        foreach (NodeGene node in parent2Genome.OutputNodes)
        {
            Assert.AreEqual(NodeGeneType.OUTPUT, node.Type);
        }
    }

    [Test]
    public void MatchConnectionsToNodes_Test()
    {
        parent1Genome.MatchConnectionsToNodes();

        foreach (NodeGene node in parent1Genome.Nodes.Values)
        {
            if (node.Type != NodeGeneType.INPUT)
            {
                //Hidden and Output nodes should have atleast one input
                Assert.GreaterOrEqual(node.Inputs.Count, 1);
            }
            else
            {
                //Input nodes should not have an input
                Assert.AreEqual(0, node.Inputs.Count);
            }

            foreach (ConnectionGene input in node.Inputs)
            {
                Assert.AreEqual(node.ID, input.OutNode);
                Assert.AreEqual(true, input.Expressed);
            }
        }
    }

    [Test]
    public void ResetNodes_Test()
    {
        foreach (NodeGene node in parent1Genome.Nodes.Values)
        {
            node.Inputs = new List<ConnectionGene>();
            node.CalculateValue(new Dictionary<int, NodeGene>(), new Stack<int>());

            Assert.AreEqual(true, node.CurrentValCalculatedFlag);
        }

        //Reset visited flag
        parent1Genome.ResetNodes();

        //Check if value was deleted
        foreach (NodeGene node in parent1Genome.Nodes.Values)
        {
            Assert.AreEqual(false, node.CurrentValCalculatedFlag);
        }
    }

    [Test]
    public void SetInputInNodes_Test()
    {
        double[] input = { 1, 2, 3 };

        parent1Genome.Init();

        //Make sure the values are not set
        foreach (NodeGene inputNode in parent1Genome.InputNodes)
        {
            Assert.AreEqual(0, inputNode.CurrentVal);
            Assert.AreEqual(false, inputNode.CurrentValCalculatedFlag);
        }

        //Set input value
        parent1Genome.SetInputInNodes(input, parent1Genome.InputNodes);

        for (int i = 0; i < input.Length; i++)
        {
            Assert.AreEqual(input[i], parent1Genome.InputNodes[i].CurrentVal);
            Assert.AreEqual(true, parent1Genome.InputNodes[i].CurrentValCalculatedFlag);
        }
    }

    [Test]
    public void SetInputInNodesException_Test()
    {
        double[] input = { 1, 2 };

        parent1Genome.Init();

        try
        {
            parent1Genome.SetInputInNodes(input, parent1Genome.InputNodes);
            Assert.Fail();
        }
        catch (System.Exception)
        {
            Assert.True(true);
        }

    }

    [Test]
    public void Calculate_Test()
    {
        double[] input = { 0.5, 1 };
        double[] desiredOutput = { 0.681, 0.744 };

        feedForwardNetwork.Init();


        double[] output = feedForwardNetwork.Calculate(input);
        Assert.AreEqual(output.Length, desiredOutput.Length);

        //Check result
        Assert.AreEqual(desiredOutput[0], output[0], 0.001);
        Assert.AreEqual(desiredOutput[1], output[1], 0.001);

        //Second example

        double[] input2 = { -1, 2 };
        double[] desiredOutput2 = { 0.669, 0.760 };

        double[] output2 = feedForwardNetwork.Calculate(input2);
        Assert.AreEqual(output2.Length, desiredOutput.Length);

        //Check result
        Assert.AreEqual(desiredOutput2[0], output2[0], 0.001);
        Assert.AreEqual(desiredOutput2[1], output2[1], 0.001);

    }

    [Test]
    public void CalculateRecurrent_Test()
    {
        double[] input ={ -1, 2};
        double[] desiredOutputFirstIteration = { 0.669, 0.760 };
        double[] desiredOutputSecondIteration = { 0.632, 0.699 };

        recurrentNetwork.Init();

        double[] outputFirstIteration = recurrentNetwork.Calculate(input);
        Assert.AreEqual(desiredOutputFirstIteration.Length, outputFirstIteration.Length);

        //Check result
        Assert.AreEqual(desiredOutputFirstIteration[0], outputFirstIteration[0], 0.001);
        Assert.AreEqual(desiredOutputFirstIteration[1], outputFirstIteration[1], 0.001);

        //Second iteration
        double[] outputSecondIteration = recurrentNetwork.Calculate(input);
        Assert.AreEqual(desiredOutputSecondIteration.Length, outputSecondIteration.Length);

        //Check result
        Assert.AreEqual(desiredOutputSecondIteration[0], outputSecondIteration[0], 0.001);
        Assert.AreEqual(desiredOutputSecondIteration[1], outputSecondIteration[1], 0.001);
    }

    #endregion

    #region Helper methods

    public bool CompareConnectionGenes(ConnectionGene gene1, ConnectionGene gene2)
    {
        if (
            gene1.InNode == gene2.InNode &&
            gene1.OutNode == gene2.OutNode &&
            gene1.Expressed == gene2.Expressed &&
            gene1.InnovationNumber == gene2.InnovationNumber &&
            gene1.Weight == gene2.Weight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CompareNodeGenes(NodeGene node1, NodeGene node2)
    {
        if (node1.ID == node2.ID &&
            node1.Type == node2.Type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
