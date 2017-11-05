using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public AudioClip clip;
    public AudioSource source;

    public Transform gunBarrelTransform;
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 50f;                                     // Distance in Unity units over which the player can fire
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;

    private int bubblesCreated = 0;
    private int maxLevelBubbles = 0;
    public int Level = 1;
    private int bubbleMultiplyer=3;
    private float minX = 19.5f;
    private float maxX = 22;
    private float minY = 1.1f;
    private float maxY = 2f;
    private float minZ = 31f;
    private float maxZ = 33f;
    public GameObject bubble;

    private GameObject g;

    private float timer;

    //capturing data
    List<HitData> hitdata;
    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();
        if (Level == 1)
        {
            hitdata = new List<HitData>();
            maxLevelBubbles = Level * 3;
            CreateChicken();
            //CreateBubbles();
        }
    }

    void CreateChicken()
    {
        var randomPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        g = Instantiate(bubble, randomPos, Quaternion.identity);
        g.SetActive(true);

        //g.GetComponent<Rigidbody>().velocity = g.GetComponent<Rigidbody>().velocity * 2;
        
        bubblesCreated++;

    }

    void CreateBubbles()
    {
        int maxbubbles = bubbleMultiplyer * (Level);
        hitdata = new List<HitData>();
        while (bubblesCreated < maxbubbles)
        {
            var randomPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            GameObject g = Instantiate(bubble, randomPos, Quaternion.identity);
            g.SetActive(true);
            if (Level > 1)
            {
                g.GetComponent<Rigidbody>().velocity = new Vector3(0.03f*Level,0,0);
            }
            bubblesCreated++;
        }

    }

	// Update is called once per frame
	void Update () {
        if (OVRInput.Get(OVRInput.Button.One) && Time.time>nextFire)
        {
            // Update the time when our player can fire next
            nextFire = Time.time + fireRate;

            // Start our ShotEffect coroutine to turn our laser line on and off
            StartCoroutine(ShotEffect());

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition(0, gunEnd.position);
            
            RaycastGun();
        }

        timer += Time.deltaTime;
        if (timer > maxLevelBubbles * 5)
        {
            timer = 0;
            Destroy(g, 0.0f);
            hitdata.Add(new HitData(Time.time, Level, false));
            if (bubblesCreated == maxLevelBubbles)
            {
                
                SendData(hitdata);
                Level++;
                //CreateBubbles();
                maxLevelBubbles = Level * 3;
                bubblesCreated = 0;
                hitdata = new List<HitData>();
            }
            CreateChicken();
        }
    }

    private void RaycastGun()
    {
        RaycastHit hit;
        // Set the start position for our visual effect for our laser to the position of gunEnd
        laserLine.SetPosition(0, gunEnd.position);
        
        if (Physics.Raycast(gunBarrelTransform.position, -gunBarrelTransform.right, out hit))
        {
            
            if (hit.collider.gameObject.name.Contains("Chicken"))
            {
                hitdata.Add(new HitData(Time.time, Level, true));
                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);
                Destroy(hit.collider.gameObject);
                //bubblesCreated--;
                if (bubblesCreated==maxLevelBubbles)
                {
                    
                    SendData(hitdata);
                    Level++;
                    //CreateBubbles();
                    maxLevelBubbles = Level * 3;
                    bubblesCreated = 0;
                    hitdata = new List<HitData>();
                }
                CreateChicken();
            }
            else
            {
                hitdata.Add(new HitData(Time.time, Level, false));
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition(1, gunBarrelTransform.position + (-gunBarrelTransform.right * weaponRange));
            }

        }
        else
        {
            hitdata.Add(new HitData(Time.time, Level, false));
            // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
            laserLine.SetPosition(1, gunBarrelTransform.position + (-gunBarrelTransform.right * weaponRange));
        }
    }

    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        source.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }

    void MoveBubble(GameObject bub)
    {
        Vector3 v = Vector3.forward * (Level * 0.1f * Time.deltaTime);
        if(v.x > maxX || v.z > maxZ)
        {
            v = new Vector3(minX,v.y,minZ);
        }
        bub.transform.Translate(v);
    }

    public void SendData(List<HitData> hitdata)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("[");
       
        foreach(HitData hit in hitdata)
        {
            sb.Append(hit.getJson());
            sb.Append(",");
        }
        sb.Remove(sb.Length-1,1);
        sb.Append("]");

        //reset list
        hitdata = new List<HitData>();

        Dictionary<string,string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Cookie", "Our session cookie");

        byte[] pData = System.Text.Encoding.ASCII.GetBytes(sb.ToString().ToCharArray());

        WWW www = new WWW("http://140278b1.ngrok.io", pData, headers);
        StartCoroutine(WaitForWWW(www));

    }

    IEnumerator WaitForWWW(WWW www)
    {
        yield return www;


        string txt = "";
        if (string.IsNullOrEmpty(www.error))
            txt = www.text;  //text of success
        else
            txt = www.error;  //error
        print(txt);
    }
}
