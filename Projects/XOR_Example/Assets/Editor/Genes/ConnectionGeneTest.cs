using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class ConnectionGeneTest {

    private ConnectionGene connectionGene1;

    [SetUp]
    public void SetUp()
    {
        connectionGene1 = new ConnectionGene(5, 10, 0.4, true, 2);
    }

    [Test]
    public void CopyConnectionGene_Test()
    {
        ConnectionGene copiedGene = new ConnectionGene(connectionGene1);

        //Not the same object
        Assert.AreNotEqual(copiedGene, connectionGene1);

        //Test values
        Assert.AreEqual(connectionGene1.InNode, copiedGene.InNode);
        Assert.AreEqual(connectionGene1.OutNode, copiedGene.OutNode);
        Assert.AreEqual(connectionGene1.Weight, copiedGene.Weight);
        Assert.AreEqual(connectionGene1.Expressed, copiedGene.Expressed);
        Assert.AreEqual(connectionGene1.InnovationNumber, copiedGene.InnovationNumber);
    }

}
