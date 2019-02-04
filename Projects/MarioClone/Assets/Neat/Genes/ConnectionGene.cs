using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionGene {

    /// <summary>
    /// This section contains all property fields
    /// </summary>

    public int InNode { get { return _inNode; } set { _inNode = value; } }
    public int OutNode { get { return _outNode; } set { _outNode = value; } }
    public double Weight { get { return _weight; } set { _weight = value; } }
    public bool Expressed { get { return _expressed; } set { _expressed = value; } }
    public int InnovationNumber { get { return _innovationNumber; } set { _innovationNumber = value; } }

    /// <summary>
    /// Private variables
    /// </summary>

    private int _inNode;
    private int _outNode;
    private double _weight;
    private bool _expressed;

    private int _innovationNumber;

    /// <summary>
    /// Create a new connection gene
    /// </summary>
    /// <param name="inNode">ID of the input node</param>
    /// <param name="outNode">ID of the output node</param>
    /// <param name="weight">weight for the connection</param>
    /// <param name="expressed">true if the connection is expressed (active)</param>
    /// <param name="innovationNumber">of the connection</param>
    public ConnectionGene(int inNode, int outNode, double weight, bool expressed, int innovationNumber)
    {
        _inNode = inNode;
        _outNode = outNode;
        _weight = weight;
        _expressed = expressed;
        _innovationNumber = innovationNumber;
    }

    public ConnectionGene(ConnectionGene connection)
    {
        _inNode = connection.InNode;
        _outNode = connection.OutNode;
        _weight = connection.Weight;
        _expressed = connection.Expressed;
        _innovationNumber = connection.InnovationNumber;
    }

}
