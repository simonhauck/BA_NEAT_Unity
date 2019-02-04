using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Properties
    public bool Alive { get { return _alive; } }

    public int LevelViewWidht { get { return _levelViewWidht; } set { _levelViewWidht = value; } }
    public int LevelViewHeight { get { return _levelViewHeight; } set { _levelViewHeight = value; } }

    #endregion

    #region Publich fields

    public float _maxTimeForLevel;
    public float _timeBetweenMovementCheck;
    public float _movementBetweenCheck;

    public LayerMask _levelAndEnemyView;

    public bool _useFixDeltaTime;

    //Neat agent
    public CustomAgent _customAgent;

    #endregion

    //Movement script
    private PlayerMovement _playerMovement;

    //Alive
    private bool _alive;
    private float _timeAlive;

    //For the fitness function
    private Vector3 _startPos;

    //Movementcheck
    private float _timeLastMovementCheck;
    private Vector3 _positionLastMovementCheck;

    //LevelView
    private int _levelViewWidht;
    private int _levelViewHeight;


	// Use this for initialization
	void Start () {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.UseFixDeltaTime = _useFixDeltaTime;
        _startPos = this.transform.position;
        _alive = true;
        _timeAlive = 0;
        _timeLastMovementCheck = 0;
        _positionLastMovementCheck = this.transform.position;

        if(_customAgent != null)
        {
            _customAgent.Genome.Init();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!_alive) return;
        float deltaTime = _useFixDeltaTime ? 0.02f : Time.deltaTime;
        _timeAlive += deltaTime;

        //Check if time is up
        if (_timeAlive >= _maxTimeForLevel)
        {
            KillPlayer(false);
            return;
        }

        //Movementcheck
        if (!CheckMovement())
        {
            KillPlayer(false);
            return;
        }
        

        if(_customAgent != null)
        {
            double[] input = GetInput(_levelViewWidht, _levelViewHeight);
            double[] output = _customAgent.Genome.Calculate(input);

            float move = (float) output[0];
            float jump = output[1] >= 0.5d ? 1f : 0f;
            _playerMovement.Move(move, jump);

        }
        else
        {
            _playerMovement.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"));
        }
        
        

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FlagPole"))
        {
            KillPlayer(true);
        }
        else if (collision.CompareTag("Enemy"))
        {
            KillPlayer(false);
        } else if (collision.CompareTag("DeathFrame"))
        {
            KillPlayer(false);
        }
    }

    #region Public methods

    public static int GetAmountOfInputs(int widht, int height)
    {
        int startWidht = Mathf.FloorToInt(widht / 2);
        int startHeight = Mathf.FloorToInt(height / 2);

        return (startWidht * 2 + 1) * (startHeight * 2 + 1) + 1;
    }

    public void KillPlayer(bool addTimeBonus)
    {
        if (!_alive) return;
        _alive = false;

        GetComponent<Renderer>().enabled = false;
        float fitness = CalculateFitness(addTimeBonus);
        
        if(_customAgent != null)
        {
            _customAgent._fitness = fitness;
            _customAgent.KillAgent();
        }
        else
        {
            Debug.Log("Player Dead. Fitness: " + fitness);
        }
    }

    #endregion

    #region Private methods

    private float CalculateFitness(bool addTimeBonus)
    {
        float result = 0f;

        //Only positive distance values are allowed
        float distanceVal = this.transform.position.x - _startPos.x;
        result += distanceVal <= 0 ? 0 : 0.1f * Mathf.Pow(distanceVal, 2);

        if (addTimeBonus)
        {
            float remaningTime = _maxTimeForLevel - _timeAlive;
            result += Mathf.Pow(remaningTime, 3);
        }

        return result;
    }

    private double[] GetInput(int widht, int height)
    {
        int startWidht = Mathf.FloorToInt(widht / 2);
        int startHeight = Mathf.FloorToInt(height / 2);

        double[] inputArray = new double[(startWidht * 2 + 1) * (startHeight * 2 + 1) + 1];
        int inputIndex = 0;

        //string debugLine = "";

        //Scan from top to bottom
        for(int i = startHeight; i >= -startHeight; i--)
        {
            float topYCoordinate = this.transform.position.y + i + 0.1f;
            float bottomYCoordinate = this.transform.position.y + i - 0.1f;

            for(int j = -startWidht; j <= startWidht; j++)
            {
                float leftXCoordinate = this.transform.position.x + j - 0.1f;
                float rightXCoordinate = this.transform.position.x + j + 0.1f;

                Vector2 topLeft = new Vector2(leftXCoordinate, topYCoordinate);
                Vector2 bottomRight = new Vector2(rightXCoordinate, bottomYCoordinate);

                Collider2D[] hitResult = Physics2D.OverlapAreaAll(topLeft, bottomRight, _levelAndEnemyView);

                bool block = false;
                bool enemy = false;

                //Check each collider
                foreach(Collider2D hit in hitResult)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        enemy = true;
                        break;
                    } else if (hit.CompareTag("Block"))
                    {
                        block = true;
                    }
                }

                //Set values input array
                if (enemy)
                {
                    inputArray[inputIndex++] = -1f;
                   // debugLine += "E ";
                } else if (block)
                {
                    inputArray[inputIndex++] = 1f;
                    //debugLine += "X ";
                }
                else
                {
                    inputArray[inputIndex++] = 0f;
                    //debugLine += "O ";
                }
            }
            //debugLine += "\n";
        }

        //Bias value
        inputArray[inputIndex++] = 1f;
        //Debug.Log(debugLine);

        return inputArray;
    }

    private bool CheckMovement()
    {

        if (_timeAlive - _timeLastMovementCheck <= _timeBetweenMovementCheck) return true;

        if(Vector3.Distance(_positionLastMovementCheck, this.transform.position) >= _movementBetweenCheck)
        {
            _positionLastMovementCheck = this.transform.position;
            _timeLastMovementCheck = _timeAlive;
            return true;
        }

        return false;
    }

    #endregion

    public class CustomAgent : AgentObject
    {
        public float _fitness;

        public CustomAgent(Genome genome, PopulationManager populationManager)
        {
            InitGenome(genome, populationManager);
        }

        public override float GetFitness()
        {
            return _fitness;
        }
    }
}
