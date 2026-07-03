# Hardware Monitoring Configuration Guide

This guide walks you through configuring **FanControl** and **MSI Afterburner / RivaTuner Statistics Server (RTSS)** to read the dual-temperature sensor data from your Arduino.

---

## FanControl Instructions

The custom plugin works as the "Master" reader. It reads the raw JSON serial stream from the Arduino, maps the temperatures to FanControl cards, and forwards them to Windows Performance Counters.

### 1. Set the Environment Variable
The plugin needs to know which COM port your Arduino is connected to.
1. Open the Windows Start Menu, search for **"Edit the system environment variables"**, and open it.
2. Click the **Environment Variables...** button.
3. Under **User variables** (or System variables), click **New...**.
4. Set the **Variable name** to `SENSOR_COM_PORT`.
5. Set the **Variable value** to your Arduino's COM port (e.g., `COM3`).
6. Click **OK** to save.

### 2. Deploy the Plugin DLL
1. Locate the compiled `FanControl.CustomLoop.dll` in this repository under the `FanControl.CustomLoop/` directory.
2. Copy `FanControl.CustomLoop.dll` and paste it into your FanControl installation's **Plugins** folder (usually located at `C:\path\to\FanControl\Plugins\`).

### 3. Run FanControl
1. Start **FanControl.exe**.
2. Go to your sensor cards. You should see two new temperature sensors named **Ambient** and **Coolant** displaying the correct real-time temperatures.

*Note: If you already had FanControl open, restart it so the plugin loads and detects the environment variable.*

---

## MSI Afterburner & RTSS Instructions

MSI Afterburner reads the temperature data from the custom Windows Performance Counters created by the plugin, divides them by 100 to restore decimals, and displays them in the RTSS overlay.

### 1. Initialize Performance Counters (First Time Only)
You must register the custom performance counters in Windows so they are readable by system utilities.
1. Open **PowerShell** as **Administrator** (Right-click -> Run as Administrator).
2. If you have any old versions of the counters registered, delete them first:
   ```powershell
   [System.Diagnostics.PerformanceCounterCategory]::Delete("ArduinoSensors")
   ```
3. Run the counter creation script from the repository's `Sources` directory:
   ```powershell
   powershell.exe -ExecutionPolicy Bypass -File "C:\path\to\repository\Sources\CreateCounters.ps1"
   ```
4. Confirm registration by running:
   ```powershell
   [System.Diagnostics.PerformanceCounterCategory]::New('ArduinoSensors').GetCounters() | Select-Object CounterName, CounterHelp, CounterType
   ```
   *Verify that you see `Temp1` and `Temp2` listed.*

### 2. Configure PerfCounter.dll in MSI Afterburner
1. Open **MSI Afterburner** and click the **Settings** (gear) icon.
2. Navigate to the **Monitoring** tab.
3. Click the **"..."** button next to **"Active hardware monitoring graphs"** to open the active plugins dialog.
4. Click on the **PerfCounter.dll** row. You should see a check mark icon appear to the left of the `PerfCounter.dll` text (enabling the plugin), then click the **Setup** button at the bottom.
5. In the **Setup PerfCounter.dll plugin** window, click the **Add** button at the bottom.
   * In the **Add performance counter** pop-up window, locate and expand **`ArduinoSensors`**.
   * Select **`Temp1`** and click **OK**.
   * Click the **Add** button again, locate **`ArduinoSensors`**, select **`Temp2`**, and click **OK**.
   * *This imports both raw counter templates into your active data sources list.*
6. Select the **`Temp1`** row (which may initially be named `SingleInstance` under the "Name" column) in your Active data sources list and configure its properties at the bottom:
   * **Object name:** `ArduinoSensors` (prepopulated)
   * **Instance name or index:** *(Leave completely blank)*
   * **Counter name:** `Temp1` (prepopulated)
   * **Name:** Change `SingleInstance` to **`Ambient`**
   * **Units:** Type **`°C`** (or just `C`)
   * **Format:** Leave **blank** to display as a rounded integer (recommended for a cleaner, compact overlay), or type **`%.2f`** if you prefer displaying two decimal places (e.g. `22.66`).
   * **Divider:** Change `1` to **`100`** *(This scales the raw integer value back to decimal form)*
7. Select the **`Temp2`** row in the Active data sources list and configure it as follows:
   * **Object name:** `ArduinoSensors` (prepopulated)
   * **Instance name or index:** *(Leave completely blank)*
   * **Counter name:** `Temp2` (prepopulated)
   * **Name:** Change `SingleInstance` to **`Coolant`**
   * **Units:** Type **`°C`** (or just `C`)
   * **Format:** Leave **blank** to display as a rounded integer (recommended), or type **`%.2f`** for two decimal places.
   * **Divider:** Change `1` to **`100`**
8. Click **OK** to save and exit the plugin configuration windows.
9. Back in the main **Monitoring** settings tab, scroll through the list of active hardware monitoring graphs.
10. **Uncheck** any unnecessary graphs that may have been imported by the Performance Counter plugin by default (e.g. `HDD1 usage`, `HDD1 read rate`, etc.) to keep your monitoring list clean.
11. Locate **`Ambient`** and **`Coolant`** in the list and ensure the checkbox next to both names is **checked** to enable active monitoring.
12. Click the **Apply** button at the bottom of the MSI Afterburner properties window to save your active graph selections.

### 3. Enable Graphs and Group OSD Elements
1. In the main **Monitoring** settings tab, select **Ambient** in the list:
   * Check the box **"Show in On-Screen Display"**.
   * Under **OSD Group Name**, type **`Loop Temps`** (or your custom overlay pill group name).
2. Select **Coolant** in the list:
   * Check the box **"Show in On-Screen Display"**.
   * Set the **OSD Group Name** to the **exact same name** (`Loop Temps`).
3. Click **Apply** and **OK** to finally save all overlay and monitoring settings.

Your in-game overlay (RTSS) will now render the grouped temperatures together as a single floating pill overlay!

---

## Troubleshooting

* **Values show as 0 or N/A in Afterburner:** If you created or updated the performance counters while FanControl was running, the C# plugin may have failed to connect to them. **Restart FanControl** to force the plugin to reconnect to the new counters.
* **COM Port is Busy:** Ensure no other application (like the Arduino IDE Serial Monitor) is reading from the designated COM port, as serial channels only allow one active reader.
