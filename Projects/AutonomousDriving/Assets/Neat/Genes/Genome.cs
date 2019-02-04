using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Genome
{
    #region Tuning Parameters

    //Parameters for mutating the weights
    public static float _PROBABILITY_PERTURBING = 0.9f;
    public static float _SIGMA_GAUSS = 1.3f;

    #endregion

    #region Properties

    public Dictionary<int, ConnectionGene> Connections { get { return _connections; } set { _connections = value; } }
    public Dictionary<int, NodeGene> Nodes { get { return _nodes; } set { _nodes = value; } }

    public List<NodeGene> InputNodes { get { return _inputNodes; } }
    public List<NodeGene> OutputNodes { get { return _outputNodes; } }

    #endregion

    #region Private variables

    //Hold all connections and neurons
    private Dictionary<int, ConnectionGene> _connections;
    private Dictionary<int, NodeGene> _nodes;

    //Variables for in and ouput
    private List<NodeGene> _inputNodes;
    private List<NodeGene> _outputNodes;

    #endregion

    public Genome()
    {
        _connections = new Dictionary<int, ConnectionGene>();
        _nodes = new Dictionary<int, NodeGene>();
    }

    public Genome(Genome genome)
    {
        _connections = new Dictionary<int, ConnectionGene>();
        _nodes = new Dictionary<int, NodeGene>();

        foreach (int i in genome.Nodes.Keys)
        {
            _nodes.Add(i, new NodeGene(genome.Nodes[i]));
        }

        foreach (int i in genome.Connections.Keys)
        {
            _connections.Add(i, new ConnectionGene(genome.Connections[i]));
        }
    }

    #region Public Methods

    #region Methods for mutation

    /// <summary>
    /// Add a new connection between the given nodes.
    /// The connection will be initialized with a random weight beteen -1.0f and 1.0f.
    /// The connection can't be created if one of the following statements is true:
    /// 1. If the nodes are both input oder both output nodes
    /// 2. An connection with the values or revered values already exists
    /// 3. If the nodes are the same object.
    /// </summary>
    /// <param name="inNode"></param>
    /// <param name="outNode"></param>
    /// <param name="connectionInnovationNumber">the innovationNumber for the connection</param>
    /// <returns>the newly created ConnectionGene or null, it the connection could not be created</returns>
    public ConnectionGene AddConnectionMutation(NodeGene inNode, NodeGene outNode, int connectionInnovationNumber)
    {
        /*
        //Test if the given nodes are two different nodes
        if (node1.ID == node2.ID) return null;

        //Check if the nodes bust be revered, because input nodes can only be inputs and output can only be output
        bool reversed = false;

        if (node1.Type == NodeGeneType.HIDDEN && node2.Type == NodeGeneType.INPUT) reversed = true;
        else if (node1.Type == NodeGeneType.OUTPUT && node2.Type == NodeGeneType.HIDDEN) reversed = true;
        else if (node1.Type == NodeGeneType.OUTPUT && node2.Type == NodeGeneType.INPUT) reversed = true;

        //check if the connection is possible.
        bool connectionImpossible = false;

        if (node1.Type == NodeGeneType.INPUT && node2.Type == NodeGeneType.INPUT) connectionImpossible = true;
        else if (node1.Type == NodeGeneType.OUTPUT && node2.Type == NodeGeneType.OUTPUT) connectionImpossible = true;

        //If the connection is impossible, than return null
        if (connectionImpossible) return null;

        //Check if the connections exists in any direction
        //TODO Check if there are circular connections?
        bool connectionExists = false;

        foreach (ConnectionGene currentConnection in _connections.Values)
        {
            if (
               (currentConnection.InNode == node1.ID && currentConnection.OutNode == node2.ID) ||
               (currentConnection.InNode == node2.ID && currentConnection.OutNode == node1.ID)
               )
            {
                connectionExists = true;
                break;
            }
        }
        

        //If the connection already exist, return null
        if (connectionExists) return null;

        */

        //Check if the connection can be created
        if (!IsConnectionPossible(inNode, outNode))
        {
            return null;
        }

        int inNodeID = inNode.ID;
        int outNodeID = outNode.ID;
        double weight = Random.Range(-1.0f, 1.0f);
        bool expressed = true;
        int innovationNumber = connectionInnovationNumber;

        //Create a new connection
        ConnectionGene newCon = new ConnectionGene(inNodeID, outNodeID, weight, expressed, innovationNumber);
        _connections.Add(innovationNumber, newCon);
        return newCon;
    }

    /// <summary>
    /// Test if a connection between these two nodes is possible.
    /// This includes a check if the connection is already existing
    /// </summary>
    /// <param name="inNode">the node where the connection starts</param>
    /// <param name="outNode">the node where the connection ends</param>
    /// <returns>true if the connection is possible</returns>
    public bool IsConnectionPossible(NodeGene inNode, NodeGene outNode)
    {
        //Check if a node is invalid
        if (outNode.Type == NodeGeneType.INPUT) return false;
        if (inNode.Type == NodeGeneType.OUTPUT) return false;

        //TODO recurrent check
        if (inNode.XValue >= outNode.XValue) return false;

        //Check fpr an existing connection
        foreach (ConnectionGene connection in _connections.Values)
        {
            if (connection.InNode == inNode.ID &&
                connection.OutNode == outNode.ID)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Mutate the genome and replaces an existing connection with a new node and two new connections.
    /// The existing connection will be disabled and a new node will be created. The new node will
    /// be set between the input and output node of the old connection. 
    /// Two connections will be created. The first one is from the disabled connections input node to the newly created node with a weight of 1.
    /// The second is from the newly created input node the the disabled connections output node with the weight if the disabled connection.
    /// If the connection is already disabled, null will be returned
    /// </summary>
    /// <param name="connection">the existing connection that will be replaced</param>
    /// <returns>an array with 4 objects. 
    /// Index 0: the disabled connection
    /// Index 1: the newly created Node
    /// Index 2: the newly created Connection between the Input and the newly created node and a weight of 1
    /// Index 3: the newly created Connection between the newly created node and the output node with the weight of the given connection.
    /// Return null, if the connection is already disabled</returns>
    public System.Object[] AddNodeMutation(ConnectionGene connection, int nodeID, int inToNewNodeInnovationNumber, int newToOutNodeInnovationNumber)
    {
        //Test if the connection is available
        if (!IsNodePossible(connection)) return null;

        NodeGene inNode = _nodes[connection.InNode];
        NodeGene outNode = _nodes[connection.OutNode];

        //Disable the connection
        connection.Expressed = false;

        //TODO new
        float yValue = inNode.XValue + ((outNode.XValue - inNode.XValue) / 2);

        //Get the activation function
        ActivationFunctionHelper.Function type = Random.Range(-1f, 1f) >= 0 ? inNode.TypeOfActivationFunction : outNode.TypeOfActivationFunction;

        NodeGene newNode = new NodeGene(nodeID, NodeGeneType.HIDDEN, yValue, type);
        ConnectionGene inToNewNodeConnection = new ConnectionGene(inNode.ID, newNode.ID, 1f, true, inToNewNodeInnovationNumber);
        ConnectionGene newToOutNodeConnection = new ConnectionGene(newNode.ID, outNode.ID, connection.Weight, true, newToOutNodeInnovationNumber);

        _nodes.Add(newNode.ID, newNode);
        _connections.Add(inToNewNodeConnection.InnovationNumber, inToNewNodeConnection);
        _connections.Add(newToOutNodeConnection.InnovationNumber, newToOutNodeConnection);

        System.Object[] changedValues = new System.Object[4];
        changedValues[0] = connection;
        changedValues[1] = newNode;
        changedValues[2] = inToNewNodeConnection;
        changedValues[3] = newToOutNodeConnection;

        return changedValues;
    }

    /// <summary>
    /// Test if it is possible to insert a node for the given connection
    /// </summary>
    /// <param name="gene"></param>
    /// <returns></returns>
    public bool IsNodePossible(ConnectionGene gene)
    {
        if (!gene.Expressed) return false;

        return true;
    }

    /// <summary>
    /// Mutate the connection weights
    /// </summary>
    public void MutateConnectionWeight()
    {
        foreach (ConnectionGene connection in _connections.Values)
        {
            if (Random.Range(0f, 1f) <= _PROBABILITY_PERTURBING)
            {
                //Pertube weight
                double weight = connection.Weight;
                //weight += Random.Range(0f, 1f) * 2 * 0.2 - 0.2;

                float debugValue = RandomFromDistribution.RandomNormalDistribution(0, _SIGMA_GAUSS);
                //float debugValue = Random.Range(0f, 1f) - Random.Range(0f, 1f);
                //float debugValue = Random.Range(-_SIGMA_GAUSS, _SIGMA_GAUSS);
                //Debug.Log(debugValue);

                weight += debugValue;             

                connection.Weight = weight;
            }
            else
            {
                //Assign new weight
                connection.Weight = Random.Range(-2.0f, 2.0f);
            }
        }
    }

    /// <summary>
    /// Add a node to the dictionary
    /// </summary>
    /// <param name="node">the node that should be added</param>
    public void AddNodeGene(NodeGene node)
    {
        _nodes.Add(node.ID, node);
    }

    /// <summary>
    /// Add a connection gene to the dictionary
    /// </summary>
    /// <param name="connection">the connection that should be added</param>
    public void AddConnectionGene(ConnectionGene connection)
    {
        _connections.Add(connection.InnovationNumber, connection);
    }

    #endregion

    #region Methods for the neuronal net

    /// <summary>
    /// Init the input and ouput nodes and match the connections to the nodes
    /// </summary>
    public void Init()
    {
        InitInputOutputNodes();
        MatchConnectionsToNodes();
    }

    /// <summary>
    /// Initialize a list with all input nodes and a second list with all output nodes
    /// </summary>
    public void InitInputOutputNodes()
    {
        _inputNodes = _nodes.Values.Where(x => x.Type == NodeGeneType.INPUT).OrderBy(x => x.ID).ToList();
        _outputNodes = _nodes.Values.Where(x => x.Type == NodeGeneType.OUTPUT).OrderBy(x => x.ID).ToList();
    }

    /// <summary>
    /// Match to each node their input connections.
    /// This means every connection that has as output the same id as the node
    /// </summary>
    public void MatchConnectionsToNodes()
    {
        //Clear the input list of the node
        foreach (NodeGene node in _nodes.Values)
        {
            node.Inputs.Clear();
        }

        //Order the connections so that they will always be added in the same order
        List<ConnectionGene> orderedConnection = _connections.Values.OrderBy(x => x.InnovationNumber).ToList();
        foreach (ConnectionGene connection in orderedConnection)
        {
            //Add only active connections
            if (connection.Expressed)
            {
                //Add the connection
                _nodes[connection.OutNode].Inputs.Add(connection);
            }
        }
    }

    /// <summary>
    /// Reset all nodes completly
    /// </summary>
    public void ResetNodes()
    {
        foreach (NodeGene node in _nodes.Values)
        {
            node.ResetNode();
        }
    }

    /// <summary>
    /// Calculate a value in the neuronal net.
    /// The neuronal net must be initialized
    /// </summary>
    /// <param name="input">array with input values. Must be the same as the amount of input nodes</param>
    /// <returns>an array with the output values.</returns>
    public double[] Calculate(double[] input)
    {
        //Check the amount of inputs
        if (input.Length != _inputNodes.Count) throw new System.Exception("Amount of inputs does not match the amount of input Nodes");

        //Reset nodes
        ResetNodes();

        //Set the input
        SetInputInNodes(input, _inputNodes);

        //Create output array
        double[] resultArray = new double[_outputNodes.Count];

        Stack<int> visitedNodes = new Stack<int>();
        for (int i = 0; i < _outputNodes.Count; i++)
        {
            resultArray[i] = _outputNodes[i].CalculateValue(_nodes, visitedNodes);
            if (visitedNodes.Count != 0) throw new System.Exception("Visited Nodes stack is not 0!!!");
        }

        return resultArray;
    }

    /// <summary>
    /// Set the input values in the list with the given nodes
    /// </summary>
    /// <param name="input">array with input values</param>
    /// <param name="inputNodes">a list with nodes. Must have the same size like the input array</param>
    public void SetInputInNodes(double[] input, List<NodeGene> inputNodes)
    {
        if (input.Length != inputNodes.Count) throw new System.Exception("Amount of inputs does not match the given amount of nodes");

        for (int i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].SetCurrentVal(input[i]);
        }
    }

    #endregion

    #endregion

    #region Static methods

    /// <summary>
    /// Crossover of two parent genomes to procuce one child
    /// </summary>
    /// <param name="parent1">the more fit parent!</param>
    /// <param name="parent2">the less fit parent</param>
    /// <returns>the newly breed child genome</returns>
    public static Genome CrossOver(Genome parent1, Genome parent2)
    {
        Genome childGenome = new Genome();

        //Copy nodes of the more fit parent
        foreach (NodeGene parent1Node in parent1.Nodes.Values)
        {
            childGenome.AddNodeGene(new NodeGene(parent1Node));
        }

        //Iterate through every connection
        foreach (ConnectionGene parent1Connection in parent1.Connections.Values)
        {
            if (parent2.Connections.ContainsKey(parent1Connection.InnovationNumber))
            {
                //Matching genes
                if (Random.Range(-1.0f, 1.0f) >= 0)
                {
                    childGenome.AddConnectionGene(new ConnectionGene(parent1Connection));
                }
                else
                {
                    childGenome.AddConnectionGene(new ConnectionGene(parent2.Connections[parent1Connection.InnovationNumber]));
                }
            }
            else
            {
                //Distjoint or excess gene
                childGenome.AddConnectionGene(new ConnectionGene(parent1Connection));
            }
        }

        return childGenome;
    }

    /// <summary>
    /// Calculate the values for the compability distance function.
    /// Calcualtes the amount of matching, disjoint and excess genes as well the average weight difference in the matching genes
    /// </summary>
    /// <param name="genome1">the first parent</param>
    /// <param name="genome2">the second parent</param>
    /// <returns>and array with values
    /// Index 0: amount of matching genes as int
    /// Index 1: average weight difference as float 
    /// Index 2: amount of disjoint genes as int
    /// Index 3: amount of excess genes as int
    /// Index 4: amount of genes in the larger genome</returns>
    public static object[] CalculateCompabilityDistanceValues(Genome genome1, Genome genome2)
    {
        int matchingGenes = 0;
        float weightDif = 0;
        int disjointGenes = 0;
        int excessGenes = 0;
        int amountGenesGenome1 = 0;
        int amountGenesGenome2 = 0;

        //Count matching genes in the node genes

        //Last integer in list is highes Number
        List<int> nodeKeys1 = genome1.Nodes.Keys.OrderBy(x => x).ToList();
        List<int> nodeKeys2 = genome2.Nodes.Keys.OrderBy(x => x).ToList();

        //Count matching nodes
        int highestNode1 = nodeKeys1[nodeKeys1.Count - 1];
        int highestNode2 = nodeKeys2[nodeKeys2.Count - 1];

        int highesNode = Mathf.Max(highestNode1, highestNode2);

        for (int i = 0; i <= highesNode; i++)
        {
            //Count matching genes if the gene is in both parents
            if (nodeKeys1.Contains(i) && nodeKeys2.Contains(i)) matchingGenes++;

            //Count disjoint genes
            if (highestNode1 > i && !nodeKeys1.Contains(i) && nodeKeys2.Contains(i)) disjointGenes++;
            else if (highestNode2 > i && nodeKeys1.Contains(i) && !nodeKeys2.Contains(i)) disjointGenes++;

            //Count excess genes
            if (highestNode1 < i && !nodeKeys1.Contains(i) && nodeKeys2.Contains(i)) excessGenes++;
            else if (highestNode2 < i && nodeKeys1.Contains(i) && !nodeKeys2.Contains(i)) excessGenes++;
        }

        //Count matching genes in the connection genes

        //Last integer in list is highes Number
        List<int> connectionKeys1 = genome1.Connections.Keys.OrderBy(x => x).ToList();
        List<int> connectionKeys2 = genome2.Connections.Keys.OrderBy(x => x).ToList();

        int highestConnection1 = connectionKeys1[connectionKeys1.Count - 1];
        int highestConnection2 = connectionKeys2[connectionKeys2.Count - 1];

        int highestConnection = Mathf.Max(highestConnection1, highestConnection2);

        int dividerAverageWeight = 0;
        for (int i = 0; i <= highestConnection; i++)
        {
            //Count matching genes if the gene is in both parents
            if (connectionKeys1.Contains(i) && connectionKeys2.Contains(i))
            {
                matchingGenes++;

                //Calculate weight difference
                ConnectionGene connection1 = genome1.Connections[i];
                ConnectionGene connection2 = genome2.Connections[i];

                weightDif += Mathf.Abs((float)(connection1.Weight - connection2.Weight));
                dividerAverageWeight++;
            };

            //Count disjoint genes
            if (highestConnection1 > i && !connectionKeys1.Contains(i) && connectionKeys2.Contains(i)) disjointGenes++;
            else if (highestConnection2 > i && connectionKeys1.Contains(i) && !connectionKeys2.Contains(i)) disjointGenes++;

            //Count excess genes
            if (highestConnection1 < i && !connectionKeys1.Contains(i) && connectionKeys2.Contains(i)) excessGenes++;
            else if (highestConnection2 < i && connectionKeys1.Contains(i) && !connectionKeys2.Contains(i)) excessGenes++;

        }

        //Average weight diff is the weightDif / the amount of values or 0, if the divider was not increased
        float averageWeightDif = dividerAverageWeight != 0 ? weightDif / dividerAverageWeight : 0;

        //Calculate the max amount of genes in the larger genome
        amountGenesGenome1 = nodeKeys1.Count + connectionKeys1.Count;
        amountGenesGenome2 = nodeKeys2.Count + connectionKeys2.Count;
        int maxAmountGenes = Mathf.Max(amountGenesGenome1, amountGenesGenome2);

        return new object[] { matchingGenes, averageWeightDif, disjointGenes, excessGenes, maxAmountGenes };
    }

    public static float CompabilityDistanceFunction(Genome genome1, Genome genome2, float factorExcessGenes, float factorDisjointGnes, float factorAverageWeightDif, float thresholdNumberOfGenes)
    {
        object[] resultArray = CalculateCompabilityDistanceValues(genome1, genome2);

        float averageWeightDiff = (float)resultArray[1];
        int disjointGenes = (int)resultArray[2];
        int excessGenes = (int)resultArray[3];
        int amountOfGenes = (int)resultArray[4];

        //Can be set to 1 for small genes
        if (amountOfGenes <= thresholdNumberOfGenes) amountOfGenes = 1;

        float result = 
            ((factorExcessGenes * excessGenes) / amountOfGenes) + 
            ((factorDisjointGnes * disjointGenes) / amountOfGenes) + 
            (factorAverageWeightDif * averageWeightDiff);
        return result;
    }

    #endregion
}
