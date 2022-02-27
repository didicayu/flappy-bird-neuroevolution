using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationController : MonoBehaviour
{
    public GameObject bird;
    public int NumOfBirds = 100;

    public GameObject Spawner;
    PipeSpawner pipspawn;

    GameObject[] birds;

    int GenNum = 0;

    [Range(0.1f,3.5f)]
    public float TimeSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        pipspawn = Spawner.GetComponent<PipeSpawner>();

        birds = new GameObject[NumOfBirds];
        for (int i = 0; i < birds.Length; i++)
        {
            birds[i] = Instantiate(bird, transform);
            birds[i].transform.position = transform.position;
        }
    }

    int NumInactive = 0;
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = TimeSpeed;

        foreach (GameObject ocell in birds)
        {
            if (!ocell.activeSelf)
            {
                NumInactive++;
            }
        }

        if (NumInactive == birds.Length)
        {
            NextGeneration();
            GenNum++;
            Debug.Log("Generation nº" + GenNum);
        }
        NumInactive = 0;
        /*
        if(Input.GetMouseButtonDown(0)){
            NextGeneration();
        }
        */
    }
    float HighestFitness = 0;
    void NextGeneration()
    {

        CalculateFitness();

        //Reset All pipes
        //pipspawn.SendAllBack(pipes);
        pipspawn.Reset();

        foreach (GameObject ocell in birds)
        {
            if (HighestFitness < ocell.GetComponent<bird>().fitness)
            {
                HighestFitness = ocell.GetComponent<bird>().fitness;        
            }
        }
        Debug.Log(HighestFitness);

        foreach (GameObject ocell in birds)
        {
            if(ocell.GetComponent<bird>().fitness == HighestFitness || HighestFitness == 0){
                ocell.GetComponent<bird>().NeuralNetwork.SaveNeuralNetwork("nn4");
            }

            ocell.GetComponent<bird>().fitness = 0;
            ocell.GetComponent<bird>().score = 0;

            ocell.transform.position = transform.position;
            ocell.SetActive(true);

            ocell.GetComponent<bird>().NeuralNetwork.LoadNeuralNetwork("nn4");
            ocell.GetComponent<bird>().NeuralNetwork.Mutate(MutationFunction);
            
        }

        //HighestFitness = 0;
    }

    float MutationFunction(float x)
    {
        if (Random.Range(0f, 1f) < 0.1f)
        {
            float offset = RandomGaussian() * 0.5f;
            float newx = x + offset;
            return newx;
        }
        else
        {
            return x;
        }
    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    void CalculateFitness()
    {

        float sum = 0;
        foreach (GameObject ocell in birds)
        {
            sum += ocell.GetComponent<bird>().score;
        }

        foreach (GameObject ocell in birds)
        {
            ocell.GetComponent<bird>().fitness = ocell.GetComponent<bird>().score / sum;
        }
    }
}
