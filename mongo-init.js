db = db.getSiblingDB("messages");

// Коллекция Sms сообщений
db.createCollection("sms_messages_collection");
db.sms_messages_collection.createIndex({ "chat_id": 1 });

// Коллекция сообщений из Telegram чата
db.createCollection("updates_collection");
db.updates_collection.createIndex({ "message.chat.id": 1 });
db.updates_collection.createIndex({ "message.chat.username": "text" });