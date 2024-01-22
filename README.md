# XIAO-M0-liquid-temperature-sensor
This is a fork from [Naata/Trinket-M0-liquid-temperature-sensor](https://github.com/Naata/Trinket-M0-liquid-temperature-sensor/tree/main) which I used as base, and modified to my particular use case, and needs. 
In my scenario, I wanted to monitor both Air/Ambient temperature and Water temperature, and have my radiator fan curves be a function of the air/water temperature difference. 

This also uses [Newtonsoft.Json](https://www.newtonsoft.com/json) to handle json parsing. There is a .NET alternative "System.Text.Json" but I lack the knowledge required to understand why I could not make it work.

The Arduino code is derived from Naata's and [Adafruit | Using a Thermistor](https://learn.adafruit.com/thermistor/using-a-thermistor) page, essentially increasing the number of measurements and the delay between these in an effort to improve accuracy. Lastly, the output is JSON instead of a single temperature value to feed into FanControl.

## PART 1. The Sensors

Most watercooling temp sensors are 10k Ohm thermistors. I used:
- [Alphacool Eiszapfen inline temperature sensor | Part #17362](https://shop.alphacool.com/en/shop/controllers-and-sensors/temperature-sensor/17362-alphacool-eiszapfen-temperature-sensor-g1/4-ig/ig-with-ag-adapter-chrome)
- [Alphacool Eiszapfen temperature sensor plug | Part #17364](https://shop.alphacool.com/en/shop/controllers-and-sensors/temperature-sensor/17364-alphacool-eiszapfen-temperature-sensor-plug-g1/4-chrome)

For both, Alphacool provides the same [Thermistor Datasheet](https://www.alphacool.com/download/kOhm_Sensor_Table_Alphacool.pdf) and this is useful to tune the calculated resistance values and improve accuracy. Also, means that the parts can be connected to either port in the pcb.

A future version of this might actually use the table itself. 

## PART 2. Microcontroller

I opted for the Seeed XIAO SAMD21, and just like the Adafruit Trinket M0, it can:
- Natively output serial via USB (which is great since I wanted to use the internal USB2 header in the motherboard and splice the other end of the cable to a type-c interface)
- Be seen in Windows as a COM device. No RGB in this case, which I preferred, although do not deny the convenience of a color coded "alert"

## PART 3. Assembly
TBD add images / insights from project. 

## PART 4. Arduino IDE

Before this project, I'd tap into [OpenWeather's Weather API](https://openweathermap.org/api), get temperature for my city via powershell, and update a .sensor file read from Fan Control. 
Code: [Arduino](https://github.com/gmoorec/XIAO-M0-Dual-Temperature-Sensor/blob/main/src/xiao_samd21_temp_sensor.ino)
I opted to output both sensor readings as a JSON formatted string (e.g.: {"temp1":22.66,"temp2":22.50}) which then the DLL in the next section parses and turns into two sensors readable from FanControl.

## PART 5. FanControl Plugin
-  Download [Fan Control](https://github.com/Rem0o/FanControl.Releases).
-  Download latest plugin DLL from [Releases](https://github.com/gmoorec/XIAO-M0-Dual-Temperature-Sensor/releases/).
-  Use instructions from [FanControl Wiki](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins#requirements).
-  Set environment variable SENSOR_COM_PORT to whichever com port your trinket is connected to
-  Profit. 
