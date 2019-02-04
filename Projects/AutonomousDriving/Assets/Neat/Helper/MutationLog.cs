using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationLog
{

    public Dictionary<int, List<int>> NodeMutations { get { return _nodeMutations; } }
    public Dictionary<ConnectionMutationKey, int> ConnectionMutations { get { return _connectionMutations; } }

    private Dictionary<int, List<int>> _nodeMutations;
    private Dictionary<ConnectionMutationKey, int> _connectionMutations;

    /// <summary>
    /// Create a new mutation log with two empty dicitionaries
    /// </summary>
    public MutationLog()
    {
        _nodeMutations = new Dictionary<int, List<int>>();
        _connectionMutations = new Dictionary<ConnectionMutationKey, int>();
    }

    #region Public methods

    /// <summary>
    /// Add the connection in the mutation log
    /// </summary>
    /// <param name="connection">the connection that should be stored</param>
    public void AddConnectionMutation(ConnectionGene connection)
    {
        AddConnectionMutation(connection.InNode, connection.OutNode, connection.InnovationNumber);
    }

    /// <summary>
    /// Add the connection in the mutation log
    /// </summary>
    /// <param name="inNodeID">the in node of the new connection</param>
    /// <param name="outNodeID">the out node of the new connection</param>
    /// <param name="connectionInnovationNumber">the innovation number of the connection</param>
    public void AddConnectionMutation(int inNodeID, int outNodeID, int connectionInnovationNumber)
    {
        ConnectionMutationKey key = new ConnectionMutationKey(inNodeID, outNodeID);
        _connectionMutations.Add(key, connectionInnovationNumber);
    }

    /// <summary>
    /// Add the node to the mutation log. The node is identified by the connection that was splitted for the node
    /// </summary>
    /// <param name="splittedConnection">the connection that was splitted for the new node</param>
    /// <param name="newNode">the newly created node</param>
    public void AddNodeMutation(ConnectionGene splittedConnection, NodeGene newNode)
    {
        AddNodeMutation(splittedConnection.InnovationNumber, newNode.ID);
    }

    /// <summary>
    /// Add the node to the mutation log. The node is identified by the connection that was splitted for the node
    /// </summary>
    /// <param name="splittedConnectionInnovationNumber">the innovation number of the connection that was splitted</param>
    /// <param name="newNodeID">the id of the new node</param>
    public void AddNodeMutation(int splittedConnectionInnovationNumber, int newNodeID)
    {
        //If the key is already in the log add the connection
        if (_nodeMutations.ContainsKey(splittedConnectionInnovationNumber))
        {
            _nodeMutations[splittedConnectionInnovationNumber].Add(newNodeID);
        }
        else
        {
            List<int> nodeIds = new List<int>();
            nodeIds.Add(newNodeID);
            _nodeMutations.Add(splittedConnectionInnovationNumber, nodeIds);
        }
    }

    /// <summary>
    /// Check if a node is already existing
    /// </summary>
    /// <param name="connection">the connection that should be splitted</param>
    /// <returns>a list with all node ids that match the connection</returns>
    public List<int> IsNodeExisting(ConnectionGene connection)
    {
        return IsNodeExisting(connection.InnovationNumber);
    }

    /// <summary>
    /// Check if a node is already existing
    /// </summary>
    /// <param name="connectionInnovationNumber">the innovation number of the connection that should be splitted</param>
    /// <returns>the id of the node or -1, if the node is not yet existing</returns>
    public List<int> IsNodeExisting(int connectionInnovationNumber)
    {
        if (_nodeMutations.ContainsKey(connectionInnovationNumber))
        {
            return _nodeMutations[connectionInnovationNumber];
        }
        else
        {
            return new List<int>();
        }
    }

    /*

    /// <summary>
    /// Get an ID for the a new node that will be splaced between the given connection.
    /// If there is already an entry for the node, the existing id will be returned.
    /// If it is a new node a new numer will be generated with the GeneCounter and after that directly stored.
    /// </summary>
    /// <param name="connectionInnovationNumber">innovationNumber of the conenction that will be splitted</param>
    /// <param name="nodeMutationCounter">to generate a new node ID if necessary</param>
    /// <returns>an ID for the node</returns>
    public int GetNodeID(int connectionInnovationNumber, GeneCounter nodeMutationCounter)
    {
        int nodeID = IsNodeExisting(connectionInnovationNumber);

        //If node is existing return the value
        if (nodeID != -1) return nodeID;

        nodeID = nodeMutationCounter.GetNewNumber();

        //Add node to the list
        AddNodeMutation(connectionInnovationNumber, nodeID);

        return nodeID;

    }

    */

    /// <summary>
    /// Get an id for a new node.
    /// If the mutation has already occured, an existing id will be returned.
    /// If the mutation is new, a new id will be generated and stored in the log
    /// </summary>
    /// <param name="connectionInnovationNumber">the innovation number of the splitted connection</param>
    /// <param name="nodeIDs">a list with all node ids' of the genome</param>
    /// <param name="nodeMutationCounter">a node mutation counter to generate a new id if necessary</param>
    /// <returns>an id for the node</returns>
    public int GetNodeID(int connectionInnovationNumber, List<int> nodeIDs, GeneCounter nodeMutationCounter)
    {
        List<int> existingNodeIDs = IsNodeExisting(connectionInnovationNumber);
        foreach (int existingID in existingNodeIDs)
        {
            //If the node Id list does not contain one of the existing nodes, break the loop
            if (!nodeIDs.Contains(existingID))
            {
                return existingID;
            }
        }

        //If the id is new, add an entry in the log
        int newID = nodeMutationCounter.GetNewNumber();
        AddNodeMutation(connectionInnovationNumber, newID);
        return newID;
    }

    /// <summary>
    /// Check if a connection is already existing
    /// </summary>
    /// <param name="inNode">the in node</param>
    /// <param name="outNode">the out node</param>
    /// <returns>the innovation number of the node or -1 if the node ist not yet existing</returns>
    public int IsConnectionExisting(NodeGene inNode, NodeGene outNode)
    {
        return IsConnectionExisting(inNode.ID, outNode.ID);
    }

    /// <summary>
    /// Check if a connection is aready existing
    /// </summary>
    /// <param name="inNodeID">id of the in node</param>
    /// <param name="outNodeID">id of the out node</param>
    /// <returns>the innovation number of the node or -1 if the node ist not yet existing</returns>
    public int IsConnectionExisting(int inNodeID, int outNodeID)
    {
        ConnectionMutationKey key = new ConnectionMutationKey(inNodeID, outNodeID);
        if (_connectionMutations.ContainsKey(key))
        {
            return _connectionMutations[key];
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Get an InnovationNumber for the connection between the given nodes.
    /// If there is already and entry for the connection, the existing InnovationNumber will be returned
    /// If it is a new connection, a new number will be generated with the connectionMutationCounter and after that directly stored
    /// </summary>
    /// <param name="inNodeID">the in node of the new connection</param>
    /// <param name="outNodeID">the out node of the new connection</param>
    /// <param name="connectionMutationCounter">GeneCounter to generate a new number if necessary</param>
    /// <returns>the InnovationNumber for the connection</returns>
    public int GetConnectionInnovationNumber(int inNodeID, int outNodeID, GeneCounter connectionMutationCounter)
    {
        int innovationNumber = IsConnectionExisting(inNodeID, outNodeID);

        //If node is existing return the value
        if (innovationNumber != -1) return innovationNumber;

        innovationNumber = connectionMutationCounter.GetNewNumber();

        //Store new mutation
        AddConnectionMutation(inNodeID, outNodeID, innovationNumber);
        return innovationNumber;
    }

    #endregion


    #region Helper class to craete keys for the connection mutations

    public class ConnectionMutationKey
    {
        public int InNode { get { return _inNode; } }
        public int OutNode { get { return _outNode; } }

        private int _inNode;
        private int _outNode;

        /// <summary>
        /// Create a key for the connection mutation dictionary
        /// </summary>
        /// <param name="inNode">the id of the in node</param>
        /// <param name="outNode">the id of the outnode</param>
        public ConnectionMutationKey(int inNode, int outNode)
        {
            _inNode = inNode;
            _outNode = outNode;
        }

        /// <summary>
        /// The object are equal to each other if the InNode and OutNode are the same
        /// </summary>
        /// <param name="obj">the object that should be compared</param>
        /// <returns>true if the objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            return Equals((ConnectionMutationKey)obj);
        }

        /// <summary>
        /// Return true if the objects have the same in- and out Node
        /// </summary>
        /// <param name="key">The object that should be compared</param>
        /// <returns>true if the objects have the same in and out Node</returns>
        public bool Equals(ConnectionMutationKey key)
        {
            if (_inNode == key._inNode &&
                _outNode == key._outNode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the objects are equal to each other
        /// </summary>
        /// <param name="key1">the first connection</param>
        /// <param name="key2">the second connection</param>
        /// <returns>true if the objects are equal to each other</returns>
        public static bool operator ==(ConnectionMutationKey key1, ConnectionMutationKey key2)
        {
            return key1.Equals(key2);
        }

        /// <summary>
        /// Check if the objects are not equal to each other
        /// </summary>
        /// <param name="key1">the first connection</param>
        /// <param name="key2">the second connection</param>
        /// <returns>true if the objects are not equal to each other</returns>
        public static bool operator !=(ConnectionMutationKey key1, ConnectionMutationKey key2)
        {
            return !key1.Equals(key2);
        }

        public override int GetHashCode()
        {
            var hashCode = 786765615;
            hashCode = hashCode * -1521134295 + _inNode.GetHashCode();
            hashCode = hashCode * -1521134295 + _outNode.GetHashCode();
            return hashCode;
        }

    }

    #endregion
}
