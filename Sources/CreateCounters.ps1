$categoryName = "ArduinoSensors"

# This script ONLY needs to be run once to tell Windows that these metrics exist.
if (-not [System.Diagnostics.PerformanceCounterCategory]::Exists($categoryName)) {
    Write-Host "Creating Windows Performance Counters for RTSS..."
    $counterData = New-Object System.Diagnostics.CounterCreationDataCollection
    $temp1 = New-Object System.Diagnostics.CounterCreationData "Temp1", "Thermistor 1", "NumberOfItems32"
    $temp2 = New-Object System.Diagnostics.CounterCreationData "Temp2", "Thermistor 2", "NumberOfItems32"
    
    $counterData.Add($temp1) | Out-Null
    $counterData.Add($temp2) | Out-Null
    
    [System.Diagnostics.PerformanceCounterCategory]::Create($categoryName, "Arduino Temp Probes", [System.Diagnostics.PerformanceCounterCategoryType]::SingleInstance, $counterData) | Out-Null
    Write-Host "Success! The performance counters have been created."
} else {
    Write-Host "Counters already exist. You are good to go!"
}

Write-Host "Press any key to close..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null
