# Telegram SMS Bridge Bot

**Telegram SMS Bridge Bot** — это Docker-приложение, которое перенаправляет SMS-сообщения в Telegram-бот. Оно предоставляет простой API для управления перенаправлением SMS, реализованный на ASP.NET Core, и использует NGINX и Ngrok для публикации API в интернете.

---

## Возможности

- **Перенаправление SMS в Telegram**: Получает SMS-сообщения и отправляет их в указанный чат Telegram.
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
