using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronalNetworkDisplay : MonoBehaviour
{

    public float _xSize;
    public float _ySize;

    public GameObject _nodePrefab;
    public GameObject _connectionPrefab;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayNetwork(Genome genome)
    {
        SetYValuesInGenome(genome);
        DisplayNodes(genome.Nodes.Values.ToList());
        DisplayConnections(genome);
    }

    public void SetYValuesInGenome(Genome genome)
    {
        foreach (NodeGene node in genome.Nodes.Values)
        {
            if (node._yValue == -1)
            {
                List<NodeGene> nodesWithSameX = genome.Nodes.Values.Where(x => node.XValue == x.XValue).OrderBy(x => x.ID).ToList();
                int amount = nodesWithSameX.Count;

                for(int i = 0; i<= amount-1; i++) {
                    //Make the nodes centered
                    float step = (float) 1 / (amount + 1);
                    float yVal = (i + 1) * step;
                    nodesWithSameX[i]._yValue = yVal;
                }
            }
        }
    }

    public void DisplayNodes(List<NodeGene> nodes)
    {
        foreach(NodeGene node in nodes)
        {
            GameObject obj = Instantiate(_nodePrefab, this.transform, false);
            obj.transform.localPosition = new Vector3(_xSize * node.XValue, _ySize * node._yValue, 0);
            obj.GetComponent<Display>().SetText(node.ID.ToString());
        }
    }


    public void DisplayConnections(Genome genome)
    {
        foreach(ConnectionGene connection in genome.Connections.Values)
        {
            if (connection.Expressed)
            {
                NodeGene inNode = genome.Nodes[connection.InNode];
                NodeGene outNode = genome.Nodes[connection.OutNode];

                Vector3 startVector = new Vector3(inNode.XValue * _xSize, inNode._yValue * _ySize, 0);
                Vector3 endVector = new Vector3(outNode.XValue * _xSize, outNode._yValue * _ySize, 0);

                GameObject obj = Instantiate(_connectionPrefab, this.transform, false);
                obj.GetComponent<ConnectionDisplay>().SetConnection(startVector, endVector, connection.Weight);
            }
        }
    }
}
