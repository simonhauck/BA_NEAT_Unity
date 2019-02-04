using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public class XOR_Callback : MonoBehaviour, PopulationManager.IPopulationManagerCallback
    {
        public int amountRuns = 100;

        //Prefab for xor calculation
        public GameObject _xorPrefab;

        //the screen
        public GameObject _screen;
        public GameObject _screenObjPrefab;

        //Display neuronal net
        public GameObject _neuronalNetDisplayPrefab;

        private PopulationManager _manager;

        private Genome startingGenome;

        private List<int> result;

        // Use this for initialization
        void Start()
        {
            startingGenome = new Genome();
            startingGenome.AddNodeGene(new NodeGene(0, NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
            startingGenome.AddNodeGene(new NodeGene(1, NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
            startingGenome.AddNodeGene(new NodeGene(2, NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
            startingGenome.AddNodeGene(new NodeGene(3, NodeGeneType.OUTPUT, 1f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));

            
            startingGenome.AddConnectionGene(new ConnectionGene(0, 3, Random.Range(0f, 1f), true, 0));
            startingGenome.AddConnectionGene(new ConnectionGene(1, 3, Random.Range(0f, 1f), true, 1));
            startingGenome.AddConnectionGene(new ConnectionGene(2, 3, Random.Range(0f, 1f), true, 2));
            


            //Nodes for a complete network

            /*
            startingGenome.AddNodeGene(new NodeGene(4, NodeGeneType.HIDDEN, 0.5f));
            startingGenome.AddNodeGene(new NodeGene(5, NodeGeneType.HIDDEN, 0.5f));

            startingGenome.AddConnectionGene(new ConnectionGene(0, 4, Random.Range(0f, 1f), true, 0));
            startingGenome.AddConnectionGene(new ConnectionGene(0, 5, Random.Range(0f, 1f), true, 1));
            startingGenome.AddConnectionGene(new ConnectionGene(1, 4, Random.Range(0f, 1f), true, 2));
            startingGenome.AddConnectionGene(new ConnectionGene(1, 5, Random.Range(0f, 1f), true, 3));

            startingGenome.AddConnectionGene(new ConnectionGene(4, 3, Random.Range(0f, 1f), true, 4));
            startingGenome.AddConnectionGene(new ConnectionGene(5, 3, Random.Range(0f, 1f), true, 5));

            startingGenome.AddConnectionGene(new ConnectionGene(2, 3, Random.Range(0f, 1f), true, 6));
            startingGenome.AddConnectionGene(new ConnectionGene(2, 4, Random.Range(0f, 1f), true, 7));
            startingGenome.AddConnectionGene(new ConnectionGene(2, 5, Random.Range(0f, 1f), true, 8));
            */

            result = new List<int>();

            _manager = new PopulationManager();
            _manager.Callback = this;
            _manager.CreateInitialPopulation(startingGenome, new GeneCounter(6), new GeneCounter(9), 400, true);

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region PopulationManager Callbacks

        public void AgentKilledCallback(AgentObject agent)
        {

        }

        public void AgentsInitializedCallback(List<AgentObject> agents, List<Species> species, int generation)
        {
            Debug.Log("All agents initalized. Generation: " + generation + ", Amount: " + agents.Count + ", Species: " + species.Count);
        }

        public void AllAgentsKilledCallback(List<AgentObject> agents, List<Species> species, int generation)
        {

            //Destry all player
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
            {
                Destroy(gameObject);
            }

            GenerationEvaluator eval = new GenerationEvaluator(agents, species, generation);
            AgentObject bestAgent = eval.BestAgent;

            Debug.Log("All agents dead. Best fitness: " + bestAgent.GetFitness()+", Average: "+eval.AverageFitness);

            //Fill the screen
            FillXorScreen(bestAgent.Genome, _screen, _screenObjPrefab);

            SetDisplay(bestAgent.Genome);

            if (CanAgentSolveXOR(bestAgent) || eval.Generation > 500)
            {


                foreach (NodeGene node in bestAgent.Genome.Nodes.Values)
                {
                    Debug.Log("Node: " + node.ID + ", Type: " + node.Type);
                }

                foreach (ConnectionGene connection in bestAgent.Genome.Connections.Values)
                {
                    if (connection.Expressed)
                    {
                        Debug.Log("Connection: In: " + connection.InNode + " Out: " + connection.OutNode + ", Weight: " + connection.Weight + ", InnovationNumber: " + connection.InnovationNumber);
                    }
                }

                result.Add(eval.Generation);

                if(result.Count < amountRuns)
                {
                    _manager.CreateInitialPopulation(startingGenome, new GeneCounter(6), new GeneCounter(9), 400);
                }
                else
                {
                    Debug.Log("FINIIIISHED");
                }

            }
            else
            {
                _manager.GenerateNextGeneration();
            }


        }

        public AgentObject InitNewAgent(PopulationManager populationManager, Genome genome)
        {
            float xPos = Random.Range(-20f, 20f);
            float yPos = Random.Range(-20f, 20f);

            GameObject obj = Instantiate(_xorPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
            obj.GetComponent<XOR_Agent>()._xorAgent = new XOR_Agent.XOR(genome, populationManager);

            return obj.GetComponent<XOR_Agent>()._xorAgent;
        }

        public bool CanAgentSolveXOR(AgentObject agent)
        {
            double result1 = agent.Genome.Calculate(new double[] { 0, 0, 1 })[0];
            double result2 = agent.Genome.Calculate(new double[] { 0, 1, 1 })[0];
            double result3 = agent.Genome.Calculate(new double[] { 1, 0, 1 })[0];
            double result4 = agent.Genome.Calculate(new double[] { 1, 1, 1 })[0];

            if (result1 > 0.5) return false;
            if (result2 < 0.5) return false;
            if (result3 < 0.5) return false;
            if (result4 > 0.5) return false;

            return true;
        }

        public void FillXorScreen(Genome genome, GameObject parent, GameObject screenPrefab)
        {
            foreach(Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i <= 10; i++)
            {
                for(int j = 0; j<= 10; j++)
                {
                    double xVal = (double) i / (double)10;
                    double yVal = (double) j / (double)10;

                    GameObject obj = Instantiate(screenPrefab, parent.transform, false);
                    obj.transform.localPosition = new Vector3(i, j, 0);

                    //Get values for xor
                    float result = (float) genome.Calculate(new double[] { xVal, yVal, 1 })[0];

                    obj.GetComponent<Renderer>().material.color = new Color(result, result, result);
                    
                }
            }
        }

        public void SetDisplay(Genome genome)
        {
            GameObject[] oldDisplays = GameObject.FindGameObjectsWithTag("NeuronalNetDisplay");
            foreach(GameObject old in oldDisplays)
            {
                Destroy(old);
            }

            GameObject display = Instantiate(_neuronalNetDisplayPrefab, new Vector3(70, -20, 0), Quaternion.identity);
            display.GetComponent<NeuronalNetworkDisplay>().DisplayNetwork(genome);
        }





        #endregion
    }
}

