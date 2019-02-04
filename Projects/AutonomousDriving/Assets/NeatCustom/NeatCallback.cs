using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeatCallback : MonoBehaviour, PopulationManager.IPopulationManagerCallback
{

    public int _generationSize;
    public GameObject _playerPrefab;

    public Transform _spawnPosition;

    private bool _evaluationRunning = false;

    private PopulationManager _manager;

    private GeneCounter _nodeCounter;
    private GeneCounter _connectionCounter;
    private Genome _startGenome;

    //GUI
    private GUIStyle _guiStyle;

    //Control Values
    private int _generation = 0;
    private int _amountAlive = 0;
    private int _amountSpecies = 0;
    private float _bestTotalFitness = 0;
    private float _bestAverageFitness = 0;
    private float _bestFitnessLastGeneration = 0;
    private float _averageFitnessLastGeneration = 0;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;

        SetStartGenome();

        //Set GUIStyle
        _guiStyle = new GUIStyle();
        _guiStyle.fontSize = 25;
        _guiStyle.normal.textColor = Color.white;

        _manager = new PopulationManager();
        _manager.Callback = this;

        
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Controls:", _guiStyle);
        GUI.Label(new Rect(10, 25, 250, 30), "TimeScale: " + Time.timeScale, _guiStyle);
        GUI.Label(new Rect(10, 50, 250, 30), "Running: " + _evaluationRunning, _guiStyle);
        GUI.EndGroup();

        GUI.BeginGroup(new Rect(10, 160, 350, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Values:", _guiStyle);
        GUI.Label(new Rect(10, 25, 250, 30), "Generation: " + _generation, _guiStyle);
        GUI.Label(new Rect(10, 50, 250, 30), "PopulationSize: " +_generationSize, _guiStyle);
        GUI.Label(new Rect(10, 75, 250, 30), "Amount Spcies: " + _amountSpecies, _guiStyle);
        GUI.Label(new Rect(10, 100, 250, 30), "Amount Alive: " +_amountAlive, _guiStyle);
        GUI.EndGroup();

        GUI.BeginGroup(new Rect(10, 320, 350, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Fitness values:", _guiStyle);
        GUI.Label(new Rect(10, 25, 250, 30), "Best Fitness: " + _bestTotalFitness, _guiStyle);
        GUI.Label(new Rect(10, 50, 250, 30), "Best avg Fitness: " + _bestAverageFitness, _guiStyle);
        GUI.Label(new Rect(10, 75, 250, 30), "Last Gen Best: " + _bestFitnessLastGeneration, _guiStyle);
        GUI.Label(new Rect(10, 100, 250, 30), "Last Gen Avg: " + _averageFitnessLastGeneration, _guiStyle);
        GUI.EndGroup();
    }

    /// <summary>
    /// Start and stop the evaluation
    /// </summary>
    public void StartStopEvaluation()
    {
        if (_evaluationRunning)
        {
            _evaluationRunning = !_evaluationRunning;
            KillAllPlayerManually();
        }
        else
        {
            _evaluationRunning = !_evaluationRunning;
            SetStartGenome();
            _manager.CreateInitialPopulation(_startGenome, _nodeCounter, _connectionCounter, _generationSize, true);
        }
    }

    /// <summary>
    /// Kill all player manually and add not time bonus
    /// </summary>
    public void KillAllPlayerManually()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            Player script = player.GetComponent<Player>();
            script.KillPlayer(false);
        }
    }

    /// <summary>
    /// Reload the complete scene
    /// </summary>
    public void ReloadLevel()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Create the start genome
    /// </summary>
    private void SetStartGenome()
    {
        _nodeCounter = new GeneCounter(0);
        _connectionCounter = new GeneCounter(0);

        _startGenome = new Genome();

        //5 input nodes + 1 bias node
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.INPUT, 0f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));

        //OutputNodes
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.OUTPUT, 1f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));
        _startGenome.AddNodeGene(new NodeGene(_nodeCounter.GetNewNumber(), NodeGeneType.OUTPUT, 1f, ActivationFunctionHelper.Function.STEEPENED_SIGMOID));

        //Add connections to first output nde
        _startGenome.AddConnectionGene(new ConnectionGene(0, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(1, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(2, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(3, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(4, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(5, 6, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));

        //Connection to second ouztputNode
        _startGenome.AddConnectionGene(new ConnectionGene(0, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(1, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(2, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(3, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(4, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));
        _startGenome.AddConnectionGene(new ConnectionGene(5, 7, Random.Range(-1f, 1f), true, _connectionCounter.GetNewNumber()));

    }

    #region Callback methods

    public AgentObject InitNewAgent(PopulationManager populationManager, Genome genome)
    {
        GameObject obj = Instantiate(_playerPrefab, _spawnPosition.position, Quaternion.identity);
        obj.GetComponent<Player>()._customAgent = new Player.CustomAgent(genome, populationManager);

        return obj.GetComponent<Player>()._customAgent;
    }

    public void AgentsInitializedCallback(List<AgentObject> agents, List<Species> species, int generation)
    {
        Debug.Log("All agents initalized. Generation: " + generation + ", Amount: " + agents.Count + ", Species: " + species.Count);

        //Debug Values for GUi
        _amountAlive = agents.Count;
        _amountSpecies = species.Count;
        _generation = generation;
    }

    public void AgentKilledCallback(AgentObject agent)
    {
        //Reduce Value for GUI
        _amountAlive -= 1;
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

        Debug.Log("All agents dead. Best fitness: " + bestAgent.GetFitness() + ", Average: " + eval.AverageFitness);

        //GUI Values
        _bestFitnessLastGeneration = bestAgent.GetFitness();
        _averageFitnessLastGeneration = eval.AverageFitness;

        if (_bestTotalFitness < _bestFitnessLastGeneration) _bestTotalFitness = _bestFitnessLastGeneration;
        if (_bestAverageFitness < _averageFitnessLastGeneration) _bestAverageFitness = _averageFitnessLastGeneration;

        //Start next generation if the evaluation is running
        if(_evaluationRunning) _manager.GenerateNextGeneration();


    }

    #endregion
}
