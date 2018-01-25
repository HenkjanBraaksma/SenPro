#include <perceptron.h>

#include <TimerOne.h>
#include <SPI.h>
#include <EEPROM.h>
#include <boards.h>
#include <RBL_nRF8001.h>

//Setup phase
float readings[4];

Perceptron sensorZeroInput;
Perceptron sensorOneInput;
Perceptron sensorTwoInput;
Perceptron allGate;
Perceptron noneGate;
Perceptron sensorZeroAbsence;
Perceptron sensorOneTwoPresence;
Perceptron nandGate;
Perceptron output;

void setup() {
  ble_set_name("SenProBLE");
  ble_begin();

  Timer1.initialize(1000000);
  Timer1.attachInterrupt(sendData);

  sensorZeroInput = Perceptron(75);
  sensorOneInput = Perceptron(130);
  sensorTwoInput = Perceptron(200);
  allGate = Perceptron(2.5);
  noneGate = Perceptron(-0.5);
  sensorZeroAbsence = Perceptron(-0.5);
  sensorOneTwoPresence = Perceptron(1.5);
  nandGate = Perceptron(-1.5);
  output = Perceptron(0.5);

  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
    
  Serial.begin(9600);

}

void loop() {
  ble_do_events();

  if(neuralNetwork(analogRead(0), analogRead(1), analogRead(2)))
  {
    digitalWrite(2, HIGH);
    digitalWrite(3, LOW);
    digitalWrite(4, LOW);
  }
  else
  {
    digitalWrite(2, LOW);
    digitalWrite(3, HIGH);
    digitalWrite(4, HIGH);
  }
}

void sendData()
{
  String output = "";
  output = output + analogRead(0) + ',' + analogRead(1) + ',' + analogRead(2) + ',' + analogRead(3) + ',' + analogRead(4) + ',' + analogRead(5);
  ble_write_bytes(output.c_str(), output.length());
  Serial.println(output);
}

bool neuralNetwork(float sensorZero, float sensorOne, float sensorTwo)
{
  sensorZeroInput.Input(sensorZero, 1);
  sensorOneInput.Input(sensorOne, 1);
  sensorTwoInput.Input(sensorTwo, 1);

  allGate.Input((sensorZeroInput.Output() + sensorOneInput.Output() + sensorTwoInput.Output()), 1);
  noneGate.Input((sensorZeroInput.Output() + sensorOneInput.Output() + sensorTwoInput.Output()), -1);
  sensorZeroAbsence.Input(sensorZeroInput.Output(), -1);
  sensorOneTwoPresence.Input((sensorOneInput.Output() + sensorTwoInput.Output()), 1);
  nandGate.Input((sensorZeroAbsence.Output() + sensorOneTwoPresence.Output()), -1);
  output.Input((allGate.Output() + noneGate.Output() + nandGate.Output()), 1);
  bool outputFinal = output.Output();

  sensorZeroInput.Reset(); sensorOneInput.Reset(); sensorTwoInput.Reset(); allGate.Reset(); 
  noneGate.Reset(); nandGate.Reset(); sensorZeroAbsence.Reset(); sensorOneTwoPresence.Reset();
  output.Reset();
  
  return outputFinal;
  
}

