using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{

    public class XOR_Agent : MonoBehaviour
    {

        public XOR _xorAgent;

        public static double[,] _xorValues = { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 0 } };

        int _loopCounter = 0;

        // Use this for initialization
        void Start()
        {
            _xorAgent._fitness = 4;
            _xorAgent.Genome.Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (_xorAgent.Active)
            {

                if (_loopCounter <= 3)
                {
                    double input1 = _xorValues[_loopCounter, 0];
                    double input2 = _xorValues[_loopCounter, 1];
                    double output = _xorValues[_loopCounter, 2];

                    //One bias node
                    double[] result = _xorAgent.Genome.Calculate(new double[] { input1, input2, 1 });

                    float difference = Mathf.Abs((float)(output - result[0]));
                    _xorAgent._fitness -= difference;

                    _loopCounter++;
                }
                else
                {
                    _xorAgent._fitness = Mathf.Pow(_xorAgent._fitness, 2);
                    _xorAgent.KillAgent();
                }

            }

        }


        public class XOR : AgentObject
        {
            public float _fitness;

            public XOR(Genome genome, PopulationManager manager)
            {
                InitGenome(genome, manager);
            }

            public override float GetFitness()
            {
                return _fitness;
            }
        }
    }
}

