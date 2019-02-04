using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneCounter {
    
    private int _currentVal = 0;
    
    /// <summary>
    /// Generate a counter with a given start number
    /// </summary>
    /// <param name="startVal">The number that will be returned when the first number is requested</param>
    public GeneCounter(int startVal)
    {
        _currentVal = startVal;
    }

    /// <summary>
    /// return the current val and increase the value afterwards
    /// </summary>
    /// <returns>a for this Counter unique number</returns>
    public int GetNewNumber()
    {
        return _currentVal++;
    }
}
