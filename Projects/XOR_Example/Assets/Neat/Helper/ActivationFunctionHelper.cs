using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationFunctionHelper
{

    public enum Function
    {
        SIGMOID,
        STEEPENED_SIGMOID,
        STEP_FUNC
    }

    /// <summary>
    /// Get the result for the given activation function and the given input
    /// </summary>
    /// <param name="function">the type of the function</param>
    /// <param name="input">the input value</param>
    /// <returns>the corresponding output value</returns>
    public static double ActivationFunction(Function function, double input)
    {
        switch (function)
        {
            case Function.STEP_FUNC:
                return StepFunction(input);
            case Function.STEEPENED_SIGMOID:
                return SteepenedSigmoidFunction(input);
            case Function.SIGMOID:
            default:
                return SigmoidFunction(input);
        }
    }

    /// <summary>
    /// Implementation for the Sigmoid function
    /// </summary>
    /// <param name="input">the x value</param>
    /// <returns>the corresponding y value</returns>
    public static double SigmoidFunction(double input)
    {
        double k = System.Math.Exp(input);
        return k / (1.0f + k);
    }

    public static double SteepenedSigmoidFunction(double input)
    {
        return 1 / (1.0 + System.Math.Exp(-4.9 * input));
    }

    /// <summary>
    /// Implementation for a Step function. The threshold is 0
    /// </summary>
    /// <param name="input">the x value</param>
    /// <returns>the corresponding y value</returns>
    public static double StepFunction(double input)
    {
        if (input <= 0) return 0;
        else return 1;
    }
}
