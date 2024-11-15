# Telegram SMS Bridge Bot

**Telegram SMS Bridge Bot** — это Docker-приложение, которое перенаправляет SMS-сообщения в Telegram-бот. Оно предоставляет простой API для управления перенаправлением SMS, реализованный на ASP.NET Core, и использует NGINX и Ngrok для публикации API в интернете.

---

## Возможности

- **Перенаправление SMS в Telegram**: Получает Сообщения из Telegram чата и отправляет sms пользователя.
- **Docker-упаковка**: Легко разворачивается с помощью Docker Compose.
- **Реверс-прокси NGINX**: Обеспечивает безопасное и эффективное перенаправление трафика к API.
- **Туннелирование с Ngrok**: Публикует локальный сервис в интернете.

---

## Требования

1. **Docker**: Установите Docker на вашем компьютере.
2. **Docker Compose**: Установите Docker Compose.
3. **Токен Ngrok**: Получите бесплатный токен аутентификации Ngrok на [сайте Ngrok](https://ngrok.com/).

---

## Настройка

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/yourusername/TelegramSmsBridge.git
   cd TelegramSmsBridge

   # Настройка и запуск Telegram SMS Bridge Bot
   cd TelegramSmsBridge
   docker compose up -d

## Настройка Ngrok

1. Замените `NGROK_AUTHTOKEN` в файле `docker-compose.yml` на ваш токен аутентификации Ngrok:
   ```yaml
   environment:
     NGROK_AUTHTOKEN: ваш_токен_аутентификации
