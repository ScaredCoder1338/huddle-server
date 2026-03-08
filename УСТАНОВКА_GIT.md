# 📥 Установка Git для Windows

## Вариант 1: Через winget (быстро)

Открой PowerShell и выполни:

```powershell
winget install --id Git.Git -e --source winget
```

После установки **перезапусти терминал** (закрой и открой заново).

---

## Вариант 2: Скачать установщик

1. Перейди на https://git-scm.com/download/win
2. Скачай установщик (64-bit Git for Windows Setup)
3. Запусти установщик
4. Нажимай "Next" везде (настройки по умолчанию подходят)
5. После установки **перезапусти терминал**

---

## Проверка установки

После установки и перезапуска терминала выполни:

```bash
git --version
```

Должно показать что-то типа: `git version 2.43.0.windows.1`

---

## Настройка Git (первый раз)

После установки настрой свои данные:

```bash
git config --global user.name "Твоё Имя"
git config --global user.email "твой@email.com"
```

Используй тот же email, что и на GitHub!

---

## Теперь можно загружать на GitHub

После установки Git выполни команды из инструкции:

```bash
cd Huddle/HuddleServer
git init
git add .
git commit -m "Huddle Server"
git branch -M main
git remote add origin https://github.com/ScaredCoder1338/huddle-server.git
git push -u origin main
```

Каждую команду вводи отдельно и жми Enter!

---

## Если попросит логин/пароль

При `git push` может попросить авторизацию:

1. **Username:** твой GitHub username (ScaredCoder1338)
2. **Password:** НЕ пароль от аккаунта! Нужен Personal Access Token:
   - Перейди на https://github.com/settings/tokens
   - Нажми "Generate new token (classic)"
   - Выбери срок действия (например, 90 дней)
   - Поставь галочку "repo"
   - Нажми "Generate token"
   - Скопируй токен и используй вместо пароля

⚠️ **ВАЖНО:** Токен показывается только один раз! Сохрани его.

---

## Готово!

После установки Git и настройки можешь продолжить деплой на Railway!
