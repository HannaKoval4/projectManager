# Менеджер проектов

Десктопное приложение для управления проектами и задачами на WPF с использованием MS SQL Server и Entity Framework.

## Структура проекта

- **Models/** - Модели сущностей (Project, Task, Employee)
- **ViewModels/** - ViewModels для паттерна MVVM
- **Views/** - XAML представления
- **Data/** - DbContext для работы с базой данных
- **Database/** - SQL-скрипты для создания БД

## Требования

- .NET Framework 4.7.2 или выше
- Visual Studio 2017 или выше
- MS SQL Server (Express или выше)
- Entity Framework 6.4.4

## Установка и настройка

1. **Создание базы данных:**
   - Откройте SQL Server Management Studio
   - Выполните скрипт `Database/CreateDatabase.sql`

2. **Настройка строки подключения:**
   - Откройте файл `App.config`
   - Измените строку подключения в секции `connectionStrings` при необходимости:
   ```xml
   <add name="ProjectManagerConnection" 
        connectionString="Data Source=localhost;Initial Catalog=ProjectManagerDB;Integrated Security=True;MultipleActiveResultSets=True" 
        providerName="System.Data.SqlClient" />
   ```

3. **Установка пакетов NuGet:**
   - Откройте проект в Visual Studio
   - Восстановите пакеты NuGet (правая кнопка на решении -> Restore NuGet Packages)
   - Или установите вручную: `EntityFramework` версии 6.4.4

4. **Сборка проекта:**
   - Откройте `ProjectManager.sln` в Visual Studio
   - Нажмите F5 для запуска или Build -> Build Solution

## Использование

- **Добавление проекта:** Нажмите кнопку "Добавить проект"
- **Редактирование проекта:** Выберите проект в таблице и нажмите "Редактировать"
- **Удаление проекта:** Выберите проект и нажмите "Удалить"
- **Добавление задачи:** Выберите проект и нажмите "Добавить задачу"
- **Обновление данных:** Нажмите кнопку "Обновить"

## Особенности

- Паттерн MVVM для разделения логики и представления
- Современный UI с закругленными углами и мягкими цветами
- Связи один-ко-многим между сущностями
- Валидация данных через атрибуты Data Annotations
- Использование ObservableCollection для автоматического обновления UI






