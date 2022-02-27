using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiagramController : MonoBehaviour
{
    public GameObject inputLayer;
    public GameObject hiddenLayer;
    public GameObject outputLayer;

    Transform[] inputChilden;
    Transform[] hiddenChilden;
    Transform[] outputChilden;

    LineRenderer[][] inputLines;
    TMP_Text[] inputText;
    TMP_Text outputText;

    GameObject[][] LineHoldersI;
    GameObject[] LineHoldersH;

    public float width = 0.2f;

    public GameObject bird;
    bird BScipt;
    GameObject go;
    nn Brain;


    // Start is called before the first frame update
    void Start()
    {
        go = new GameObject("Line");

        inputChilden = inputLayer.GetComponentsInDirectChildren<Transform>();
        hiddenChilden = hiddenLayer.GetComponentsInDirectChildren<Transform>();
        outputChilden = outputLayer.GetComponentsInDirectChildren<Transform>();

        LineHoldersI = new GameObject[inputChilden.Length][];
        LineHoldersH = new GameObject[hiddenChilden.Length];

        Material mat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

        for (int i = 0; i < LineHoldersI.Length; i++)
        {
            LineHoldersI[i] = new GameObject[hiddenChilden.Length];
        }

        BScipt = bird.GetComponent<bird>();
        Brain = BScipt.NeuralNetwork;

        inputText = new TMP_Text[inputChilden.Length];
        outputText = outputChilden[0].GetComponentInChildren<TMP_Text>();

        ///Assign Input-Hidden Lines
        for (int i = 0; i < inputChilden.Length; i++)
        {
            inputText[i] = inputChilden[i].GetComponentInChildren<TMP_Text>();
            for (int j = 0; j < hiddenChilden.Length; j++)
            {
                LineHoldersI[i][j] = Instantiate(go);
                LineHoldersI[i][j].transform.SetParent(inputChilden[i].transform);
                LineHoldersI[i][j].AddComponent<LineRenderer>();

                LineHoldersI[i][j].GetComponent<LineRenderer>().SetPosition(0, new Vector3(inputChilden[i].position.x, inputChilden[i].position.y, -1));
                LineHoldersI[i][j].GetComponent<LineRenderer>().SetPosition(1, new Vector3(hiddenChilden[j].position.x, hiddenChilden[j].position.y, -1));

                LineHoldersI[i][j].GetComponent<LineRenderer>().startWidth = width;
                LineHoldersI[i][j].GetComponent<LineRenderer>().endWidth = width;

                LineHoldersI[i][j].GetComponent<LineRenderer>().material = mat;
                LineHoldersI[i][j].GetComponent<Renderer>().sortingLayerName = "Foreground";
                LineHoldersI[i][j].GetComponent<Renderer>().sortingOrder = 4;

                if (Brain.weights_ih.matrix[j, i] >= 0)
                {
                    LineHoldersI[i][j].GetComponent<LineRenderer>().startColor = new Color(Brain.weights_ih.matrix[j, i], 0, 0);
                    LineHoldersI[i][j].GetComponent<LineRenderer>().endColor = new Color(Brain.weights_ih.matrix[j, i], 0, 0);
                }
                else
                {
                    LineHoldersI[i][j].GetComponent<LineRenderer>().startColor = new Color(0, 0, Brain.weights_ih.matrix[j, i] * -1);
                    LineHoldersI[i][j].GetComponent<LineRenderer>().endColor = new Color(0, 0, Brain.weights_ih.matrix[j, i] * -1);
                }

            }
        }

        ///Assign Hidden-Output Lines
        for (int i = 0; i < hiddenChilden.Length; i++)
        {
            LineHoldersH[i] = Instantiate(go);
            LineHoldersH[i].transform.SetParent(hiddenChilden[i].transform);

            LineHoldersH[i].gameObject.AddComponent<LineRenderer>();
            LineRenderer lr = LineHoldersH[i].gameObject.GetComponent<LineRenderer>();
            Renderer ren = LineHoldersH[i].GetComponent<Renderer>();

            ren.sortingOrder = 4;

            lr.SetPosition(0, new Vector3(hiddenChilden[i].position.x,hiddenChilden[i].position.y,-1));
            lr.SetPosition(1, new Vector3(outputChilden[0].position.x,outputChilden[0].position.y, -1));

            lr.startWidth = width;
            lr.endWidth = width;

            lr.material = mat;
            
            
            
            ren.sortingLayerName = "Foreground";
            

            for (int j = 0; j < outputChilden.Length; j++)
            {
                if (Brain.weights_ho.matrix[j, i] >= 0)
                {
                    lr.startColor = new Color(Brain.weights_ho.matrix[j, i], 0, 0);
                    lr.endColor = new Color(Brain.weights_ho.matrix[j, i], 0, 0);
                }
                else
                {
                    lr.startColor = new Color(0, 0, Brain.weights_ho.matrix[j, i] * -1);
                    lr.endColor = new Color(0, 0, Brain.weights_ho.matrix[j, i] * -1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < inputChilden.Length; i++)
        {
            inputText[i].text = BScipt.inputs[inputText.Length-i-1].ToString("0.00");
        }

        outputText.text = BScipt.output.ToString("0.00");
    }
}
