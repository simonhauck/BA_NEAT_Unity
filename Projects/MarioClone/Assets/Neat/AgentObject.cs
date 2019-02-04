using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentObject{

    #region Properties

    public Genome Genome { get { return _genome; } set { _genome = value; } }
    public bool Active { get { return _active; } set { _active = value; } }

    #endregion

    #region private fields

    private PopulationManager _evaluator;
    private Genome _genome;

    private bool _active = true;

    #endregion

    public AgentObject()
    {

    }

    #region Abstract Overide Methods

    public abstract float GetFitness();

    #endregion

    #region Public methods

    public void KillAgent()
    {
        _active = false;
        //Notify the PopulationManager
        _evaluator.AgentKilled(this);
    }

    public void InitGenome(Genome genome, PopulationManager populationManager)
    {
        _genome = genome;
        _evaluator = populationManager;
    }

    #endregion
}
