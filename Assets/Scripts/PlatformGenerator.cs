using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
	public GameObject[] thePlatforms;
	public Transform generationPoint;
	public float distanceBetweenMin;
	public float distanceBetweenMax;
    public ObjectPooler[] theObjectPools;

	private float distanceBetween;
    private int platformSelector;
    private float[] platformWidths;

    private float minHeight;
    private float maxHeight;
    private float heightChange;
    public float maxHeightChange;
    public Transform maxHeightPoint;

    private CoinGenerator theCoinGenerator;
    public float randomCoinThreshold;

    public float randomSpikeThreshold;
    public ObjectPooler spikePooler;

    public float powerupHeight;
    public ObjectPooler powerupPool;
    public float powerupThreshhold;

    // Start is called before the first frame update
    void Start()
    {
        platformWidths = new float[theObjectPools.Length];
        for(int i = 0; i < platformWidths.Length; i++)
        {
            platformWidths[i] = theObjectPools[i].pooledObject.GetComponent<BoxCollider2D>().size.x;
        }

        minHeight = transform.position.y;
        maxHeight = maxHeightPoint.position.y;

        theCoinGenerator = FindObjectOfType<CoinGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
     	if(transform.position.x < generationPoint.position.x)
     	{
            platformSelector = Random.Range(0, theObjectPools.Length);
     		distanceBetween = Random.Range(distanceBetweenMin, distanceBetweenMax);
            heightChange = transform.position.y + Random.Range(maxHeightChange, -maxHeightChange);
     		transform.position = new Vector3(transform.position.x + (platformWidths[platformSelector] / 2) + distanceBetween,
     										Mathf.Clamp(heightChange, minHeight, maxHeight), 
     										transform.position.z);

            if(Random.Range(0f, 100f) < powerupThreshhold)
            {
                GameObject newPowerup = powerupPool.GetPooledObject();
                newPowerup.transform.position = transform.position + new Vector3(
                                                    distanceBetween/2,
                                                    Random.Range(1f,powerupHeight),
                                                    0f);
                newPowerup.SetActive(true);
            }
            //object pooling
     		GameObject newPlatform = theObjectPools[platformSelector].GetPooledObject();
     		newPlatform.transform.position = transform.position;
     		newPlatform.transform.rotation = transform.rotation;
     		newPlatform.SetActive(true);

            if(Random.Range(0f,100f) < randomCoinThreshold)
            {
            theCoinGenerator.SpawnCoins(new Vector3(transform.position.x,
                                                    transform.position.y +1f,
                                                    transform.position.z));
            }

            if(Random.Range(0f,100f) < randomSpikeThreshold)
            {
                GameObject newSpike = spikePooler.GetPooledObject();
                float spikeXPosition = Random.Range(-platformWidths[platformSelector]/2 +2f, platformWidths[platformSelector]/2 -2f);
                Vector3 spikePosition = new Vector3(spikeXPosition,0.5f,0f);

                newSpike.transform.position = transform.position + spikePosition;
                newSpike.transform.rotation = transform.rotation;
                newSpike.SetActive(true);

            }

            transform.position = new Vector3(transform.position.x + (platformWidths[platformSelector] / 2) + distanceBetween,
                                            transform.position.y, 
                                            transform.position.z);


     	}   
    }
}
