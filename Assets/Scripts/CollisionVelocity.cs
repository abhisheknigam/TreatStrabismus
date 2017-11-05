using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVelocity : MonoBehaviour {

    private float timer = 0;
    Vector3 getRandomUpdate()
    {
        float x = Random.Range(-0.8f, 0.8f);
        float y = Random.Range(-0.8f, 0.8f);
        return new Vector3(x, y, 0);
    }

    private void Start()
    {
        timer = 0;
        gameObject.GetComponent<Rigidbody>().velocity = getRandomUpdate();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3)
        {
            timer = 0;
            gameObject.GetComponent<Rigidbody>().velocity = getRandomUpdate();
        }
    }

    void OnCollisionEnter(Collision c)
    {
        string name = gameObject.name;
        gameObject.GetComponent<Rigidbody>().velocity = -gameObject.GetComponent<Rigidbody>().velocity;

    }
}
