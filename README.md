<div id="top"></div>


<br />
<div align="center">
  <h2 align="center">Sensor Monitor</h2>
  <p align="center">Визуализация данных измерительной сети К3 МФ МГТУ им. Баумана</p>
</div>


### О проекте

![product-screenshot](https://lh3.googleusercontent.com/zNAjXxiTvyCPo_rmWi5kXfvvbNT6LdILJtxVOZOFRlTuR-2aub_aWPkO1JKnxuV3ZzamXmtTWCD68E5oXo9Yy2QfM1eFTj4lTyqTOc_ZyUYZGnRTpkr7Az-Z7zRkxbo2JFNGEpGo4YY1FRc0CDzdPd2cU4CuciUiE3-AThMGRBpBoBmMNU6jUh3QTbcNNeFzefuxStZtsHuMih0sjODSlXD7LMzaqIDd4T6T3N_7BbO0mPP17Tz_KmSUSiyNMUsarv1XIKFrEHUvrFrGW3TAQIFJbUv044jJKDSnvYAOqEX7dI4T47rV1k1INGtOti3XpW1W2w1rPECG-t3xLq0RLPJkLqxIhWg71uVaYBvJTbOxMQzFlmXZLZSxaDyXE5bkt7nL_UT-FDfD4Xr9_0xzczpLDxBwIOTw8AMOU5YSgr98tJdGyvsOOYWZwe8MfgazkZvv9R1SCm4TUwuX7yp1eek0Hdgf75p5nlc6BfoFXwhugvikjWd40YlXGEVA3w5GczMgfn8FfTvBfjnbZHKGXatj0UF-pN8HZZoYbKYuC1-TbyUOk0xxVq_txeNvlQ4uJDnMK6-BUBeXGq4bzpULCE7FxLpUGStLDy62DZ2aJXHOL5FD-Xsah2Ht48wBJgx6cmYeoKg6gDnWi92rOT_aJrBRiHqwZ69vYx0VW9K8_R_bEcCjxbR1Hkyw0jvDz6b36xsc_WcC32oqxS455vrVxT97C00H3tEBC5c7xEj1EEY53lS1DGqziZLPMc8=w1832-h969-no?authuser=0)

Данный проект представляет собой приложение, предназначеное для визуального представления данных, собранных приборами измерительной сети К3 МФ МГТУ им. Баумана. Продукт разработан в рамках ознакомительной практики.

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


### Использование

0. Выполнить сборку проекта `SensorMonitor`
1. Выгрузить данные для визуализации из БД ([cтраница выгрузки](http://dbrobo.mgul.ac.ru/mainexport.html); для JSON-файла требуется изменить расширение в имени файла на .json)
2. Предварительная настройка
2.1. Запустить `SensorMonitor.exe`
2.2. Добавить требуемые форматы записей приборов (`Приборы` > `Форматы записей приборов` ИЛИ скопировать стандартные форматы из папки `DefaultSM` в папку при .exe файле `SensorLogs`)
2.3. Добавить требуемые файлы приборов (`Приборы` > `Список приборов`)
*ИЛИ*
2.1. Скопировать стандартные форматы записей приборов и файлы приборов (из папки `DefaultSM` в папку `SensorLogs`при .exe файле)
2.2. Запустить `SensorMonitor.exe`
3. Ипортировать выгруженные данные (`Приборы` > `Импортировать выгруженные данные`)
4. Настроить отображение
4.1. Выбрать временной период для визуализации и осреднение
4.2. Применить настройки (`Показать данные за период`)
4.3. Выбрать вид графиков
5. Выбрать данные для визуализации (`Добавить график`)
6. Включить отображение данных (ПКМ по элементу в списке графиков > `Показать/скрыть график`)
