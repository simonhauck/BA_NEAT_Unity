using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Species {

    public static int GENERATION_UNTIL_DEAD = 15;

    #region Properties

    public int ID { get { return _id; } set{ _id = value; } }
    public AgentObject RepresentiveAgent { get { return _representiveAgent; } set { _representiveAgent = value; } }
    public List<AgentObject> Members { get { return _members; } set { _members = value; } }

    public float TotalSharedFitness { get { return _totalSharedFitness; } set { _totalSharedFitness = value; } }
    public float MaxFitness { get { return _maxFitness; } set { _maxFitness = value; } }


    #endregion

    private int _id;
    private AgentObject _representiveAgent;
    private List<AgentObject> _members;

    private float _totalSharedFitness;

    //Maximal achiced fitness
    private float _maxFitness = 0;
    private int _generationSinceFitnessIncreased = 0;

    public Species(int id, AgentObject representiveAgent)
    {
        _id = id;
        _representiveAgent = representiveAgent;
        _members = new List<AgentObject>();
    }

    public void CalculateTotalSharedFitness()
    {
        _totalSharedFitness = 0;
        foreach(AgentObject agent in _members)
        {
            _totalSharedFitness += agent.GetFitness();
        }

        _totalSharedFitness /= _members.Count;
    }

    /// <summary>
    /// Order the members in descending order by their fitness
    /// </summary>
    public void SortMembersByFitness()
    {
        _members = _members.OrderByDescending(x => x.GetFitness()).ToList();
    }

    /// <summary>
    /// Select a new random representive agent
    /// </summary>
    public void SelectNewRandomRepresentiveAgent()
    {
        if (_members.Count == 0) throw new System.Exception("No Members in Species. Can't select a new Representive Agent");

        int index = Random.Range(0, _members.Count);
        _representiveAgent = _members[index];
    }

    /// <summary>
    /// Reset the species. Clear the list of members and set the totalSharedFitness to 0
    /// </summary>
    public void ResetSpecies()
    {
        _members.Clear();
        _totalSharedFitness = 0;
    }

    //TODO test
    public void SetBestFitness()
    {
        foreach(AgentObject agent in _members)
        {
            if(_maxFitness < agent.GetFitness())
            {
                _maxFitness = agent.GetFitness();
                _generationSinceFitnessIncreased = 0;
            }
        }
    }

    //TODO test
    public void RemoveHalf()
    {
        if(_members.Count >= 2)
        {
            int firstDeleted = Mathf.FloorToInt(_members.Count / 2f);
            for(int i = _members.Count-1; i>=firstDeleted; i--)
            {
                _members.RemoveAt(i);
            }
        }
    }

    public bool IsActive()
    {
        if(_generationSinceFitnessIncreased <= GENERATION_UNTIL_DEAD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IncreaseGeneration()
    {
        _generationSinceFitnessIncreased++;
    }
}
