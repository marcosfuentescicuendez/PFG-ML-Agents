using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class scriptAgent : Agent {

    [SerializeField] private Transform oppositeTransform;

    public Transform positionBlueGoalA, positionBlueGoalB, positionRedGoalA, positionRedGoalB, positionBall;
    public RedGoalCounter rGoalA, rGoalB;
    public BlueGoalCounter bGoalA, bGoalB;
    public Timer timer;
    public BallCarrier ball;

    //private Rigidbody agentRigidbody;
    //private Rigidbody ballRigidbody;

    public CharacterController cc;

    private bool noLongerFather;

    private bool posesion;

    private float temp, tpos;

    public override void OnEpisodeBegin() {
        //Reset de los Agentes
        if (transform.name == "RedCharacter1") {
            transform.position = new Vector3(0, 8, 10);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        } else {
            transform.position = new Vector3(0, 8, -10);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (transform.name == "RedCharacter1") {
            //Reset de la Bola
            ball.transform.SetParent(null);
            //Metodo de aleatorización
            float x = Random.Range(-7, 7);
            float y = Random.Range(5, 13);
            ball.transform.position = new Vector3(x, y, 0);
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
        temp = 0f;
    }

    public override void CollectObservations(VectorSensor sensor) {
        //Observaciones del personaje del agente
        sensor.AddObservation(transform.forward);
        //sensor.AddObservation(transform.position);
        //sensor.AddObservation(agentRigidbody.velocity);

        //Observación del otro agente
        //Vector3 distanceAgent = oppositeTransform.position - transform.position;
        //sensor.AddObservation(distanceAgent);

        //Observaciones de la bola
        Vector3 distanceBall = positionBall.position - transform.position;
        sensor.AddObservation(distanceBall.normalized);
        //sensor.AddObservation(ballRigidbody.velocity);

        //Observaciones de los aros
        if (transform.name == "RedCharacter1") {
            Vector3 distanceBlueA = positionBlueGoalA.position - transform.position;
            Vector3 distanceBlueB = positionBlueGoalB.position - transform.position;
            sensor.AddObservation(distanceBlueA.normalized);
            sensor.AddObservation(distanceBlueB.normalized);
        } else {
            Vector3 distanceRedA = positionRedGoalA.position - transform.position;
            Vector3 distanceRedB = positionRedGoalB.position - transform.position;
            sensor.AddObservation(distanceRedA.normalized);
            sensor.AddObservation(distanceRedB.normalized);
        }

        //Observaciones de quien tiene la pelota (float)
        if (ball.transform.parent != null) {
            if (ball.transform.parent.name == "RedCharacter1" && transform.name == "RedCharacter1") {
                posesion = true;
            } else if (ball.transform.parent.name == "BlueCharacter1" && transform.name == "BlueCharacter1") {
                posesion = true;
            } else {
                posesion = false;
            }
        } else {
            posesion = false;
        }
        sensor.AddObservation(posesion);

        /*float blueScore = bGoalA.score + bGoalB.score;
        float redScore = rGoalA.score + rGoalB.score;
        sensor.AddObservation(blueScore);
        sensor.AddObservation(redScore);*/

        //Observaciones de los ángulos y distancias con respecto a los aros del otro equipo, respectivamente
        /*if (transform.name == "RedCharacter1") {
            //Vector3 distanceA = positionBlueGoalA.position - transform.position;
            //Vector3 distanceB = positionBlueGoalB.position - transform.position;

            //sensor.AddObservation(distanceA);
            //sensor.AddObservation(distanceB);

            sensor.AddObservation(Vector3.Angle(transform.forward, distanceBlueA));
            sensor.AddObservation(Vector3.Angle(transform.forward, distanceBlueB));
            sensor.AddObservation(Vector3.Angle(positionBlueGoalA.forward, distanceBlueA));
            sensor.AddObservation(Vector3.Angle(positionBlueGoalB.forward, distanceBlueB));
        } else {
            //Vector3 distanceA = positionRedGoalA.position - transform.position;
            //Vector3 distanceB = positionRedGoalB.position - transform.position;

            //sensor.AddObservation(distanceA);
            //sensor.AddObservation(distanceB);

            sensor.AddObservation(Vector3.Angle(transform.forward, distanceRedA));
            sensor.AddObservation(Vector3.Angle(transform.forward, distanceRedB));
            sensor.AddObservation(Vector3.Angle(positionRedGoalA.forward, distanceRedA));
            sensor.AddObservation(Vector3.Angle(positionRedGoalB.forward, distanceRedB));
        }*/
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
            AddReward(0.1f);//comprobar cantidad
        }
    }
    
    private void checkAngles() {
        //Recompensas si el jugador dispara la bola con un buen ángulo y buena posición
        if (transform.name == "RedCharacter1") {
            Vector3 distanceA = positionBlueGoalA.position - transform.position;
            Vector3 distanceB = positionBlueGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 40f && Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 40f && Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f)) {
                float t;
                if (Vector3.Angle(transform.forward, distanceA) > Vector3.Angle(transform.forward, distanceB)) {
                    t = 1f - (Vector3.Angle(transform.forward, distanceB)/40f);
                } else {
                    t = 1f - (Vector3.Angle(transform.forward, distanceA)/40f);
                }
                AddReward(Mathf.Lerp(1f, 1.3f, t));
            }
            /* else {
                AddReward(0.4f);
            }*/
        } else {
            Vector3 distanceA = positionRedGoalA.position - transform.position;
            Vector3 distanceB = positionRedGoalB.position - transform.position;
            if ((Vector3.Angle(transform.forward, distanceA) <= 40f && Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f) || 
            (Vector3.Angle(transform.forward, distanceB) <= 40f && Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f)) {
                float y;
                if (Vector3.Angle(transform.forward, distanceA) > Vector3.Angle(transform.forward, distanceB)) {
                    y = 1f - (Vector3.Angle(transform.forward, distanceB)/40f);
                } else {
                    y = 1f - (Vector3.Angle(transform.forward, distanceA)/40f);
                }
                AddReward(Mathf.Lerp(1f, 1.3f, y));
            }
            /* else {
                AddReward(0.4f);
            }*/
        }
    }

    private void allRewards() {

        //Recompensas si el jugador tiene un buen ángulo y buena posición (teniendo la bola en su posesion)
        if (transform.name == "RedCharacter1") {
            Vector3 distanceA = positionBlueGoalA.position - transform.position;
            Vector3 distanceB = positionBlueGoalB.position - transform.position;
            if (Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f || Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f) {
                if (ball.transform.parent != null) {
                    if (ball.transform.parent.name == transform.name) {
                        float t = 0f;
                        if (Vector3.Angle(transform.forward, distanceA) > Vector3.Angle(transform.forward, distanceB)) {
                            if (Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceB)/180f);
                            } else if (Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceA)/180f);
                            }
                        } else {
                            if (Vector3.Angle(positionBlueGoalA.forward, distanceA) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceA)/180f);
                            } else if (Vector3.Angle(positionBlueGoalB.forward, distanceB) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceB)/180f);
                            }
                        }
                        AddReward(Mathf.Lerp(-0.2f, 0.2f, t));
                    }
                }
            }
        } else {
            Vector3 distanceA = positionRedGoalA.position - transform.position;
            Vector3 distanceB = positionRedGoalB.position - transform.position;
            if (Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f || Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f) {
                if (ball.transform.parent != null) {
                    if (ball.transform.parent.name == transform.name) {
                        float t = 0f;
                        if (Vector3.Angle(transform.forward, distanceA) > Vector3.Angle(transform.forward, distanceB)) {
                            if (Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceB)/180f);
                            } else if (Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceA)/180f);
                            }
                        } else {
                            if (Vector3.Angle(positionRedGoalA.forward, distanceA) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceA)/180f);
                            } else if (Vector3.Angle(positionRedGoalB.forward, distanceB) <= 45f) {
                                t = 1f - (Vector3.Angle(transform.forward, distanceB)/180f);
                            }
                        }
                        AddReward(Mathf.Lerp(-0.2f, 0.2f, t));
                    }
                }
            }
        }

        //Penalización por mantener la bola en su poder por demasiado tiempo
        /*if (ball.transform.parent != null) {
             if (ball.transform.parent.name == transform.name) {
                tpos += Time.deltaTime;
                float p = 1 - tpos/120f;
                AddReward(Mathf.Lerp(-0.1f, -0.01f, p));
            } else {
                tpos = 0f;
            }
        } else {
            tpos = 0f;
        }*/

        //Recompensa por llevar la pelota
        if (ball.transform.parent != null) {
            if (ball.transform.parent.name == transform.name) {
                AddReward(0.05f);
            }
        }

        //Recompensa por mirar hacia la pelota
        if (ball.transform.parent == null) {
            //if (Vector3.Angle(transform.forward, (positionBall.position - transform.position)) < 90f) {
                float m = 1 - (Vector3.Angle(transform.forward, (positionBall.position - transform.position))/180f);
                AddReward(Mathf.Lerp(-0.05f, 0.05f, m));
            //}
            //magnitude(transform.forward - (positionBall.position - transform.position).normalized);
            //2 muy malo - 0 muy bueno (MathF.clamp(x,0,1)--> lerp del resultado)
        } else {
            if (ball.transform.parent.name == oppositeTransform.name) {
                //if (Vector3.Angle(transform.forward, (positionBall.position - transform.position)) < 90f) {
                    float m = 1 - (Vector3.Angle(transform.forward, (positionBall.position - transform.position))/180f);
                    AddReward(Mathf.Lerp(-0.05f, 0.05f, m));
                //}
            }
        }

        //Penalización por no tener la bola
        /*if (ball.transform.parent != null) {
            if (ball.transform.parent.name == oppositeTransform.name) {
                temp += Time.deltaTime;
                if (temp > 5f) {
                    float g = 1 - temp/120f;
                    //float k = (Mathf.Lerp(-1f, -0.5f, (timer.targetTime/120f)));
                    AddReward(Mathf.Lerp(-0.5f, -0.05f, g));
                }
            } else {
                temp = 0f;
            }
        } else {
            temp += Time.deltaTime;
            if (temp > 5f) {
                float g = 1 - temp/120f;
                //float k = (Mathf.Lerp(-1f, -0.5f, (timer.targetTime/120f)));
                AddReward(Mathf.Lerp(-0.5f, -0.05f, g));
            }
        }*/

        //Recompensa por distancia a la bola
        /*float distanceBall = Vector3.Distance(ball.transform.position, transform.position);
        if (distanceBall <= 20 && ball.transform.parent == null) {
            float m = 1f - (distanceBall/20f);
            AddReward(Mathf.Lerp(0.01f, 0.1f, m));
        }*/

        //Recompensa negativa si el jugador pierde la bola
        if (ball.fatherAsleep == "") {
            noLongerFather = true;
        }
        if (ball.fatherAsleep == transform.name && noLongerFather) {
            AddReward(-1f);//comprobar cantidad
            noLongerFather = false;
        }

        //Recompensas si anota o pierde puntos
        if (bGoalA.toUpdate || bGoalB.toUpdate) {
            if (transform.name == "RedCharacter1") {
                    AddReward(1f);
                } else {
                    AddReward(-1f);
                }
        } else if (rGoalA.toUpdate || rGoalB.toUpdate) {
            if (transform.name == "BlueCharacter1") {
                    AddReward(1f);
                } else {
                    AddReward(-1f);
                }
        }

        //Recompensas al finalizar el encuentro
        if (timer.targetTime <= 0f) {
            int redGoals = bGoalA.score + bGoalB.score;
            int blueGoals = rGoalA.score + rGoalB.score;

            if (redGoals > blueGoals) {
                if (transform.name == "RedCharacter1") {
                    AddReward(1f);
                } else {
                    AddReward(-1f);
                }
            } else if (redGoals < blueGoals) {
                if (transform.name == "BlueCharacter1") {
                    AddReward(1f);
                } else {
                    AddReward(-1f);
                }
            } else {
                AddReward(-0.1f);
            }
            
            EndEpisode();
        }
    }
}
