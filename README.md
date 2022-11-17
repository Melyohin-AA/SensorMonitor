<br/>
<div align="center">
  <h2 align="center">Sensor Monitor</h2>
  <p align="center">Visualization of the university's monitoring network data</p>
</div>


### About the project

![product-screenshot](https://github.com/Melyohin-AA/SensorMonitor/raw/master/_ReadmeFiles/MainImage.png)

The product is the desktop application. It can be used to plot different charts based on data downloaded from the university's database.

##### Features
* Importing data in JSON и CSV formats
* Visualization of data in the form of charts (X-axis – time, Y-axis – values)
* Selecting data for charts
* Plotting charts for different time ranges
* Charts may be based on:
  * raw data;
  * data averaged over different time periods;
  * minimum and maximum daily values;
* Different chart types:
  * dots (explicitly dated measurements);
  * polyline;
  * curve line;
  * stepped line;
* Display of multiple charts on a plane at once
* Managing list of supported device models
* Target OS – Windows 10

##### Technology stack
* `C#`
* `.NET Framework 4.7.2`
* `LiveCharts`
* `WPF`
* `MSTest`


### Usage

1. Perform compilation and configuration (*OR* download [configured build](https://drive.google.com/file/d/1EXU2SVFG2bm_i0PqjO7bTzlEQJbZUiyC/view?usp=sharing))
    1. Compile build of the `SensorMonitor` project via Visual Studio compiler
    2. Download data for visualization from the database ([download page](http://dbrobo.mgul.ac.ru/mainexport.html); for JSON-files it is necessary to change extention to .json)
    3. Configure the project
        1. Run `SensorMonitor.exe`
        2. Add record formats you want to read (`Приборы` > `Форматы записей приборов`)
        3. Add device files you want to read for (`Приборы` > `Список приборов`)
2. Import downloaded data (`Приборы` > `Импортировать выгруженные данные`)
3. Set chart display and plotting parameters
    1. Select time period for plotting and averaging
    2. Apply settings (`Показать данные за период`)
    3. Select chart type
4. Chose data for plotting (`Добавить график`)
5. Turn on chart display (RMB click on item in chart list > `Показать/скрыть график`)


### Demo

Youtube video:<br/>
[![Demo](https://img.youtube.com/vi/REFaSrarCGA/0.jpg)](https://youtu.be/REFaSrarCGA)
