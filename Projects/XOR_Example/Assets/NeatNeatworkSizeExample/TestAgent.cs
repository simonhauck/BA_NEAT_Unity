using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : AgentObject
{
    private float _targetFitness;
    private float _actualFitness;

    public TestAgent(PopulationManager evaluator, Genome genome, float targetFitness)
    {
        this._targetFitness = targetFitness;
        InitGenome(genome, evaluator);
    }

    public void CalcualteFitness()
    {
        float sum = 0;
        foreach(ConnectionGene con in this.Genome.Connections.Values)
        {
            if (con.Expressed)
            {
                sum += (float) con.Weight;
            }
        }
        float diff = Mathf.Abs(_targetFitness - sum);
        _actualFitness = 1000f / diff;
    }

    public override float GetFitness()
    {
        return _actualFitness;
    }
}
