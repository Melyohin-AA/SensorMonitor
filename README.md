<div id="top"></div>


<br />
<div align="center">
  <h2 align="center">Sensor Monitor</h2>
  <p align="center">Визуализация данных измерительной сети К3 МФ МГТУ им. Баумана</p>
</div>


<details>
  <summary>Содержание</summary>
  <ol>
    <li>
      <a href="#о-проекте">О проекте</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
  </ol>
</details>

1. [О проекте](#о-проекте)


## О проекте

[![Main Screenshot][product-screenshot]](https://drive.google.com/file/d/1GW9tw3S38J8hnETgJp3o5rTpM5npRciO/view?usp=sharing)

Данный проект представляет собой приложение, предназначеное для визуального представления данных, собранных приборами измерительной сети К3 МФ МГТУ им. Баумана. Продукт разработан в рамках ознакомительной практики.

Данные измерительной сети представлены множеством датированных измерений.
[Страница выгрузки данных из БД](http://dbrobo.mgul.ac.ru/mainexport.html)

Особенности продукта:
* импорт данных из БД измерительной сети в форматах JSON и CSV;
* визуализация данных в виде графиков (ось X - время, ось Y - значение);
* выбор данных для графиков;
* построение графиков за разные временные диапазоны;
* построение графиков на основе:
  * сырых данных;
  * данных, осреднённых за различные периоды времени;
  * минимальных и максимальных суточных значений;
* различные типы графиков:
  * точки (непосредственно датированные измерения);
  * ломаная;
  * кривая;
  * ступенчатый график;
* одновременное отображение на одной плоскости до нескольких графиков;
* возможность расширения поддерживаемых моделей приборов;
* целевая ОС - Windows 10.

Разработка велась на языке программирования `C#` с использованием `.NET Framework 4.7.2`. Среда разработки - `Microsoft Visual Studio 2017`. Тип приложения - `WPF`. Для тестирования использовался `MSTest`. Библиотека, использованная для построения и визуализации, - `LiveCharts`.


## Инструкция по настройке проекта

1. Склонировать проект
   ```sh
   git clone https://github.com/Melyohin-AA/SensorMonitor.git
   ```
2. Открыть проект в Visual Studio
3. Запустить сборку `SensorMonitor`
