using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] WallHitTracker tracker;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Wall hit");
            var carAgent =  other.gameObject.GetComponent<CarAgent>();
            tracker.WallHitCount++;
            Debug.Log(tracker.WallHitCount);
            carAgent.AddReward(-13f);
            carAgent.EndEpisode();
        }
    }
}
