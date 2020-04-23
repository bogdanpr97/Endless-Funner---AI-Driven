using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public PlayerController thePlayer;
    public RobotController theRobot;


	private Vector3 lastPlayerPosition;
	private float distanceToMoveX;

    // Start is called before the first frame update
    void Start()
    {
        lastPlayerPosition = theRobot.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    	distanceToMoveX = theRobot.transform.position.x - lastPlayerPosition.x;
        transform.position = new Vector3(transform.position.x + distanceToMoveX, transform.position.y, transform.position.z);
    	lastPlayerPosition = theRobot.transform.position;
        
    }
}
