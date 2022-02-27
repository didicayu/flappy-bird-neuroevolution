using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public float maxTime = 1;
    private float timer = 0;
    public int maxPipes = 10;

    public GameObject pipe;
    Transform[] pipes;
    public float height;

    [HideInInspector]
    public bool StopSpawaning = false;
    int numPipes;

    // Start is called before the first frame update
    void Start()
    {
        pipes = new Transform[maxPipes];
        timer = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > maxTime && !StopSpawaning)
        {

            GameObject NewPipe = Instantiate(pipe);
            NewPipe.transform.parent = gameObject.transform;
            NewPipe.transform.position = transform.position + new Vector3(0, Random.Range(-height, height), 0);

            numPipes++;
            timer = 0;
        }

        if (numPipes >= maxPipes)
        {
            StopSpawaning = true;
        }

        pipes = gameObject.GetComponentsInDirectChildren<Transform>();

        timer += Time.deltaTime;
    }

    public void SendBack(GameObject TriggeredPipe)
    {
        if (timer > maxTime)
        {
            TriggeredPipe.transform.position = new Vector2(transform.position.x, Random.Range(-height, height));
            timer = 0;
        }
    }
    public void Reset(){
        timer = maxTime;
        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }

        numPipes = 0;
        StopSpawaning = false;
    }
}
