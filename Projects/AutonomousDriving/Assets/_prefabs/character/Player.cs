using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject _glasses;
    public float _raycastDistance;
    public Material _materialDeadPlayer;


    //LevelGrid
    public float _horizontalGridSize = 9f;
    public float _verticalGridSize = 8f;

    //Values for killing players
    public float _maxTimeForLevel = 60;
    public float _timeBetweenMovementCheck = 5;
    public float _minMovementBetweenCheck = 7f;

    //Movement
    public float _maxMovePower;
    public float _maxTurnPower;

    //LayerMask for the walls
    public LayerMask _wallLayerMask;

    //Agent object for the neuronal net
    public CustomAgent _customAgent;

    public bool _useFixTimeIntervalls;

    private bool _alive;

    //Values for fitness
    //private float _distanceTravelled;
    private float _timeAlive;
    private Vector3 _lastPos;

    //MovementCheck
    private float _timeSinceLastMovementCheck;
    private Vector3 _positionLastMovementCheck;

    private List<Vector3> _checkPoints;

    #region Override methods

    // Use this for initialization
    void Start () {
        _alive = true;
        _timeAlive = 0;
        //_distanceTravelled = 0;
        _lastPos = this.transform.position;

        //Movement check
        _positionLastMovementCheck = this.transform.position;
        _timeSinceLastMovementCheck = 0;

        _checkPoints = new List<Vector3>();

        if(_customAgent != null)
        {
            _customAgent.Genome.Init();
        }
        
	}
	
	// Update is called once per frame
	void Update () {

        if (!_alive) return;

        float deltaTime = _useFixTimeIntervalls ? 0.02f : Time.deltaTime;

        //Update values
        _timeAlive += deltaTime;
        //_distanceTravelled += Vector3.Distance(this.transform.position, _lastPos);
        _lastPos = this.transform.position;

        //Check if agent lived to long
        if (_timeAlive >= _maxTimeForLevel)
        {
            KillPlayer(false);
            return;
        }

        //Check if character has moved
        CheckMovement(deltaTime);

        AddCheckPoint(this.transform.position, _checkPoints);

        //Get Input
        float[] inputDistance = GetInput();

        //For manual controlling
        if (Input.GetKey(KeyCode.W))
        {
            MovePlayer(1f, deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotatePlayer(1f, deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            RotatePlayer(-1f, deltaTime);
        }

        //Move with neuronal net
        if(_customAgent != null)
        {
            //Get Inputs
            double[] input = new double[6];
            for (int i = 0; i < inputDistance.Length; i++)
            {
                input[i] = inputDistance[i];
            }
            input[5] = 1.0d;

            //Result
            double[] result = _customAgent.Genome.Calculate(input);

            //Get a value between -1 and 1
            float rotation = -1f + (float)result[0] * 2;
            float movement = (float)result[1];

            RotatePlayer(rotation, deltaTime);
            MovePlayer(movement, deltaTime);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            KillPlayer(false);
        } else if (other.CompareTag("Finish"))
        {
            KillPlayer(true);
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Get the input view for the player.
    /// The result contains 5 elements with the distance to the wall if measured.
    /// Else the value will be the raycast distance
    /// </summary>
    /// <returns>array with 5 result values. Can not be null or empty</returns>
    public float[] GetInput()
    {
        float[] result = new float[5];

        //- 60 degrees
        int indexArray = 0;
        for (int i = -60; i <= 60; i = i + 30)
        {
            Vector3 direction = _glasses.transform.TransformDirection(Vector3.right);
            direction = Quaternion.AngleAxis(i, Vector3.up) * direction;
            RaycastHit hit;
            if(Physics.Raycast(_glasses.transform.position, direction, out hit, _raycastDistance, _wallLayerMask))
            {
                Debug.DrawRay(_glasses.transform.position, direction * hit.distance, Color.yellow);
                result[indexArray] = hit.distance;
            }
            else
            {
                Debug.DrawRay(_glasses.transform.position, direction * _raycastDistance, Color.blue);
                result[indexArray] = _raycastDistance;
            }

            indexArray++;
        }


        //Debug.Log(result[0] + " " + result[1] + " " + result[2] + " " + result[3] + " " + result[4]);
        

        return result;

    }

    /// <summary>
    /// Move a player forwards
    /// </summary>
    /// <param name="moveForce">should be between -1 and 1. Part of the maxMovePower</param>
    /// <param name="deltaTime">Time value that will be added</param>
    public void MovePlayer(float moveForce, float deltaTime)
    {
        this.transform.position += this.transform.right * moveForce * _maxMovePower * deltaTime;
            
    }

    /// <summary>
    /// Rotate the player 
    /// </summary>
    /// <param name="rotation">should be between -1 and 1. Part of the maxRotationPowers</param>
    /// <param name="deltaTime">Time value that should be added</param>
    public void RotatePlayer(float rotation, float deltaTime)
    {
        this.transform.Rotate(0, _maxTurnPower * rotation * deltaTime, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    public void KillPlayer(bool addTimeBonus)
    {
        //If the player is not alive, return

        if (!_alive) return;
        _alive = false;

        GetComponent<Renderer>().material = _materialDeadPlayer;

        float fitness = CalculateFitness(addTimeBonus);

        if(_customAgent != null)
        {
            _customAgent._fitness = fitness;
            _customAgent.KillAgent();
        }


        //Debug.Log("Player killed "+fitness);
    }

    #endregion

    #region Private methods

    private float CalculateFitness(bool addTimeBonus)
    {
        float fitness = 0.1f * Mathf.Pow(_checkPoints.Count, 2);

        if (!addTimeBonus)
        {
            return fitness;
        }
        else
        {
            //Add a time bonus
            //float distanceTraveld = _distanceTravelled;
            float timeBonus = _maxTimeForLevel - _timeAlive;

            if(timeBonus <= 0)
            {
                timeBonus = 0;
            }

            return fitness + 2 * Mathf.Pow(timeBonus, 2);
        }
            
    }

    private void CheckMovement(float deltaTime)
    {
        _timeSinceLastMovementCheck += deltaTime;

        if(_timeSinceLastMovementCheck >= _timeBetweenMovementCheck)
        {
            _timeSinceLastMovementCheck = 0f;

            float distance = Vector3.Distance(this.transform.position, _positionLastMovementCheck);

            if(distance >= _minMovementBetweenCheck)
            {
                _positionLastMovementCheck = this.transform.position;
            }
            else
            {
                KillPlayer(false);
            }
        }
    }

    private void AddCheckPoint(Vector3 position, List<Vector3> visitedCheckpoints)
    {
        Vector3 snappedVector = new Vector3(
                position.x - position.x % _verticalGridSize,
                0f,
                position.z - position.z % _horizontalGridSize
            );

        bool addValue = true;

        foreach(Vector3 pos in visitedCheckpoints)
        {
            //Tolerance value
            if(Vector3.Distance(pos, snappedVector) <= 0.5f)
            {
                addValue = false;
                break;
            }
        }

        if (addValue)
        {
            visitedCheckpoints.Add(snappedVector);
        }

        //Debug.Log(visitedCheckpoints.Count);
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
