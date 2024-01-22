#include <SPI.h>
#include <math.h>

#define SERIESRESISTOR 10000                           // the value of the 'other' resistor
#define THERMISTORPIN1 A7                              // Pin for Thermistor #1
#define THERMISTORPIN2 A6                              // Pin for Thermistor #2
#define THERMISTORNOMINAL 10000                        // Thermistor nominal resistance (10k)
#define TEMPERATURENOMINAL 25                          // Temp. for nominal resistance (almost always 25 C)
#define NUMSAMPLES 10                                  // how many samples to take and average, more samples take more time. Averaging reduces drastic fluctiations.
#define BCOEFFICIENT 3435                              // The beta coefficient of the thermistor (usually 3000-4000)

#define WAIT_TIME 1000

void setup() {
  Serial.begin(9600);
}

void take_reading(float &average1, float &average2) {
  average1 = 0.0;
  average2 = 0.0;

  for (byte i = 0; i < NUMSAMPLES; i++) {
    float currentRead1 = analogRead(THERMISTORPIN1);  // Read from first thermistor
    float currentRead2 = analogRead(THERMISTORPIN2);  // Read from second thermistor

    // Calculate resistance for the first thermistor
    currentRead1 = (1023 / currentRead1) - 1;          
    currentRead1 = SERIESRESISTOR / currentRead1;       
    average1 += currentRead1;

    // Calculate resistance for the second thermistor
    currentRead2 = (1023 / currentRead2) - 1;          
    currentRead2 = SERIESRESISTOR / currentRead2;       
    average2 += currentRead2;
    delay(50)
  }

  average1 /= NUMSAMPLES;  // Average resistance for the first thermistor
  average2 /= NUMSAMPLES;  // Average resistance for the second thermistor
}

float calculate_temp(float resistance) {
  float steinhart = resistance / THERMISTORNOMINAL;    // (R/Ro)
  steinhart = log(steinhart);                          // ln(R/Ro)
  steinhart /= BCOEFFICIENT;                           // 1/B * ln(R/Ro)
  steinhart += 1.0 / (TEMPERATURENOMINAL + 273.15);    // + (1/To)
  steinhart = 1.0 / steinhart;                         // Invert
  steinhart -= 273.15;                                 // convert absolute temp to C
  return steinhart;
}

void json_temp(float temp1, float temp2) {             // Outputs in format: {"temp1":22.66,"temp2":22.50}
  Serial.print("{\"temp1\":");
  Serial.print(temp1);
  Serial.print(",\"temp2\":");
  Serial.print(temp2);
  Serial.println("}");
}

void loop() {
  float averageResistance1, averageResistance2;
  take_reading(averageResistance1, averageResistance2);

  float temp1 = calculate_temp(averageResistance1);
  float temp2 = calculate_temp(averageResistance2);

  json_temp(temp1, temp2);
  //Serial.println(averageResistance1);
  //Serial.println(averageResistance2);

  delay(WAIT_TIME);
}
