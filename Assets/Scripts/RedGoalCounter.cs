using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedGoalCounter : MonoBehaviour
{

    public Text scoreBoard;
    public RedGoalCounter rGoalA;
    public Rigidbody ballRb;
    public int score = 0;
    public bool toUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard.text = "Equipo Azul: 0";
    }

    // Update is called once per frame
    void Update()
    {
        if (toUpdate == true) {
            scoreBoard.text = "Equipo Azul: " + (score + rGoalA.score);
            //Debug.Log(score + ", " + rGoalA.score);
            toUpdate = false;
        }
    }

    void OnTriggerEnter(Collider objX) {
        if (objX.gameObject.name == "Ball" && ballRb.isKinematic == false) {
            score = score + 10;
            toUpdate = true;
        }
    }    
}