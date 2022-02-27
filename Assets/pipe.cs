using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipe : MonoBehaviour
{
    GameObject PipeTrigger;
    public float speed = 0.2f;

    PipeSpawner spawner;

    public Transform TopPipe;
    public Transform BottomPipe;
    public Sprite SelectedPipe;
    public Sprite TopSprite;
    public Sprite BottomSprite;

    SpriteRenderer TopPipeParent;
    SpriteRenderer BottomPipeParent;

    [HideInInspector]
    public bool amIClosestPipe = false;

    // Start is called before the first frame update
    void Start()
    {
        PipeTrigger = GameObject.Find("Pipetrigger");
        spawner = gameObject.GetComponentInParent<PipeSpawner>();

        TopPipeParent = TopPipe.GetComponentInParent<SpriteRenderer>();
        BottomPipeParent = BottomPipe.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < PipeTrigger.transform.position.x){
            spawner.SendBack(gameObject);
            spawner.StopSpawaning = true;
        }
        
        if(amIClosestPipe){
            TopPipeParent.sprite = SelectedPipe;
            TopPipeParent.flipY = true;
            BottomPipeParent.sprite = SelectedPipe;
        }
        else
        {
            TopPipeParent.sprite = TopSprite;
            TopPipeParent.flipY = false;
            BottomPipeParent.sprite = BottomSprite;
        }
    }

    void FixedUpdate() {
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }
}
