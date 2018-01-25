#include <perceptron.h>

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
  sensorZeroInput = Perceptron(0.5);
  sensorOneInput = Perceptron(0.5);
  sensorTwoInput = Perceptron(0.5);
  allGate = Perceptron(2.5);
  noneGate = Perceptron(-0.5);
  sensorZeroAbsence = Perceptron(-0.5);
  sensorOneTwoPresence = Perceptron(1.5);
  nandGate = Perceptron(-1.5);
  output = Perceptron(0.5);

  Serial.begin(9600);
}

void loop() {
  int sensorZero = 0;
  int sensorOne = 1;
  int sensorTwo = 0;

  Serial.println("The outcome of the network is");
  Serial.println(neuralNetwork(sensorZero, sensorOne, sensorTwo));

  sensorZeroInput.Reset();
  sensorOneInput.Reset();
  sensorTwoInput.Reset();
  allGate.Reset();
  noneGate.Reset();
  sensorZeroAbsence.Reset();
  sensorOneTwoPresence.Reset();
  nandGate.Reset();
  output.Reset();
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

  return output.Output();
  
}



