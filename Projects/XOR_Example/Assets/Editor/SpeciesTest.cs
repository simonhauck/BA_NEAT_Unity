using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;


public class SpeciesTest {

    private Species species;
    private PopulationManager populationManager;

    private AgentObject agent1;
    private AgentObject agent2;
    private AgentObject agent3;

    #region CustomAgent

    private class CustomAgent : AgentObject
    {

        private int _fitness;

        public CustomAgent(PopulationManager evaluator, Genome genome, int fitness)
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
        populationManager = new PopulationManager();

        agent1 = new CustomAgent(populationManager, new Genome(), 1);
        agent2 = new CustomAgent(populationManager, new Genome(), 2);
        agent3 = new CustomAgent(populationManager, new Genome(), 4);

        species = new Species(0, agent1);
        species.Members.Add(agent1);
        species.Members.Add(agent2);
        species.Members.Add(agent3);

    }

    [Test]
    public void CalculateTotalSharedFitness_Test()
    {
        species.CalculateTotalSharedFitness();
        Assert.AreEqual(2.333f, species.TotalSharedFitness, 0.001f);
    }

    [Test]
    public void SortMemberyByFitness()
    {
        species.SortMembersByFitness();

        Assert.AreEqual(agent3, species.Members[0]);
        Assert.AreEqual(agent2, species.Members[1]);
        Assert.AreEqual(agent1, species.Members[2]);
    }

    [Test]
    public void SelectNewRandomRepresentiveAgent_Test()
    {
        bool newRepresentiveAgentFound = false;
        for(int i = 0; i<5; i++)
        {
            species.SelectNewRandomRepresentiveAgent();
            if(species.RepresentiveAgent != agent1)
            {
                newRepresentiveAgentFound = true;
                break;
            }
        }
        Assert.True(newRepresentiveAgentFound);
    }

    [Test]
    public void ResetSpecies_Test()
    {
        species.TotalSharedFitness = 10f;
        Assert.AreEqual(3, species.Members.Count);
        Assert.AreEqual(10, species.TotalSharedFitness);

        species.ResetSpecies();
        Assert.AreEqual(0, species.Members.Count);
        Assert.AreEqual(0, species.TotalSharedFitness);
    }
}
