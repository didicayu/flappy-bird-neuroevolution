using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class bird : MonoBehaviour
{
    public float velocity = 1f;
    private Rigidbody2D rb;

    [HideInInspector]
    public nn NeuralNetwork;

    [HideInInspector]
    public float[] inputs = new float[5];

    public GameObject PipesParent;

    [HideInInspector]
    public Transform[] pipes;
    public Transform Ceiling;

    [HideInInspector] //Commented out for debugging purposes
    public float score = 0, fitness = 0;
    float gameScore = 0;

    public TMP_Text gameScoreText;

    public bool SaveThisBrain = false;
    public string SaveName = "NeuralNetSave";
    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        PipesParent = GameObject.Find("PipeSpawner");


        NeuralNetwork = new nn(5, 8, 1);
        NeuralNetwork.LoadNeuralNetwork("NeuralNetSave");
    }
    void Start()
    {
        
    }

    Transform Closest;
    void Update()
    {

        //Calculate fitness values
        CalculateScore();


        if(transform.position.y > Ceiling.position.y){
            transform.position = new Vector3(transform.position.x, Ceiling.transform.position.y, transform.position.z);
        }

        if (!PipesParent.GetComponent<PipeSpawner>().StopSpawaning)
        {
            pipes = PipesParent.GetComponentsInDirectChildren<Transform>();
        }

        SetInputs();

        Think();

        if(SaveThisBrain){
            SaveBrain(SaveName);
            SaveThisBrain = false;
            Debug.Log("Brain Saved Succesfully");
        }
    }


    Transform PreviousPipe;

    void SetInputs()
    {
        ClosestPipe(pipes);

        inputs[0] = transform.position.y;
        inputs[4] = rb.velocity.normalized.y;

        if (Closest != null)
        {
            pipe SelectedPipeScript = Closest.GetComponent<pipe>();

            inputs[1] = SelectedPipeScript.TopPipe.position.normalized.y;
            inputs[2] = SelectedPipeScript.BottomPipe.position.normalized.y;
            Vector3 NormalizedDistance = (Closest.position - transform.position) / 10f;
            inputs[3] = NormalizedDistance.x;
            

            if(PreviousPipe != Closest && PreviousPipe != null){

                CalculateGameScore(Closest);

                SelectedPipeScript.amIClosestPipe = true;
                PreviousPipe.GetComponent<pipe>().amIClosestPipe = false;
            }
            else if(PreviousPipe == null){
                SelectedPipeScript.amIClosestPipe = true;
            }

            PreviousPipe = Closest;
        }
        else
        {
            inputs[1] = 0;
            inputs[2] = 0;
            inputs[3] = 0;
        }

        //Debug.Log(inputs[3]);
    }

    void CalculateGameScore(Transform closest){
        if(gameScoreText != null){
            gameScore++;
            gameScoreText.text = gameScore.ToString();
        }
    }

    float distance = 0;

    void ClosestPipe(Transform[] pipes)
    {

        float previousDistance = Mathf.Infinity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (transform.position.x < pipes[i].position.x + 0.7f)
            {
                distance = pipes[i].position.x - transform.position.x;
                if (distance < previousDistance)
                {
                    previousDistance = distance;
                    Closest = pipes[i];
                }
            }
        }
    }

    float BonusScore = 0;
    void CalculateScore(){
        score += (Time.deltaTime + BonusScore);
    }

    public void jump()
    {
        rb.velocity = Vector2.up * velocity;
    }

    [HideInInspector]
    public float output;

    void Think()
    {

        output = NeuralNetwork.FeedForward(inputs)[0];

        //Debug.Log(output);

        if (output > 0.5)
        {
            jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Obstacle"){
            gameObject.SetActive(false);
        }
    }

    void SaveBrain(string filename){
        NeuralNetwork.SaveNeuralNetwork(filename);
    }
}
