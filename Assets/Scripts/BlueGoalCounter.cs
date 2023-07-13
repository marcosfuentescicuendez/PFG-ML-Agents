using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueGoalCounter : MonoBehaviour
{

    public Text scoreBoard;
    public BlueGoalCounter bGoalA;
    public Rigidbody ballRb;
    public int score = 0;
    public bool toUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard.text = "Equipo Rojo: 0";
    }

    // Update is called once per frame
    void Update()
    {
        if (toUpdate == true) {
            scoreBoard.text = "Equipo Rojo: " + (score + bGoalA.score);
            //Debug.Log(score + ", " + bGoalA.score);
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