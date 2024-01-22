# XIAO-M0-liquid-temperature-sensor
This is a fork from [Naata/Trinket-M0-liquid-temperature-sensor](https://github.com/Naata/Trinket-M0-liquid-temperature-sensor/tree/main) which I used as base, and modified to my particular use case, and needs. 
In my scenario, I wanted to monitor both Air/Ambient temperature and Water temperature, and have my radiator fan curves be a function of the air/water temperature difference. 

## PART 1. The Sensors

Most watercooling temp sensors are 10k Ohm thermistors. I used:
- [Alphacool Eiszapfen inline temperature sensor | Part #17362](https://shop.alphacool.com/en/shop/controllers-and-sensors/temperature-sensor/17362-alphacool-eiszapfen-temperature-sensor-g1/4-ig/ig-with-ag-adapter-chrome)
- [Alphacool Eiszapfen temperature sensor plug | Part #17364](https://shop.alphacool.com/en/shop/controllers-and-sensors/temperature-sensor/17364-alphacool-eiszapfen-temperature-sensor-plug-g1/4-chrome)

For both, Alphacool provides the same [Thermistor Datasheet](https://www.alphacool.com/download/kOhm_Sensor_Table_Alphacool.pdf) and this is useful to tune the calculated resistance values and improve accuracy. Also, means that the parts can be connected to either port in the pcb.

## PART 2. Microcontroller

I opted for the Seeed XIAO SAMD21, and just like the Adafruit Trinket M0, it can:
- Natively output serial via USB (which is great since I wanted to use the internal USB2 header in the motherboard and splice the other end of the cable to a type-c interface)
- Be seen in Windows as a COM device. No RGB in this case, which I preferred, although do not deny the convenience of a color coded "alert"

## PART 3. Assembly

##PART 4. Arduino IDE

Before this project, I'd tap into [OpenWeather's Weather API](https://openweathermap.org/api), get temperature for my city via powershell, and update a .sensor file read from Fan Control. 
Arduino code link

I opted to output both sensor readings as a JSON formatted string.

## FanControl Plugin
1. I use tool called [Fan Control](https://github.com/Rem0o/FanControl.Releases). Download latest plugin DLL from [Releases](https://github.com/Naata/Trinket-M0-liquid-temperature-sensor/releases). Use instructions from [FanControl Wiki](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins#requirements).
2. Set environment variable SENSOR_COM_PORT to whichever com port your trinket is connected to
2. Works! :)

------

3. Solder some goldpins to your **Trinket M0** - pin **A4** will be used as analog input, which is conveniant as **GND** is right next to it.

4. Flash [trinket_m0_temp_sensor.ino](src/trinket_m0_temp_sensor.ino) into your board.

5. Connect temp probe from your liquid cooling loop into your board. You remembered to add it to your loop, right? :)



