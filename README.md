# Creatio
## Настройки перед установкой
> .env файл содержит в себе настройки и пароли. 
*Если поменять пароль, то надо поменять это в некоторых фалйах:*
- ConnectionStrings.config
- create_db.sql
## Установка
1. Запустить скрипт build-create-db(1).cmd
2. Запустить скрипт build-create-db(2).cmd
3. Запустить скрипт install.web(3).cmd
4. Зайти на localhost:*WEB_PORT порт из .env*
5. Логин/пароль Supervisor/Supervisor
6. Если попросит скомпилировать, нужно выбрать "Нет"
7. Действие -> Выгрузить все пакеты в файловую систему
8. Скомпилировать
9. Положить файлы Packages из ветки проекта в \\wsl.localhost\docker-desktop-data\data\docker\volumes\creatiotesttask_task-web_config\_data\_Pkg
10. В браузере -> Обновить пакеты из файловой системы
11. Последовательно:
    - Обновить структуру БД
    - Установить SQL сценарии
    - Установить данные
    - Сгенирировать исходный код для трубующих генерации
    - Компилировать всё
## Удаление
> Запустить uninstall.cmd (Удалит даже данные) 

# ContactsSyncService
## Настройки перед установкой
> .env файл содержит в себе настройки и пароли. 
## Установка
1. Запустить скрипт step-1.cmd
2. Запустить скрипт step-2.cmd
3. Логи в \\wsl.localhost\docker-desktop-data\data\docker\volumes\contactssyncservice_contacts-sync-service_logs\_data\_Pkg
## Удаление
> Запустить uninstall.cmd (Удалит даже данные) 