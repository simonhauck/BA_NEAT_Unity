using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;


public class ActivationFunctionHelper_Test {

    [SetUp]
    public void SetUp()
    {

    }

    [Test]
    public void ActivationFunction_Test()
    {
        Assert.AreEqual(0.5, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.SIGMOID,0));
        Assert.AreEqual(0.731, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.SIGMOID, 1), 0.0005);
        Assert.AreEqual(0.2689, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.SIGMOID, -1), 0.0005);

        Assert.AreEqual(0, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.STEP_FUNC, 0));
        Assert.AreEqual(0, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.STEP_FUNC, 0));

        Assert.AreEqual(1, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.STEP_FUNC, 2));
        Assert.AreEqual(1, ActivationFunctionHelper.ActivationFunction(ActivationFunctionHelper.Function.STEP_FUNC, 0.1));
    }

    [Test]
    public void SigmoidFunction_Test()
    {
        Assert.AreEqual(0.5, ActivationFunctionHelper.SigmoidFunction(0));
        Assert.AreEqual(0.731, ActivationFunctionHelper.SigmoidFunction(1), 0.0005);
        Assert.AreEqual(0.2689, ActivationFunctionHelper.SigmoidFunction(-1), 0.0005);
    }

    [Test]
    public void StepFunction_Test()
    {
        Assert.AreEqual(0, ActivationFunctionHelper.StepFunction(0));
        Assert.AreEqual(0, ActivationFunctionHelper.StepFunction(0));

        Assert.AreEqual(1, ActivationFunctionHelper.StepFunction(2));
        Assert.AreEqual(1, ActivationFunctionHelper.StepFunction(0.1));
    }
}
