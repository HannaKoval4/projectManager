# Инструкция по установке SQL Server LocalDB

## Способ 1: Установка через Visual Studio Installer (самый простой)

1. Откройте **Visual Studio Installer**
2. Нажмите **Modify** (Изменить) для вашей версии Visual Studio
3. Перейдите на вкладку **Individual components** (Отдельные компоненты)
4. Найдите и отметьте:
   - **SQL Server Express LocalDB**
   - **SQL Server Data Tools**
5. Нажмите **Modify** для установки
6. После установки перезагрузите компьютер

## Способ 2: Отдельная установка SQL Server Express

1. Скачайте **SQL Server Express** с официального сайта:
   https://www.microsoft.com/ru-ru/sql-server/sql-server-downloads
2. Выберите **Express** версию
3. При установке выберите:
   - **Basic** (базовая установка)
   - Или **Custom** и выберите **LocalDB**
4. После установки перезагрузите компьютер

## Способ 3: Установка через Chocolatey (если установлен)

```powershell
choco install sql-server-express-localdb
```

## Проверка установки

После установки выполните в PowerShell:

```powershell
sqllocaldb info
```

Или в SSMS попробуйте подключиться к серверу:
- **Server name:** `(localdb)\MSSQLLocalDB`
- **Authentication:** Windows Authentication

## Альтернатива: Использование существующего SQL Server

Если у вас уже установлен SQL Server (но не запущен):

1. Откройте **Services** (Службы): `Win+R` → `services.msc`
2. Найдите службы:
   - **SQL Server (MSSQLSERVER)** или
   - **SQL Server (SQLEXPRESS)**
3. Запустите службу (правый клик → Start)
4. В SSMS подключитесь:
   - Для **MSSQLSERVER**: `localhost` или `.`
   - Для **SQLEXPRESS**: `localhost\SQLEXPRESS`

## После установки

1. Откройте SSMS
2. Подключитесь к `(localdb)\MSSQLLocalDB`
3. Выполните скрипт `Database/CreateDatabase.sql`






