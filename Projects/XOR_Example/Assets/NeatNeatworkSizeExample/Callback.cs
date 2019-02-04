using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour, PopulationManager.IPopulationManagerCallback
{

    public float targetFitness;
    public PopulationManager manager = new PopulationManager();

    public GeneCounter connectionCounter;
    public GeneCounter nodeCounter;

    public List<AgentObject> _agents;
    public bool _newAgentsAvailable = false;
    public bool _startNewGen = false;

    public void AgentKilledCallback(AgentObject agent)
    {

    }

    public void AgentsInitializedCallback(List<AgentObject> agents, List<Species> species, int generation)
    {
        _agents = agents;
        _newAgentsAvailable = true;

    }

    public void AllAgentsKilledCallback(List<AgentObject> agents, List<Species> species, int generation)
    {
        Debug.Log("Generation " + generation + " finished");
        Debug.Log("Amount Agents: " + agents.Count + ", Amount Species: " + species.Count);

        float bestWeightDiff = 100;
        float averageWeightDiff = 0;

        float bestFitness = 0;

        foreach (AgentObject agent in agents)
        {
            float weightSum = 0;
            foreach (ConnectionGene con in agent.Genome.Connections.Values)
            {
                if (con.Expressed)
                {
                    weightSum += (float)con.Weight;
                }
                
            }
            float diff = Mathf.Abs(targetFitness - weightSum);

            if (diff < bestWeightDiff) bestWeightDiff = diff;
            if (agent.GetFitness() > bestFitness) bestFitness = agent.GetFitness();
            averageWeightDiff += diff;
        }

        averageWeightDiff /= agents.Count;

        Debug.Log("Best Diff: " + bestWeightDiff + " Average:" + averageWeightDiff + " Best Fitness: " + bestFitness);

        //Start next+
        _startNewGen = true;
        manager.GenerateNextGeneration();
    }

    public AgentObject InitNewAgent(PopulationManager populationManager, Genome genome)
    {
        return new TestAgent(populationManager, genome, targetFitness);
    }

    // Use this for initialization
    void Start()
    {
        Genome parent3GenomeSimple = new Genome();
        //Create the input nodes
        parent3GenomeSimple.AddNodeGene(new NodeGene(1, NodeGeneType.INPUT, 0f));
        parent3GenomeSimple.AddNodeGene(new NodeGene(2, NodeGeneType.INPUT, 0f));
        parent3GenomeSimple.AddNodeGene(new NodeGene(3, NodeGeneType.INPUT, 0f));

        //Create the output node
        parent3GenomeSimple.AddNodeGene(new NodeGene(4, NodeGeneType.OUTPUT, 1f));

        //Create connections
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(1, 4, 1.0, true, 1));
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(2, 4, 1.0, true, 2));
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(3, 4, 1.0, true, 3));


        //Init the connection gene counter with 11
        connectionCounter = new GeneCounter(4);
        nodeCounter = new GeneCounter(5);

        manager.Callback = this;

        manager.CreateInitialPopulation(parent3GenomeSimple, nodeCounter, connectionCounter, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (_newAgentsAvailable)
        {
            foreach (AgentObject agent in _agents)
            {
                TestAgent castedAgent = (TestAgent)agent;
                castedAgent.CalcualteFitness();
                castedAgent.KillAgent();
            }
        }
        else if (_startNewGen)
        {
            _startNewGen = false;
           
        }
    }
}
