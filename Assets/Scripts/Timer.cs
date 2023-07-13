using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 public class Timer: MonoBehaviour {
    
    public float targetTime = 120.0f;
    public Text timer;
    
    public moveToGoalAgent pRed, pBlue;

    public BallCarrier ball;
    
    void Update(){
    
        //Controlador del tiempo
        if (targetTime > 0) {
            targetTime -= Time.deltaTime;
            timer.text = timeFormat(targetTime);
        } else {
            timer.text = "END";
            timerEnded();
        }
    
    }
    
    void timerEnded() { //Efecto tras la finalización del tiempo
        //Cambios para el entrenamiento
        /*
        ball.stop = true;
        pRed.stop = true;
        pBlue.stop = true;
        */
    }
 
    string timeFormat(float time) { //Se encarga de transformar el formato de visualización del timer
        int sec = (int) time;
        int min;
        string result = "";
        if (sec > 60) {
            min = sec/60;
            sec = sec%60;
            string addMin = min.ToString();
            if (min < 10) {
                addMin = "0" + addMin;
            }
            result = addMin + ":";
        }
        string addSec = sec.ToString();
        if (sec < 10) {
            addSec = "0" + addSec;
        }

        return result + addSec;
    }
 
 }
