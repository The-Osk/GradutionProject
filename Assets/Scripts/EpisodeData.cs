using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "EpisodeData")]
public class EpisodeData
{
    public int epNo = 0;
    public int timesteps = 0;
    public float time = 0;

    public int wallHit = 0;
    public string collidedWith = "";

    public double distanceCovered = 0;

    public double maxSpeed = 0;
    public double avgSpeed = 0;


    public float totalReward = 0;

    public float rewardFromCheckpoints = 0;
    public float rewardFromFinish = 0;
    public float rewardFromTimeRunOut = 0;

    public float rewardFromWall = 0;

    public float rewardFromSpeed = 0;
    public float rewardFromTime = 0;
    public float rewardFromBrakes = 0;


}
