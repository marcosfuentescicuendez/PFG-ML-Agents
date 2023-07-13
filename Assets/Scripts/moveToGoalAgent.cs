using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class moveToGoalAgent : Agent {

    [SerializeField] private Transform stadio;
    [SerializeField] private Transform oppositeTransform;

    public Transform positionBlueGoalA, positionBlueGoalB, positionRedGoalA, positionRedGoalB;
    public RedGoalCounter rGoalA, rGoalB;
    public BlueGoalCounter bGoalA, bGoalB;
    public Timer timer;
    public BallCarrier ball;

    public CharacterController cc;

    //private float oldDistanceBall;

    private bool noLongerFather;

    public override void OnEpisodeBegin() {
        //Reset de los Agentes
        if (transform.name == "RedCharacter1") {
            transform.localPosition = new Vector3(0, 8, 10);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        } else {
            transform.localPosition = new Vector3(0, 8, -10);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (transform.name == "RedCharacter1") {
            //Reset de la Bola
            ball.transform.SetParent(null);
            ball.transform.localPosition = new Vector3(0, 9, 0);
            ball.rb.isKinematic = true;

            //Reset de marcadores
            bGoalA.score = 0;
            bGoalB.score = 0;
            bGoalA.scoreBoard.text = "Equipo Rojo: 0";
            rGoalA.score = 0;
            rGoalB.score = 0;
            rGoalA.scoreBoard.text = "Equipo Azul: 0";

            //Reset del timer
            timer.targetTime = 120.0f;
        }

        //oldDistanceBall = Vector3.Distance(transform.localPosition, ball.transform.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(ball.transform.localPosition);
        sensor.AddObservation(oppositeTransform.localPosition);

        sensor.AddObservation(positionBlueGoalA.transform.localPosition);
        sensor.AddObservation(positionBlueGoalB.transform.localPosition);

        sensor.AddObservation(positionRedGoalA.transform.localPosition);
        sensor.AddObservation(positionRedGoalB.transform.localPosition);

        sensor.AddObservation(stadio.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];
        if (actions.ContinuousActions[2] > 0 && ball.transform.parent != null) {
            if (ball.transform.parent.name == transform.name) {
                checkAngles();
                ball.shoot = true;
            }
        } else {
            ball.shoot = false;
        }
        float bForce = 10f;
        float rForce = 100f;

        transform.Rotate(Vector3.up * rForce * x * Time.deltaTime, Space.World); //Velocidad de giro horizontal
        transform.Rotate(Vector3.right * rForce * y * Time.deltaTime); //Velocidad de giro vertical
        cc.Move(transform.forward * bForce * Time.deltaTime); //Velocidad lineal
        
        allRewards();
    }

    //Control manual del objeto en el caso de no querer que se utilice el Agente
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment <float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        if (Input.GetKey("p")) {
            continuousActions[2] = 1f;
        } else {
            continuousActions[2] = -1f;
        }
    }

    private void OnTriggerEnter (Collider objX) {
        if (objX.gameObject.name == "Ball") {
            AddReward(50f);
        }
    }
    
    private void checkAngles() {
        //Recompensas si el jugador dispara la bola con un buen ángulo y buena posición
        if (transform.name == "RedCharacter1") {
            Vector3 distanceA = positionBlueGoalA.position - transform.position;
            Vector3 distanceB = positionBlueGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 30f && Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 30f && Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f)) {
                AddReward(60f);
            }
        } else {
            Vector3 distanceA = positionRedGoalA.position - transform.position;
            Vector3 distanceB = positionRedGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 30f && Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 30f && Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f)) {
                AddReward(60f);
            }
        }
    }

    private void allRewards() {

        //Recompensa si se acerca a la bola
        /*float distanceBall = Vector3.Distance(transform.localPosition, ball.transform.localPosition);
        
        if (distanceBall < oldDistanceBall) {
            AddReward(0.1f);
        }
        oldDistanceBall = distanceBall;*/

        if (transform.name == "RedCharacter1") {
            Vector3 distanceA = positionBlueGoalA.position - transform.position;
            Vector3 distanceB = positionBlueGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 30f && Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 30f && Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f)) {
                if (ball.transform.parent != null) {
                    if (ball.transform.parent.name == transform.name) {
                        AddReward(1f);
                    }
                }
            }
        } else {
            Vector3 distanceA = positionRedGoalA.position - transform.position;
            Vector3 distanceB = positionRedGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 30f && Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 30f && Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f)) {
                if (ball.transform.parent != null) {
                    if (ball.transform.parent.name == transform.name) {
                        AddReward(1f);
                    }
                }
            }
        }

        //Recompensa negativa si el jugador pierde la bola
        if (ball.fatherAsleep == "") {
            noLongerFather = true;
        }
        if (ball.fatherAsleep == transform.name && noLongerFather) {
            AddReward(-50f);
            noLongerFather = false;
        }

        //Recompensas si anota o pierde puntos
        if (bGoalA.toUpdate || bGoalB.toUpdate) {
            if (transform.name == "RedCharacter1") {
                    AddReward(100f);
                } else {
                    AddReward(-100f);
                }
        } else if (rGoalA.toUpdate || rGoalB.toUpdate) {
            if (transform.name == "BlueCharacter1") {
                    AddReward(100f);
                } else {
                    AddReward(-100f);
                }
        }

        //Recompensas al finalizar el encuentro
        if (timer.targetTime <= 0f) {
            int redGoals = bGoalA.score + bGoalB.score;
            int blueGoals = rGoalA.score + rGoalB.score;

            //Revisar cantidad de recompensas!!!!

            if (redGoals > blueGoals) {
                if (transform.name == "RedCharacter1") {
                    AddReward(100f);
                } else {
                    AddReward(-100f);
                }
            } else if (redGoals < blueGoals) {
                if (transform.name == "BlueCharacter1") {
                    AddReward(100f);
                } else {
                    AddReward(-100f);
                }
            } else {
                AddReward(-10f);
            }
            
            EndEpisode();
        }
    }
}
