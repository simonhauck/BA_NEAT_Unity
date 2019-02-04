using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationEvaluator {

    #region Properties

    //Generation values
    public int Generation { get { return _generation; } }

    //Best agent
    public AgentObject BestAgent { get { return _bestAgent; } }
    public float BestAgentFitness { get { return _bestAgent.GetFitness(); } }

    //Average stuff
    public float AverageFitness { get { return _averageFitness; } }

    //Generation stuff
    public int AmountSpecies { get { return _amountSpecies; } }

    #endregion

    #region Private fields

    private int _generation;

    //Best agent values
    private AgentObject _bestAgent;

    private float _averageFitness;

    //Amount generations
    private int _amountSpecies;

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="agents">the agents</param>
    /// <param name="species">the species</param>
    /// <param name="generation">the generation</param>
    public GenerationEvaluator(List<AgentObject> agents, List<Species> species, int generation)
    {
        _generation = generation;
        _amountSpecies = species.Count;
        _bestAgent = GetBestAgent(agents);
        _averageFitness = CalculateAverageFitness(agents);
    }

    #region Calcualte Values

    private AgentObject GetBestAgent(List<AgentObject> agents)
    {
        AgentObject best = null;
        foreach(AgentObject agent in agents)
        {
            if(best == null || best.GetFitness() < agent.GetFitness())
            {
                best = agent;
            }
        }
        return best;
    }

    private float CalculateAverageFitness(List<AgentObject> agents)
    {
        float average = 0;

        if (agents.Count == 0) return average;

        for(int i = 0; i<agents.Count; i++)
        {
            average += agents[i].GetFitness();
        }

        average /= agents.Count;
        return average;
    }

    #endregion



}
