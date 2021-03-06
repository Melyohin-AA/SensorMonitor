<div id="top"></div>


<br />
<div align="center">
  <p align="right">2021.07 - 2022.03</p>
  <h2 align="center">Sensor Monitor</h2>
  <p align="center">Визуализация данных сети наблюдения К3 МФ МГТУ им. Баумана</p>
</div>


### О проекте

![product-screenshot](https://github.com/Melyohin-AA/SensorMonitor/raw/master/_ReadmeFiles/MainImage.png)

Данный проект представляет собой приложение, предназначенное для визуального представления данных, собранных приборами сети наблюдения К3 МФ МГТУ им. Баумана. Продукт разработан в рамках ознакомительной практики.

Особенности продукта:
* Импорт данных из БД измерительной сети в форматах JSON и CSV
* Визуализация данных в виде графиков (ось X - время, ось Y - значение)
* Выбор данных для графиков
* Построение графиков за разные временные диапазоны
* Построение графиков на основе:
  * сырых данных;
  * данных, осреднённых за различные периоды времени;
  * минимальных и максимальных суточных значений;
* Различные типы графиков:
  * точки (непосредственно датированные измерения);
  * ломаная;
  * кривая;
  * ступенчатый график;
* Одновременное отображение на одной плоскости до нескольких графиков
* Возможность расширения поддерживаемых моделей приборов
* Целевая ОС - Windows 10

Разработка велась на языке программирования `C#` с использованием `.NET Framework 4.7.2`. Среда разработки - `Microsoft Visual Studio 2017`. Тип приложения - `WPF`. Для тестирования использовался `MSTest`. Библиотека, использованная для построения и визуализации, - `LiveCharts`.


### Использование

1. Выполнить сборку и настройку проекта (*ИЛИ* скачать [настроенную сборку проекта](https://drive.google.com/file/d/1EXU2SVFG2bm_i0PqjO7bTzlEQJbZUiyC/view?usp=sharing))
    1. С помощью Visual Studio выполнить сборку проекта `SensorMonitor`
    2. Выгрузить данные для визуализации из БД ([страница выгрузки](http://dbrobo.mgul.ac.ru/mainexport.html); для JSON-файла требуется изменить расширение в имени файла на .json)
    3. Предварительная настройка
        1. Запустить `SensorMonitor.exe`
        2. Добавить требуемые форматы записей приборов (`Приборы` > `Форматы записей приборов`)
        3. Добавить требуемые файлы приборов (`Приборы` > `Список приборов`)
2. Импортировать выгруженные данные (`Приборы` > `Импортировать выгруженные данные`)
3. Настроить отображение
    1. Выбрать временной период для визуализации и осреднение
    2. Применить настройки (`Показать данные за период`)
    3. Выбрать вид графиков
4. Выбрать данные для визуализации (`Добавить график`)
5. Включить отображение данных (ПКМ по элементу в списке графиков > `Показать/скрыть график`)
