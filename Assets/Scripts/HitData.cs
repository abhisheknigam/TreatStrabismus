using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitData {

    public float time;

    public int level;

    public bool isHit;

    public float nearestBubbleDistance;

    public HitData(float time, int level, bool isHit)
    {
        this.time = time;
        this.level = level;
        this.isHit = isHit;
    }

    public string getJson()
    {
        return "{\"time\":" + time + ",\"level\":"+level+",\"isHit\":"+isHit.ToString().ToLower()+"}";
    }
}
