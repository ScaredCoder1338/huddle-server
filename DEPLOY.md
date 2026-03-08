# 🚀 Деплой Huddle Server (Бесплатно)

## 📋 Подготовка завершена

Сервер готов к деплою! Добавлены:
- ✅ Dockerfile для контейнеризации
- ✅ .dockerignore для оптимизации
- ✅ Поддержка переменной окружения PORT
- ✅ CORS настроен для работы из любого источника

---

## 🎯 Вариант 1: Railway.app (РЕКОМЕНДУЕТСЯ)

### Преимущества:
- ✅ Бесплатно 500 часов в месяц
- ✅ Автоматический деплой из GitHub
- ✅ HTTPS из коробки
- ✅ Простая настройка

### Шаги:

1. **Создай GitHub репозиторий**
   ```bash
   cd Huddle/HuddleServer
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/твой-username/huddle-server.git
   git push -u origin main
   ```

2. **Зарегистрируйся на Railway**
   - Перейди на https://railway.app
   - Войди через GitHub
   - Нажми "New Project"
   - Выбери "Deploy from GitHub repo"
   - Выбери свой репозиторий huddle-server

3. **Railway автоматически:**
   - Обнаружит Dockerfile
   - Соберет и запустит сервер
   - Даст тебе URL типа: `https://huddle-server-production.up.railway.app`

4. **Получи URL сервера**
   - В Railway перейди в Settings → Domains
   - Скопируй URL (например: `huddle-server-production.up.railway.app`)

5. **Обнови клиент**
   - Открой `Huddle/Services/ChatService.cs`
   - Измени URL с `http://localhost:5000/chathub` на `https://твой-url.railway.app/chathub`

---

## 🎯 Вариант 2: Render.com

### Преимущества:
- ✅ Полностью бесплатно (без ограничений по времени)
- ✅ Автоматический деплой
- ✅ HTTPS из коробки
- ⚠️ Засыпает после 15 минут неактивности (просыпается за ~30 сек)

### Шаги:

1. **Создай GitHub репозиторий** (как в варианте 1)

2. **Зарегистрируйся на Render**
   - Перейди на https://render.com
   - Войди через GitHub
   - Нажми "New +" → "Web Service"
   - Подключи свой репозиторий

3. **Настрой сервис:**
   - Name: `huddle-server`
   - Environment: `Docker`
   - Instance Type: `Free`
   - Нажми "Create Web Service"

4. **Получи URL:**
   - После деплоя получишь URL: `https://huddle-server.onrender.com`

5. **Обнови клиент** (как в варианте 1)

---

## 🎯 Вариант 3: Azure (Microsoft)

### Преимущества:
- ✅ $200 кредитов на 30 дней
- ✅ Профессиональная платформа
- ✅ Отличная интеграция с .NET

### Шаги:

1. **Зарегистрируйся на Azure**
   - https://azure.microsoft.com/free
   - Нужна банковская карта (не снимут деньги)

2. **Установи Azure CLI**
   ```bash
   # Windows
   winget install Microsoft.AzureCLI
   ```

3. **Деплой:**
   ```bash
   cd Huddle/HuddleServer
   az login
   az webapp up --name huddle-server --runtime "DOTNET:10.0" --sku F1
   ```

4. **Получи URL:**
   - `https://huddle-server.azurewebsites.net`

---

## 📝 После деплоя

### Обнови клиент:

Открой `Huddle/Services/ChatService.cs` и измени:

```csharp
private readonly string _serverUrl = "https://твой-сервер.railway.app/chathub";
```

Или сделай настраиваемым через конфиг:

```csharp
private readonly string _serverUrl = 
    Environment.GetEnvironmentVariable("HUDDLE_SERVER_URL") 
    ?? "http://localhost:5000/chathub";
```

---

## 🔍 Проверка работы

После деплоя проверь:

1. **Открой в браузере:**
   ```
   https://твой-сервер.railway.app/chathub
   ```
   Должна быть ошибка 404 или сообщение о SignalR - это нормально!

2. **Запусти клиент** и попробуй отправить сообщение

3. **Проверь логи** в Railway/Render/Azure

---

## 💡 Рекомендация

Для начала используй **Railway.app**:
- Проще всего настроить
- Бесплатно 500 часов/месяц (достаточно для тестов)
- Автоматический деплой при push в GitHub
- Не засыпает как Render

---

## 🆘 Если что-то не работает

1. Проверь логи в Railway/Render
2. Убедись что CORS настроен правильно
3. Проверь что клиент использует HTTPS (не HTTP)
4. Убедись что порт правильно настроен

Нужна помощь с деплоем? Скажи на какую платформу хочешь задеплоить!
