using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Replay
{
    public bool useTrainedExample;
	public List<double> states;
	public double reward;

	public Replay(double px, double gapX, double r)
	{
		states = new List<double>();
        // states.Add(ry);
        // states.Add(rv); 
        states.Add(px);
        // states.Add(py);
        states.Add(gapX);
		reward = r;
	}
}

public class Brain : MonoBehaviour
{
	public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckRadius;
	private bool isOnGround;

	private Rigidbody2D rb;
	private RobotController robotAccess;
	private GameManager theGameManager;
	public GameObject theEyes;
	ANN ann;

	float reward = 0.0f;
	List<Replay> replayMemory = new List<Replay>();
	int mCapacity = 10000;
	bool hitObstacle;
	bool seeGround = true;
	public float seeDistance;

	float discount = 0.99f;
	float exploreRate = 100.0f;
	float maxExploreRate = 100.0f;
	float minExploreRate = 0.01f;
	float exploreDecay = 0.0004f;

	int failCount = 0;
    float sumOfJumps = 0;
    float sumOfStays = 0;

    int frames = 0;
	float timer = 0;
	float maxBalanceTime = 0;
    Vector3 bestTarget;
    private bool wait;

    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(2,2,1,6,0.2f);
        rb = GetComponent<Rigidbody2D>();
        robotAccess = GetComponent<RobotController>();
        theGameManager = FindObjectOfType<GameManager>();
        Time.timeScale = 5.0f;
        wait = false;
    }

    GUIStyle guiStyle = new GUIStyle();
	void OnGUI()
	{
		guiStyle.fontSize = 50;
		guiStyle.normal.textColor = Color.white;
		GUI.BeginGroup (new Rect (10, 10, 600, 150));
		GUI.Box (new Rect (0,0,140,140), "Stats", guiStyle);
		GUI.Label(new Rect (10,10,500,30), "Fails: " + failCount, guiStyle);
		GUI.Label(new Rect (10,100,500,30), "Decay Rate" + exploreRate, guiStyle);
		GUI.Label(new Rect (10,150,500,30), "Last Best Balance" + maxBalanceTime, guiStyle);
		GUI.Label(new Rect (10,200,500,30), "This Balance" + timer, guiStyle);
		GUI.EndGroup ();
	}

	void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Killbox")
        {
         	hitObstacle = true;   
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("space"))
        {
        	Debug.Log(ann.PrintWeights());
        }
         if(Input.GetKey(KeyCode.Return))
        {
            ann.LoadWeights();
        }
    }

    void FixedUpdate() {
        frames++;
    	// seeGround = true;
    	// isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    	Debug.DrawRay(theEyes.transform.position, theEyes.transform.right * 20, Color.green);
    	RaycastHit2D hit = Physics2D.Raycast(theEyes.transform.position, theEyes.transform.right * 20);
    	if(hit && hit.collider.tag == "Killbox")
    	{
    		seeGround = false;
    		Debug.DrawRay(theEyes.transform.position, theEyes.transform.right * 20, Color.red);

    	}
    	    	// double[] distancesFromObjects = new double[platforms.Length];
    	// for(int i = 0; i < platforms.Length; i++)
    	// {
    	// 	Vector3 heading = transform.position - platforms[i].transform.position;
    	// 	distancesFromObjects[i] = heading.magnitude;
    	// }
    	// // second closest, to be honest
    	// System.Array.Sort(distancesFromObjects);
    	// double closestPlatform = distancesFromObjects[1];
    	// int indexOfClosest = distancesFromObjects.ToList().IndexOf(closestPlatform);
    	// Vector3 closestPoint = platforms[indexOfClosest].transform.position;

    	timer += Time.deltaTime;
    	List<double> states = new List<double>();
    	List<double> qs = new List<double>();
    	GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
    	Vector3 bestPoint = GetClosestEnemy(platforms);
        Vector3 closestPoint = GetClosestGap(platforms);

	    Vector3 directionToNextPlatform = bestPoint - transform.position;
        Vector3 directionToNextGap = closestPoint - transform.position;
        // states.Add(transform.position.y);
        // states.Add(rb.velocity.y);
        states.Add(directionToNextPlatform.x);
        // states.Add(directionToNextPlatform.y);
    	states.Add(directionToNextGap.x);
        // Debug.Log(directionToNextGap.x);

    	qs = SoftMax(ann.CalcOutput(states));
    	double maxQ = qs.Max();
    	int maxQIndex = qs.ToList().IndexOf(maxQ);
    	exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

    	if(Random.Range(0,100) < exploreRate)
    		maxQIndex = Random.Range(0,2);

        if(maxQIndex == 1)
         sumOfJumps ++;

        if(maxQIndex == 0)
        sumOfStays ++;

		if(frames % 8 == 0)
		{
            if(sumOfJumps > sumOfStays)
            {
			robotAccess.RobotJump();
            }
            sumOfStays = 0;
            sumOfJumps = 0;
            frames = 0;
		}

    	if(rb.velocity.x < 0.5)
    		robotAccess.RobotJump();

    	if(hitObstacle)
    		reward = -5.0f;
    	else
    		reward = 0.1f;

    	

    	Replay lastMemory = new Replay(
            // transform.position.y,
            // rb.velocity.y,
        directionToNextPlatform.x,
        // directionToNextPlatform.y,
                                         directionToNextGap.x,
    									reward);
    	if(replayMemory.Count > mCapacity)
    		replayMemory.RemoveAt(0);

    	replayMemory.Add(lastMemory);
        
    	if(hitObstacle)
    	{
    		for(int i = replayMemory.Count - 1; i >= 0; i--)
    		{
    			List<double> tOutputsOld = new List<double>();
    			List<double> tOutputsNew = new List<double>();
    			tOutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

    			double maxQOld = tOutputsOld.Max();
    			int action = tOutputsOld.ToList().IndexOf(maxQOld);

    			double feedback;
    			if(i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
    				feedback = replayMemory[i].reward;
    			else
    			{
    				tOutputsNew = SoftMax(ann.CalcOutput(replayMemory[i+1].states));
    				maxQ = tOutputsNew.Max();
    				feedback = (replayMemory[i].reward + discount * maxQ);
    			}

    			tOutputsOld[action] = feedback;
    			ann.Train(replayMemory[i].states, tOutputsOld);
    		}
    		if(timer > maxBalanceTime)
    		{
    			maxBalanceTime = timer;
    		}

    		timer = 0;

    		hitObstacle = false;
    		theGameManager.Reset();
    		replayMemory.Clear();
    		failCount++;
    	}
    }

    IEnumerator Wait()
    {
    	wait = true;
        yield return new WaitForSeconds(0.5f);
        wait = false;
    }

    Vector3 GetClosestEnemy (GameObject[] platforms)
    {
    	bestTarget = new Vector3(0f,0f,0f);
        Vector3 currentPosition = transform.position;
   		float closestDistanceSqr = Mathf.Infinity;
        foreach(GameObject potentialTarget in platforms)
        {
            Vector3 leftMargin = potentialTarget.transform.position -
             new Vector3((potentialTarget.GetComponent<Collider2D>().bounds.size.x) / 2f , 0f, 0f);
            // Debug.DrawRay(leftMargin, Vector3.up * 20, Color.red);

        	if(leftMargin.x > currentPosition.x)
        	{
	            Vector3 directionToTarget = leftMargin - currentPosition;
	            float dSqrToTarget = directionToTarget.sqrMagnitude;
	            if(dSqrToTarget < closestDistanceSqr)
	            {
	                closestDistanceSqr = dSqrToTarget;
	                bestTarget = leftMargin;
	            }
      	    }
        }
     	Debug.DrawRay(bestTarget, Vector3.up * 20, Color.green);
        return bestTarget;
    }

     Vector3 GetClosestGap (GameObject[] platforms)
    {
        bestTarget = new Vector3(0f,0f,0f);
        Vector3 currentPosition = transform.position;
        float closestDistanceSqr = Mathf.Infinity;
        foreach(GameObject potentialTarget in platforms)
        {
            Vector3 halfThePlatform = new Vector3((potentialTarget.GetComponent<Collider2D>().bounds.size.x) / 2f , 0f, 0f);
            Vector3 leftMargin = potentialTarget.transform.position - halfThePlatform;
             
            // Debug.DrawRay(leftMargin, Vector3.up * 20, Color.red);

            if(leftMargin.x < currentPosition.x)
            {
                Vector3 directionToTarget = leftMargin - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = leftMargin + Vector3.Scale(halfThePlatform, new Vector3(2f,0f,0f));
                }
            }
        }
        Debug.DrawRay(bestTarget, Vector3.up * 20, Color.blue);
        return bestTarget;
    }


    List<double> SoftMax(List<double> values)
    {
    	double max = values.Max();

    	float scale = 0.0f;
    	for(int i = 0; i < values.Count; ++i)
    	{
    		scale += Mathf.Exp((float)(values[i] - max));
    	}

    	List<double> result = new List<double>();
    	for(int i = 0; i < values.Count; ++i)
    	{
    		result.Add(Mathf.Exp((float)(values[i] - max)) / scale);
    	}

    	return result;
    }
}

// jocul soft max functie, nn din cartea aia / despre reinforcement si q mai bine / ce parametri am folosit