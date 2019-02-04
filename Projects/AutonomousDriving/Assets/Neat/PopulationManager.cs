using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager
{

    #region Fine Tuning Parameters

    //Parameters for the compability distance function
    public static float _FACTOR_EXCESS_GENES = 1f;
    public static float _FACTOR_DISJOINT_GENES = 1f;
    public static float _FACTOR_AVG_WEIGHT_DIF = 0.8f;
    public static float _THRESHOLD_NUMBER_OF_GENES = 20;
    public static float _COMPABILITY_THRESHOLD = 3f;

    //Parameters for mutation
    public static float _MUTATE_WEIGHTS_CHANCE = 0.8f;
    public static float _ADD_CONNECTION_MUTATION_CHANCE = 0.05f;
    public static float _ADD_NODE_MUTATION_CHANCE = 0.03f;
    public static int _AMOUNT_MUTATION_TRIES = 20;
    public static int _SIZE_THRESHOLD = 9;

    #endregion

    #region Propeties

    public List<AgentObject> Agents { get { return _agents; } }
    public List<Species> Species { get { return _species; } }

    public int PopulationSize { get { return _populationSize; } set { _populationSize = value; } }

    public IPopulationManagerCallback Callback { get { return _callback; } set { _callback = value; } }

    #endregion

    #region Private fields

    private int _populationSize;
    private int _generation;

    private IPopulationManagerCallback _callback;

    private List<AgentObject> _agents;
    private List<Species> _species;

    private GeneCounter _nodeInnovationCounter;
    private GeneCounter _connectionInnovationCounter;

    private MutationLog _mutationLog;

    #endregion

    #region Public methods

    /// <summary>
    /// Create an initial population of agent objects. The results will be published ti the IPopulationManagerCallback
    /// </summary>
    /// <param name="startingGenome">The original genome. The genome will be cloned to match the population size</param>
    /// <param name="nodeInnovationCounter">the initialized counter for the node innovation</param>
    /// <param name="connectionInnovationCounter">the initalized counter for the connections</param>
    /// <param name="populationSize">The amount of agents created for each generation</param>
    public void CreateInitialPopulation(Genome startingGenome, GeneCounter nodeInnovationCounter, GeneCounter connectionInnovationCounter, int populationSize)
    {
        _generation = 0;
        _nodeInnovationCounter = nodeInnovationCounter;
        _connectionInnovationCounter = connectionInnovationCounter;
        _populationSize = populationSize;
        _mutationLog = new MutationLog();

        _agents = new List<AgentObject>();
        _species = new List<Species>();

        for (int i = 0; i < _populationSize; i++)
        {
            Genome randomGenome = new Genome(startingGenome);

            AgentObject agent = _callback.InitNewAgent(this, randomGenome);
            _agents.Add(agent);
        }

        PlaceAgentInSpecies(_agents, _species);

        //Notify the callback if not existing
        if (_callback != null) _callback.AgentsInitializedCallback(_agents, _species, _generation);

    }

    public void CreateInitialPopulation(Genome startingGenome, GeneCounter nodeInnovationCounter, GeneCounter connectionInnovationCounter, int populationSize, bool randomizeWeights)
    {
        _generation = 0;
        _nodeInnovationCounter = nodeInnovationCounter;
        _connectionInnovationCounter = connectionInnovationCounter;
        _populationSize = populationSize;
        _mutationLog = new MutationLog();

        _agents = new List<AgentObject>();
        _species = new List<Species>();

        for (int i = 0; i < _populationSize; i++)
        {
            Genome randomGenome = new Genome(startingGenome);

            if (randomizeWeights)
            {
                foreach (ConnectionGene con in randomGenome.Connections.Values)
                {
                    con.Weight = Random.Range(-1f, 1f);
                }
            }

            AgentObject agent = _callback.InitNewAgent(this, randomGenome);
            _agents.Add(agent);
        }

        PlaceAgentInSpecies(_agents, _species);

        //Notify the callback if not existing
        if (_callback != null) _callback.AgentsInitializedCallback(_agents, _species, _generation);
    }

    /// <summary>
    /// Check if a species is for the given agent available
    /// </summary>
    /// <param name="agent">The agent that should be checked</param>
    /// <param name="species">List of species that will be checked</param>
    /// <returns>The first matching species or null if no species was found</returns>
    public Species IsSpeciesAvailableForAgent(AgentObject agent, List<Species> species)
    {
        foreach (Species s in species)
        {
            float distanceValue = Genome.CompabilityDistanceFunction(s.RepresentiveAgent.Genome, agent.Genome, _FACTOR_EXCESS_GENES, _FACTOR_DISJOINT_GENES, _FACTOR_AVG_WEIGHT_DIF, _THRESHOLD_NUMBER_OF_GENES);

            if (distanceValue <= _COMPABILITY_THRESHOLD)
            {
                //Species found
                return s;
            }
        }
        return null;
    }

    /// <summary>
    /// Called when an agent dies. The Manager will check if there are active agents left.
    /// The AgentKilled() method in the IPopulationManagerCallback will be called, and the llAgentsKilledCallback() method
    /// </summary>
    public void AgentKilled(AgentObject killedAgent)
    {
        //Notify the callback
        _callback.AgentKilledCallback(killedAgent);

        //Check for remaining agents
        foreach (AgentObject agent in _agents)
        {
            //Break
            if (agent.Active) return;
        }
        _callback.AllAgentsKilledCallback(_agents, _species, _generation);
    }

    public void GenerateNextGeneration()
    {
        //List for new agents
        List<AgentObject> newGeneration = new List<AgentObject>();

        //Put best genomes in new generation without mutation
        List<Genome> bestGenomes = GetBestGenomeForSpecies(_species);
        foreach (Genome g in bestGenomes)
        {
            Genome copiedGenome = new Genome(g);
            AgentObject newAgent = _callback.InitNewAgent(this, copiedGenome);
            newGeneration.Add(newAgent);
        }

        //Place species that will be evaluated in the next generation here
        List<Species> speciesForNextGeneration = new List<Species>();

        //Set highest fitness and calculate shared fitness
        foreach(Species s in _species)
        {
            s.SortMembersByFitness();
            s.CalculateTotalSharedFitness();
            s.SetBestFitness();
            s.SelectNewRandomRepresentiveAgent();
            s.RemoveHalf();

            if (s.IsActive())
            {
                s.IncreaseGeneration();
                speciesForNextGeneration.Add(s);
            }
        }

        
        for(int i = _species.Count -1; i >= 0; i--)
        {
            if (speciesForNextGeneration.Count < 10)
            {
                Species s = _species[i];
                if (!speciesForNextGeneration.Contains(s))
                {
                    speciesForNextGeneration.Add(s);
                }
            }
            else
            {
                break;
            }
        }
        

        //Create remaing Offsprings by Crossover
        int agentsToCreate = _populationSize - newGeneration.Count;
        Dictionary<Species, int> randomSpecies = GetAmountOffSpringsForSpecies(agentsToCreate, speciesForNextGeneration);

        foreach (Species s in randomSpecies.Keys)
        {
            //Get twice as much agents as aprents for the crossover
            List<AgentObject> parentAgents = GetRandomAgentsBiasFitness(randomSpecies[s] * 2, s);
            for (int i = 0; i < parentAgents.Count; i = i + 2)
            {
                AgentObject moreFitParent;
                AgentObject lessFitParent;
                if (parentAgents[i].GetFitness() >= parentAgents[i + 1].GetFitness())
                {
                    moreFitParent = parentAgents[i];
                    lessFitParent = parentAgents[i + 1];
                }
                else
                {
                    moreFitParent = parentAgents[i + 1];
                    lessFitParent = parentAgents[i];
                }

                //Create new Agent
                AgentObject child = CrossOverAgent(moreFitParent, lessFitParent);

                MutateAgent(child, _ADD_CONNECTION_MUTATION_CHANCE, _ADD_NODE_MUTATION_CHANCE, _MUTATE_WEIGHTS_CHANCE, _nodeInnovationCounter, _connectionInnovationCounter, _mutationLog);
                newGeneration.Add(child);
            }
        }

        //Place the agents in the species
        PlaceAgentInSpecies(newGeneration, _species);
        _agents = newGeneration;

        _generation++;
        _callback.AgentsInitializedCallback(_agents, _species, _generation);
    }

    /// <summary>
    /// Return a list with the best genomes in each species
    /// </summary>
    /// <param name="species">the list of species that will be searched</param>
    /// <returns>the List with the best genomes</returns>
    public List<Genome> GetBestGenomeForSpecies(List<Species> species)
    {
        AgentObject bestAgent = null;

        List<Genome> bestGenomes = new List<Genome>();
        foreach (Species s in species)
        {
            s.SortMembersByFitness();

            AgentObject agent = s.Members[0];

            if (bestAgent == null || bestAgent.GetFitness() <= agent.GetFitness())
            {
                bestAgent = agent;
            }

            // species with less than 5 members can not save reproduce
            if (s.Members.Count < 5) continue;

            if (agent == null) throw new System.Exception("Agent is null!");

            bestGenomes.Add(agent.Genome);
        }

        //If the best agent is not yet in the list (because the species has less then 5 members) add the agenbt
        if (!bestGenomes.Contains(bestAgent.Genome)) bestGenomes.Add(bestAgent.Genome);

        return bestGenomes;
    }

    /// <summary>
    /// Return the sum of the totalSharedFitness of the species list.
    /// </summary>
    /// <param name="species">species which totalSharedFitness should be summed</param>
    /// <returns>the sum of totalSharedFitness</returns>
    public float GetTotalFitnessForSpecies(List<Species> species)
    {
        //TODO test
        float sum = 0;
        foreach (Species s in species)
        {
            sum += s.TotalSharedFitness;
        }
        return sum;
    }

    /// <summary>
    /// Return a list of random numbers
    /// </summary>
    /// <param name="min">Min value [inclusive]</param>
    /// <param name="max">Max value [inclusive]</param>
    /// <param name="amountOfNumbers">amount of results</param>
    /// <returns>list with random numbers in ascending order</returns>
    public List<float> GetOrderedRandomNumberAsc(float min, float max, int amountOfNumbers)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < amountOfNumbers; i++)
        {
            result.Add(Random.Range(min, max));
        }

        result = result.OrderBy(x => x).ToList();
        return result;
    }

    //TODO Test
    /// <summary>
    /// Return the amount of offSprings for each species based on the fitness value
    /// </summary>
    /// <param name="amountOffSpring">the total amount of offsprings in all species combined</param>
    /// <param name="species">the list of species</param>
    /// <returns>dictionary with species as a key and the value of offsprings</returns>
    public Dictionary<Species, int> GetAmountOffSpringsForSpecies(int amountOffSpring, List<Species> species)
    {
        Dictionary<Species, int> result = new Dictionary<Species, int>();
        float totalFitnessSum = GetTotalFitnessForSpecies(species);

        int generatedChilds = 0;
        foreach(Species s in species)
        {
            int amountForSpecies = Mathf.FloorToInt((s.TotalSharedFitness / totalFitnessSum) * (float)amountOffSpring);
            result.Add(s, amountForSpecies);
            generatedChilds += amountForSpecies;
        }

        //If not all required childs were created, place the last randomly in a species
        for(int i = generatedChilds; i<amountOffSpring; i++)
        {
            Species selected = species[Random.Range(0, species.Count)];
            result[selected] = result[selected] + 1;
        }

        return result;
    }

    /// <summary>
    /// Get a list of agent objects based on their shared fitness from the species.
    /// THe list can contain an agent multiple times
    /// </summary>
    /// <param name="amountAgentsInResult">the amount of agents that should be selected</param>
    /// <param name="species">The species from which the agent should be selected</param>
    /// <returns>a list with agent objects</returns>
    public List<AgentObject> GetRandomAgentsBiasFitness(int amountAgentsInResult, Species species)
    {
        float totalFitnessSum = 0;
        foreach(AgentObject member in species.Members)
        {
            totalFitnessSum += member.GetFitness();
        }

        //Get Random values
        List<float> randomNumbers = GetOrderedRandomNumberAsc(0f, totalFitnessSum, amountAgentsInResult);

        List<AgentObject> result = new List<AgentObject>();
        float sumFitness = 0;
        int indexRandomNumber = 0;

        foreach (AgentObject agent in species.Members)
        {
            sumFitness += agent.GetFitness();
            bool checkNextNumber = true;
            while (checkNextNumber && indexRandomNumber < amountAgentsInResult)
            {
                if (randomNumbers[indexRandomNumber] <= sumFitness)
                {
                    indexRandomNumber++;
                    result.Add(agent);
                }
                else
                {
                    checkNextNumber = false;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Createa new agent with a new genome that is created by crossover of the two parents
    /// </summary>
    /// <param name="moreFitParent">the more fit parent</param>
    /// <param name="lessFitParent">the less fit parent</param>
    /// <returns>the newly created agent</returns>
    public AgentObject CrossOverAgent(AgentObject moreFitParent, AgentObject lessFitParent)
    {
        Genome childGenome = Genome.CrossOver(moreFitParent.Genome, lessFitParent.Genome);
        AgentObject child = _callback.InitNewAgent(this, childGenome);
        return child;
    }

    /// <summary>
    /// Mutate the given agent with different probabilities
    /// </summary>
    /// <param name="agent">The agent that should be mutated</param>
    /// <param name="addConnectionMutationChance">chance to add a connection. Should be between 0f and 1f</param>
    /// <param name="addNodeMutationChance">chance to add a node. Should be between 0f and 1f</param>
    /// <param name="mutationWeightChance">chance to mutate the weights. Should be between 0f and 1f</param>
    /// <param name="nodeInnovationCounter">a node innovation counter</param>
    /// <param name="connectionInnovationCounter">a connection innovation counter</param>
    public void MutateAgent(AgentObject agent,
        float addConnectionMutationChance,
        float addNodeMutationChance,
        float mutationWeightChance,
        GeneCounter nodeInnovationCounter, GeneCounter connectionInnovationCounter, MutationLog mutationLog)
    {
        //Mutate weights
        if (Random.Range(0f, 1f) <= mutationWeightChance)
        {
            agent.Genome.MutateConnectionWeight();
        }

        //Mutate Node
        if (Random.Range(0f, 1f) <= addNodeMutationChance)
        {
            AddNodeMutation(agent, _AMOUNT_MUTATION_TRIES, mutationLog, connectionInnovationCounter, nodeInnovationCounter);
        }

        //Mutate Connection
        if (Random.Range(0f, 1f) <= addConnectionMutationChance)
        {
            AddConnectionMutation(agent, _AMOUNT_MUTATION_TRIES, mutationLog, connectionInnovationCounter);
        }
    }

    /// <summary>
    /// Place the agents in species. If a species is available, the agent will be added. If no species is matching
    /// a new Species will be created an the agent will be set as representive member.
    /// Before the Agents are placed in the species, the species will be resetted.
    /// A species will be deleted, if no member is in it
    /// </summary>
    /// <param name="agents">the agents that should be placed</param>
    /// <param name="species">a list with species</param>
    public void PlaceAgentInSpecies(List<AgentObject> agents, List<Species> species)
    {
        foreach (Species s in species)
        {
            s.ResetSpecies();
        }

        foreach (AgentObject agent in agents)
        {
            Species availableSpecies = IsSpeciesAvailableForAgent(agent, species);
            if (availableSpecies != null)
            {
                availableSpecies.Members.Add(agent);
            }
            else
            {
                Species newSpecies = new Species(0, agent);
                newSpecies.Members.Add(agent);
                species.Add(newSpecies);
            }
        }

        //Remove empty species
        species.RemoveAll(x => x.Members.Count == 0);
    }

    public void AddConnectionMutation(AgentObject agent, int amountTries, MutationLog mutationLog, GeneCounter connectionInnovationCounter)
    {
        List<int> nodeKeys = agent.Genome.Nodes.Keys.ToList();
        for (int i = 0; i < amountTries; i++)
        {
            //Selet two random nodes
            NodeGene inNode = agent.Genome.Nodes[nodeKeys[Random.Range(0, nodeKeys.Count)]];
            NodeGene outNode = agent.Genome.Nodes[nodeKeys[Random.Range(0, nodeKeys.Count)]];

            //If mutation is possible, mutate the agent
            if (agent.Genome.IsConnectionPossible(inNode, outNode))
            {
                int innovationNumber = mutationLog.GetConnectionInnovationNumber(inNode.ID, outNode.ID, connectionInnovationCounter);
                agent.Genome.AddConnectionMutation(inNode, outNode, innovationNumber);

                break;
            }

        }
    }

    public void AddNodeMutation(AgentObject agent, int amountTries, MutationLog mutationLog, GeneCounter connectionInnovationCounter, GeneCounter nodeCounter)
    {
        List<int> connectionKeys = agent.Genome.Connections.Keys.OrderBy(x=>x).ToList();
        if (connectionKeys.Count <= 0) return;

        for (int i = 0; i < amountTries; i++)
        {
            ConnectionGene connection = null;
            //Check the size of the genome, if the genome is small, take an older gene
            if (agent.Genome.Nodes.Keys.Count <= _SIZE_THRESHOLD)
            {
                connection = agent.Genome.Connections[connectionKeys[Random.Range(0, Mathf.RoundToInt(connectionKeys.Count - (Mathf.Sqrt(connectionKeys.Count))))]];
            }
            else
            {
                //Select random connection
                 connection = agent.Genome.Connections[connectionKeys[Random.Range(0, connectionKeys.Count)]];
            }        

            //If mutation is possible, mutate the agent
            if (agent.Genome.IsNodePossible(connection))
            {
                //int nodeID = mutationLog.GetNodeID(connection.InnovationNumber, nodeCounter);
                int nodeID = mutationLog.GetNodeID(connection.InnovationNumber, agent.Genome.Nodes.Keys.ToList(), nodeCounter);

                int innovationNumberInToNewNode = mutationLog.GetConnectionInnovationNumber(connection.InNode, nodeID, connectionInnovationCounter);
                int innovationNumberNewNodeToOut = mutationLog.GetConnectionInnovationNumber(nodeID, connection.OutNode, connectionInnovationCounter);

                agent.Genome.AddNodeMutation(connection, nodeID, innovationNumberInToNewNode, innovationNumberNewNodeToOut);
                break;
            }
        }
    }

    #endregion

    #region Private methods



    #endregion

    #region Callback

    public interface IPopulationManagerCallback
    {

        /// <summary>
        /// Create a new Agent object with your custom class.
        /// </summary>
        /// <param name="populationManager">That must be set in the constructor</param>
        /// <param name="genome">The genome that must be set in the constructor</param>
        /// <returns>the initialized agent object</returns>
        AgentObject InitNewAgent(PopulationManager populationManager, Genome genome);

        void AgentsInitializedCallback(List<AgentObject> agents, List<Species> species, int generation);

        void AgentKilledCallback(AgentObject agent);

        void AllAgentsKilledCallback(List<AgentObject> agents, List<Species> species, int generation);

    }

    #endregion
}
