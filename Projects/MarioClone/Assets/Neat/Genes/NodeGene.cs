using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGene {

    /// <summary>
    /// This section contains all property fields
    /// </summary>

    #region Properties

    public int ID { get { return _id; } set { _id = value; } }
    public NodeGeneType Type { get { return _type; } set { _type = value; } }

    public List<ConnectionGene> Inputs { get { return _inputs; } set { _inputs = value; } }
    public ActivationFunctionHelper.Function TypeOfActivationFunction { get { return _typeOfFunction; } set { _typeOfFunction = value; } }

    public bool CurrentValCalculatedFlag { get { return _currentValCalculatedFlag; } }
    public double CurrentVal { get { return _currentVal; } }
    public double LastVal { get { return _lastVal; } }

    public float XValue { get { return _xValue; } }

    public float _yValue = -1;

    #endregion

    #region Private fields

    private int _id;
    private NodeGeneType _type;

    //Input connections
    private List<ConnectionGene> _inputs;

    //Activation function type
    private ActivationFunctionHelper.Function _typeOfFunction;

    //Values for the neuronal net
    private double _currentVal = 0;
    private double _lastVal = 0;
    private bool _currentValCalculatedFlag = false;

    private float _xValue;

    #endregion


    /// <summary>
    /// Create a new node gene with an id and a type
    /// </summary>
    /// <param name="id">of the node gene. Must be unique</param>
    /// <param name="type">of the node gene</param>
    public NodeGene(int id, NodeGeneType type, float xValue)
    {
        _id = id;
        _type = type;
        _xValue = xValue;
        _inputs = new List<ConnectionGene>();
        _typeOfFunction = ActivationFunctionHelper.Function.SIGMOID;
    }

    /// <summary>
    /// Create a new node gene with an id and a type
    /// </summary>
    /// <param name="id">of the node gene. Must be unique</param>
    /// <param name="type">of the node gene</param>
    /// <param name="function">the type of activation function</param>
    public NodeGene(int id, NodeGeneType type, float yValue, ActivationFunctionHelper.Function function)
    {
        _id = id;
        _type = type;
        _typeOfFunction = function;
        _xValue = yValue;
        _inputs = new List<ConnectionGene>();
    }

    /// <summary>
    /// Copy an existing NodeGene. Only the id and the type will be copied.
    /// </summary>
    /// <param name="nodeGene"></param>
    public NodeGene(NodeGene nodeGene)
    {
        _id = nodeGene.ID;
        _type = nodeGene.Type;
        _typeOfFunction = nodeGene.TypeOfActivationFunction;
        _xValue = nodeGene.XValue;
        _inputs = new List<ConnectionGene>();
    }

    #region Public Neuronal net methods

    public double CalculateValue(Dictionary<int, NodeGene> nodes, Stack<int> visitedNodes)
    {
        //Check if the node is in the stack, if so return the last value
        if (visitedNodes.Contains(_id)) return _lastVal;

        //If value already calculated return the value
        if (_currentValCalculatedFlag) return _currentVal;

        //Add the node to the stack
        visitedNodes.Push(_id);

        //Node is calculated the first time
        _lastVal = _currentVal;

        _currentVal = 0;
        foreach(ConnectionGene connection in _inputs)
        {
            //Add only active connections
            if (connection.Expressed)
            {
                //Recursive call of calculate value
                _currentVal += connection.Weight * nodes[connection.InNode].CalculateValue(nodes, visitedNodes);
            }    
        }

        _currentVal = ActivationFunctionHelper.ActivationFunction(_typeOfFunction, _currentVal);
        _currentValCalculatedFlag = true;

        //Check if the removed node is really the correct node
        int poppedNode = visitedNodes.Pop();
        if (poppedNode != _id) throw new System.Exception("The popped node does not match the id! PoppedNode: " + poppedNode + ", ID: " + _id);

        return _currentVal;
    }

    /// <summary>
    /// Set the calculated flag to false.
    /// Should be used when new inputs are available
    /// </summary>
    public void ResetNode()
    {
        _currentValCalculatedFlag = false;
    }

    public void SetCurrentVal(double value)
    {
        _currentVal = value;
        _currentValCalculatedFlag = true;
    }

    #endregion

}
