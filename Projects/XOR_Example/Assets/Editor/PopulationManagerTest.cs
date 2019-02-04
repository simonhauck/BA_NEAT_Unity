using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class PopulationManagerTest  : PopulationManager.IPopulationManagerCallback{

    private PopulationManager populationManager;

    private Genome parent1Genome;
    private Genome parent2Genome;
    private Genome parent3GenomeSimple;

    private GeneCounter connectionInnovationCounter;
    private GeneCounter nodeInnovationCounter;

    //Variables to store results from the AgentsInitialied callback
    private List<AgentObject> resultAgentsInitalizedAgentList;
    private List<Species> resultAgentsInitializedSpeciesList;
    private int resultGenerationAgentsInitialized;

    //Variables to store the results from the AgentsKilled callback
    private AgentObject resultAgentKilled;
    private bool resultAllAgentsKilled;

    #region Callback Methods Populationmanager

    public void AgentsInitializedCallback(List<AgentObject> agents, List<Species> species, int generation)
    {
        resultAgentsInitalizedAgentList = agents;
        resultAgentsInitializedSpeciesList = species;
        resultGenerationAgentsInitialized = generation;
    }

    public void AllAgentsKilledCallback(List<AgentObject> agents, List<Species> species, int generation)
    {
        resultAllAgentsKilled = true;
    }

    public void AgentKilledCallback(AgentObject agent)
    {
        resultAgentKilled = agent;
    }

    public AgentObject InitNewAgent(PopulationManager populationManager, Genome genome)
    {
        return new CustomAgent(populationManager, genome, 100);
    }

    #endregion

    #region Agent class

    private class CustomAgent : AgentObject
    {
        float _fitness;

        public CustomAgent(PopulationManager evaluator, Genome genome, float fitness)
        {
            _fitness = fitness;
            InitGenome(genome, evaluator);
        }

        public override float GetFitness()
        {
            return _fitness;
        }
    }

    #endregion

    [SetUp]
    public void SetUp()
    {
        populationManager = new PopulationManager
        {
            Callback = this
        };

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
        parent2Genome.AddNodeGene(new NodeGene(6, NodeGeneType.HIDDEN, 0.5f));

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
        // Parent 3
        //-----------------------------------------------------------------------------------

        parent3GenomeSimple = new Genome();
        //Create the input nodes
        parent3GenomeSimple.AddNodeGene(new NodeGene(7, NodeGeneType.INPUT, 0f));
        parent3GenomeSimple.AddNodeGene(new NodeGene(8, NodeGeneType.INPUT, 0f));
        parent3GenomeSimple.AddNodeGene(new NodeGene(9, NodeGeneType.INPUT, 0f));

        //Create the output node
        parent3GenomeSimple.AddNodeGene(new NodeGene(10, NodeGeneType.OUTPUT, 1f));

        //Create connections
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(7, 10, 1.0, true, 11));
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(8, 10, 1.0, true, 12));
        parent3GenomeSimple.AddConnectionGene(new ConnectionGene(9, 10, 1.0, true, 13));


        //Init the connection gene counter with 11
        connectionInnovationCounter = new GeneCounter(14);
        nodeInnovationCounter = new GeneCounter(11);

        //Set result values from callback to null
        resultAgentsInitalizedAgentList = null;
        resultAgentsInitializedSpeciesList = null;
        resultAgentKilled = null;
        resultAllAgentsKilled = false;

        //Set fine tuning parameters
        PopulationManager._FACTOR_EXCESS_GENES = 1;
        PopulationManager._FACTOR_DISJOINT_GENES = 1;
        PopulationManager._FACTOR_AVG_WEIGHT_DIF = 0.4f;
        PopulationManager._THRESHOLD_NUMBER_OF_GENES = 20;
        PopulationManager._COMPABILITY_THRESHOLD = 3;
}

    [Test]
    public void CreateInitialPopulation_Test()
    {
        //Test the the reusult values
        Assert.Null(resultAgentsInitalizedAgentList);
        Assert.Null(resultAgentsInitializedSpeciesList);

        //Init population
        populationManager.CreateInitialPopulation(parent3GenomeSimple, nodeInnovationCounter, connectionInnovationCounter, 100);

        Assert.NotNull(resultAgentsInitalizedAgentList);
        Assert.NotNull(resultAgentsInitializedSpeciesList);

        //Test the amount of agents
        Assert.AreEqual(populationManager.PopulationSize, resultAgentsInitalizedAgentList.Count);
        Assert.AreEqual(1, resultAgentsInitializedSpeciesList.Count);

        int loopCounter = 0;
        foreach (AgentObject agent in resultAgentsInitalizedAgentList)
        {
            Assert.AreEqual(100, agent.GetFitness());
            Genome agentGenome = agent.Genome;

            //Should not be the same objects but contain the same values
            Assert.AreNotEqual(parent3GenomeSimple, agentGenome);
            Assert.AreEqual(parent3GenomeSimple.Nodes.Count, agentGenome.Nodes.Count);
            Assert.AreEqual(parent3GenomeSimple.Connections.Count, agentGenome.Connections.Count);

            //Test if the object in the species list is the same as the object in the agents list
            Assert.AreEqual(agent, resultAgentsInitializedSpeciesList[0].Members[loopCounter++]);

            //Test all nodes
            foreach(int nodeKey in parent3GenomeSimple.Nodes.Keys)
            {
                Assert.True(agentGenome.Nodes.ContainsKey(nodeKey));
                Assert.True(CompareNodeGenes(parent3GenomeSimple.Nodes[nodeKey], agentGenome.Nodes[nodeKey]));
            }

            //Test all connections
            foreach(int connectionKey in parent3GenomeSimple.Connections.Keys)
            {
                Assert.True(agentGenome.Connections.ContainsKey(connectionKey));
                Assert.True(CompareConnectionGenes(parent3GenomeSimple.Connections[connectionKey], agentGenome.Connections[connectionKey]));
            }
        }
    }

    [Test]
    public void IsSpeciesAvailableForAgent_Test()
    {
        //Initialize population
        populationManager.CreateInitialPopulation(parent3GenomeSimple, nodeInnovationCounter, connectionInnovationCounter, 100);

        AgentObject invalidAgent = new CustomAgent(populationManager, parent1Genome, 100);
        AgentObject invalidAgent2 = new CustomAgent(populationManager, parent2Genome, 100);
        AgentObject validAgent = new CustomAgent(populationManager, new Genome(parent3GenomeSimple), 100);

        //Return null for the given genome. Should not match
        Assert.Null(populationManager.IsSpeciesAvailableForAgent(invalidAgent, populationManager.Species));
        Assert.Null(populationManager.IsSpeciesAvailableForAgent(invalidAgent2, populationManager.Species));
        Assert.NotNull(populationManager.IsSpeciesAvailableForAgent(validAgent, populationManager.Species));
    }

    [Test]
    public void KillAgents_Test()
    {
        populationManager.CreateInitialPopulation(parent3GenomeSimple, nodeInnovationCounter, connectionInnovationCounter, 100);

        for(int i = 0; i<resultAgentsInitalizedAgentList.Count; i++)
        {
            AgentObject agent = resultAgentsInitalizedAgentList[i];
            agent.KillAgent();

            Assert.AreEqual(agent, resultAgentKilled);
            resultAgentKilled = null;
            
            //Check if the AllAgentsCallback was invoked after the last agent was killed
            if(i == resultAgentsInitalizedAgentList.Count - 1)
            {
                Assert.True(resultAllAgentsKilled);
            }
            else
            {
                Assert.False(resultAllAgentsKilled);
            }        
        }

    }

    [Test]
    public void GetBestGenomeForSpecies_Test()
    {
        //Create agents
        AgentObject agent1 = new CustomAgent(populationManager, new Genome(), 100);
        AgentObject agent2 = new CustomAgent(populationManager, new Genome(), 200);
        AgentObject agent3 = new CustomAgent(populationManager, new Genome(), 300);

        //Create two species
        Species species1 = new Species(0, agent1);
        species1.Members.Add(agent1);

        Species species2 = new Species(0, agent3);
        species2.Members.Add(agent3);

        Species species3 = new Species(0, agent2);

        for (int i = 0; i < 4; i++)
        {
            species1.Members.Add(new CustomAgent(populationManager, new Genome(), 50));
            species2.Members.Add(new CustomAgent(populationManager, new Genome(), 50));
            species3.Members.Add(new CustomAgent(populationManager, new Genome(), 50));
        }

        List<Species> species = new List<Species>() { species1, species2, species3 };

        List<Genome> bestGenomes = populationManager.GetBestGenomeForSpecies(species);

        Assert.AreEqual(2, bestGenomes.Count);
        Assert.AreEqual(agent1.Genome, bestGenomes[0]);
        Assert.AreEqual(agent3.Genome, bestGenomes[1]);
    }

    [Test]
    public void GetTotalFitnessForSpecies_Test()
    {
        //Create agents
        AgentObject agent1 = new CustomAgent(populationManager, new Genome(), 100);
        AgentObject agent2 = new CustomAgent(populationManager, new Genome(), 200);
        AgentObject agent3 = new CustomAgent(populationManager, new Genome(), 300);
        AgentObject agent4 = new CustomAgent(populationManager, new Genome(), 400);

        //Create two species
        Species species1 = new Species(0, agent1);
        species1.Members.Add(agent1);
        species1.Members.Add(agent2);
        species1.CalculateTotalSharedFitness();


        Species species2 = new Species(0, agent3);
        species2.Members.Add(agent3);
        species2.Members.Add(agent4);
        species2.CalculateTotalSharedFitness();

        //Create a list of species
        List<Species> speciesList = new List<Species>();
        speciesList.Add(species1);
        speciesList.Add(species2);
        
        

        float sum = populationManager.GetTotalFitnessForSpecies(speciesList);
        Assert.AreEqual(500, sum);
    }

    [Test]
    public void GetOrderedRandomNumberAsc_Test()
    {
        List<float> result = populationManager.GetOrderedRandomNumberAsc(0, 100f, 1000);

        Assert.AreEqual(1000, result.Count);

        float lastVal = Mathf.NegativeInfinity;
        for(int i = 0; i<result.Count; i++)
        {
            Assert.GreaterOrEqual(result[i], 0);
            Assert.LessOrEqual(result[i], 100);
            Assert.GreaterOrEqual(result[i], lastVal);
            lastVal = result[i];
        }
    }

    [Test]
    public void GetAmountOffSpringsForSpecies_Test()
    {
        //Create agents
        AgentObject agent1 = new CustomAgent(populationManager, new Genome(), 1);
        AgentObject agent2 = new CustomAgent(populationManager, new Genome(), 2);
        AgentObject agent3 = new CustomAgent(populationManager, new Genome(), 3);
        AgentObject agent4 = new CustomAgent(populationManager, new Genome(), 3);

        //Create two species
        Species species1 = new Species(0, agent1);
        species1.Members.Add(agent1);
        species1.Members.Add(agent2);
        species1.CalculateTotalSharedFitness();


        Species species2 = new Species(0, agent3);
        species2.Members.Add(agent3);
        species2.Members.Add(agent4);
        species2.CalculateTotalSharedFitness();

        //Create a list of species
        List<Species> speciesList = new List<Species>();
        speciesList.Add(species1);
        speciesList.Add(species2);

        Dictionary<Species, int> result = populationManager.GetAmountOffSpringsForSpecies(90, new List<Species>(speciesList));
        Assert.AreEqual(30, result[species1]);
        Assert.AreEqual(60, result[species2]);

    }

    [Test]
    public void GetRandomAgentsBiasFitness_Test()
    {
        //Create agents
        AgentObject agent1 = new CustomAgent(populationManager, new Genome(), 100);
        AgentObject agent2 = new CustomAgent(populationManager, new Genome(), 300);

        //Create two species
        Species species1 = new Species(0, agent1);
        species1.Members.Add(agent1);
        species1.Members.Add(agent2);

        List<AgentObject> resultAgents = populationManager.GetRandomAgentsBiasFitness(1000, species1);

        //Count values
        int amountAgents1 = 0;
        int amountAgents2 = 0;

        for(int i = 0; i < resultAgents.Count; i++)
        {
            if (resultAgents[i] == agent1) amountAgents1++;
            if (resultAgents[i] == agent2) amountAgents2++;
        }

        Assert.AreEqual(1000, amountAgents1 + amountAgents2);
        Assert.GreaterOrEqual(amountAgents2, 600);

    }

    [Ignore("Doest seem to be importent")]
    public void CrossOverAgent_Test()
    {

    }

    [Test]
    public void MutateAgent_Weight_Test()
    {
        MutationLog mutationLog = new MutationLog();

        AgentObject customAgent = new CustomAgent(populationManager, parent2Genome, 10);
        Assert.AreEqual(6, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(9, customAgent.Genome.Connections.Count);

        Dictionary<int, double> weights = new Dictionary<int, double>();
        foreach(int key in customAgent.Genome.Connections.Keys)
        {
            weights.Add(key, customAgent.Genome.Connections[key].Weight);
        }

        //Mutate weights
        populationManager.MutateAgent(customAgent, 0f, 0f, 1f, nodeInnovationCounter, connectionInnovationCounter, mutationLog);

        //Nodes should not have changed
        Assert.AreEqual(6, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(9, customAgent.Genome.Connections.Count);
        Assert.AreEqual(14, connectionInnovationCounter.GetNewNumber());
        Assert.AreEqual(11, nodeInnovationCounter.GetNewNumber());


        foreach(int key in weights.Keys)
        {
            Assert.AreNotEqual(weights[key], customAgent.Genome.Connections[key].Weight);
        }
    }

    [Test]
    public void MutateAgent_Connection_Test()
    {
        MutationLog mutationLog = new MutationLog();

        AgentObject customAgent = new CustomAgent(populationManager, parent2Genome, 10);
        Assert.AreEqual(6, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(9, customAgent.Genome.Connections.Count);

        Dictionary<int, double> weights = new Dictionary<int, double>();
        foreach (int key in customAgent.Genome.Connections.Keys)
        {
            weights.Add(key, customAgent.Genome.Connections[key].Weight);
        }

        //Mutate connections, test multiple times, because there can be invalid connecionts
        for(int i = 0; i<100; i++)
        {
            populationManager.MutateAgent(customAgent, 1f, 0f, 0f, nodeInnovationCounter, connectionInnovationCounter, mutationLog);

            //If a connection occurs, leave the loop
            if (customAgent.Genome.Connections.Count >= 10) break;
        }
        
        //Nodes should not have changed
        Assert.AreEqual(6, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(11, nodeInnovationCounter.GetNewNumber());

        Assert.AreEqual(10, customAgent.Genome.Connections.Count);
        Assert.AreEqual(15, connectionInnovationCounter.GetNewNumber());

        //WEights should not have changed
        foreach (int key in weights.Keys)
        {
            Assert.AreEqual(weights[key], customAgent.Genome.Connections[key].Weight);
        }

    }

    [Test]
    public void MutateAgent_Node_Test()
    {
        MutationLog mutationLog = new MutationLog();

        AgentObject customAgent = new CustomAgent(populationManager, parent2Genome, 10);
        Assert.AreEqual(6, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(9, customAgent.Genome.Connections.Count);

        Dictionary<int, double> weights = new Dictionary<int, double>();
        foreach (int key in customAgent.Genome.Connections.Keys)
        {
            weights.Add(key, customAgent.Genome.Connections[key].Weight);
        }

        //Can fail sometimes, when a disabled connection is selected
        populationManager.MutateAgent(customAgent, 0f, 1f, 0f, nodeInnovationCounter, connectionInnovationCounter, mutationLog);

        //1 Node shoudl be added and 2 connections
        Assert.AreEqual(7, customAgent.Genome.Nodes.Count);
        Assert.AreEqual(12, nodeInnovationCounter.GetNewNumber());

        Assert.AreEqual(11, customAgent.Genome.Connections.Count);
        Assert.AreEqual(16, connectionInnovationCounter.GetNewNumber());

        //WEights should not have changed
        foreach (int key in weights.Keys)
        {
            Assert.AreEqual(weights[key], customAgent.Genome.Connections[key].Weight);
        }
    }

    [Test]
    public void PlaceAgentInSpecies_Test()
    {
        AgentObject agent1 = new CustomAgent(populationManager, parent1Genome, 1);
        AgentObject agent2 = new CustomAgent(populationManager, parent1Genome, 2);

        AgentObject agent3 = new CustomAgent(populationManager, parent2Genome, 3);
        AgentObject agent4 = new CustomAgent(populationManager, parent2Genome, 4);

        AgentObject agent5 = new CustomAgent(populationManager, parent3GenomeSimple, 5);

        Species species1 = new Species(0, agent1);
        Species species2 = new Species(0, agent5);

        List<AgentObject> agents = new List<AgentObject> { agent1, agent2, agent3, agent4 };
        List<Species> species = new List<Species> { species1, species2 };

        //Species one should be updated,
        //Species two should be deleted
        //For agent 3 and 4 is a new species required
        populationManager.PlaceAgentInSpecies(agents, species);

        Assert.AreEqual(2, species.Count);
        Assert.AreEqual(species1, species[0]);
        Assert.AreNotEqual(species2, species[1]);

        //Check agents
        Assert.AreEqual(agent1, species[0].RepresentiveAgent);
        Assert.AreEqual(agent3, species[1].RepresentiveAgent);

        Assert.AreEqual(2, species[0].Members.Count);
        Assert.AreEqual(2, species[1].Members.Count);

        Assert.AreEqual(agent1, species[0].Members[0]);
        Assert.AreEqual(agent2, species[0].Members[1]);

        Assert.AreEqual(agent3, species[1].Members[0]);
        Assert.AreEqual(agent4, species[1].Members[1]);
    }

    [Test]
    public void GenerateNextGeneration_Test()
    {
        populationManager.CreateInitialPopulation(parent3GenomeSimple, nodeInnovationCounter, connectionInnovationCounter, 500);
        Assert.NotNull(resultAgentsInitalizedAgentList);
        Assert.NotNull(resultAgentsInitializedSpeciesList);

        //Clear the list to prepare the next generation
        resultAgentsInitalizedAgentList = null;
        resultAgentsInitializedSpeciesList = null;

        populationManager.GenerateNextGeneration();
        Assert.NotNull(resultAgentsInitalizedAgentList);
        Assert.NotNull(resultAgentsInitializedSpeciesList);
        Assert.AreEqual(500, resultAgentsInitalizedAgentList.Count);
        Assert.AreEqual(1, resultGenerationAgentsInitialized);

        //Clear the list to prepare the next generation
        resultAgentsInitalizedAgentList = null;
        resultAgentsInitializedSpeciesList = null;

        populationManager.GenerateNextGeneration();
        Assert.NotNull(resultAgentsInitalizedAgentList);
        Assert.NotNull(resultAgentsInitializedSpeciesList);
        Assert.AreEqual(500, resultAgentsInitalizedAgentList.Count);
        Assert.AreEqual(2, resultGenerationAgentsInitialized);
    }

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
