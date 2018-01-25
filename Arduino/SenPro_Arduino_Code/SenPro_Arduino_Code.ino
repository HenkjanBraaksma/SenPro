#include <TimerOne.h>
#include <SPI.h>
#include <EEPROM.h>
#include <boards.h>
#include <RBL_nRF8001.h>

void setup() {
  ble_set_name("SenProBLE");
  ble_begin();

  Timer1.initialize(1000000);
  Timer1.attachInterrupt(sendData);

  Serial.begin(9600);

}

void loop() {
  ble_do_events();
}

void sendData()
{
  String output = "";
  output = output + analogRead(0) + ',' + analogRead(1) + ',' 
           + analogRead(2) + ',' + analogRead(3);
  ble_write_bytes(output.c_str(), output.length());
}

