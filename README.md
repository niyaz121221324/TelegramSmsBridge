# Telegram SMS Bridge Bot

**Telegram SMS Bridge Bot** — это приложение, которое работает в контейнере Docker и перенаправляет сообщения из Telegram чата в виде SMS с помощью приложения **SmsMonitor**. Оно предоставляет удобный API для управления перенаправлением сообщений, реализованный на **ASP.NET Core**, и использует **NGINX** и **Ngrok** для публикации API в интернете.

---

## Основные возможности

- **Отправка SMS на основе сообщений из Telegram**  
  Получение сообщений из Telegram чата и автоматическая передача их в **SmsMonitor** для отправки в виде SMS.

- **Лёгкая развёртка**  
  Проект упрощён для быстрого запуска с использованием **Docker Compose**.

- **Реверс-прокси через NGINX**  
  Надёжное и эффективное перенаправление трафика к API через NGINX.

- **Публикация через Ngrok**  
  Предоставляет доступ к локальному API через публичный URL.

---

## Системные требования

Перед началом работы убедитесь, что у вас установлено следующее:

1. **Docker**  
   Установите Docker на вашем устройстве.  
   [Инструкция по установке Docker](https://docs.docker.com/get-docker/)

2. **Docker Compose**  
   Убедитесь, что **Docker Compose** установлен.  
   [Инструкция по установке Docker Compose](https://docs.docker.com/compose/install/)

3. **Ngrok токен**  
   Получите токен аутентификации на [официальном сайте Ngrok](https://ngrok.com/).  
   Вставьте его в файл **ngrok.yml** перед запуском.

Также установите приложение SmsMonitor для работы с отправкой СМС сообщений (https://github.com/niyaz121221324/SmsMonitor/)

---

## Установка и запуск

Следуйте этим шагам для установки и запуска приложения:

1. **Клонируйте репозиторий проекта**  
   Выполните следующие команды в терминале:  
   ```bash
   git clone https://github.com/niyaz121221324/TelegramSmsBridge.git
   cd TelegramSmsBridge
2. **Запустите проект**
   ```bash
   docker compose up --build
