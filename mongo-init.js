// Переключаемся на базу данных сообщений
db = db.getSiblingDB("messages");

// Создаем root пользователя, если он еще не создан
if (db.system.users.find({ user: "admin" }).count() === 0) {
    db.createUser({
        user: "admin",
        pwd: "admin",
        roles: [
            { role: "root", db: "admin" }
        ]
    });
    print("Root user 'admin' created");
} else {
    print("Root user 'admin' already exists");
}

// Создаем коллекцию sms_messages_collection
if (!db.getCollectionNames().includes("sms_messages_collection")) {
    db.createCollection("sms_messages_collection");
    db.sms_messages_collection.createIndex({ "chat_id": 1 }, { unique: true });
    print("Created collection 'sms_messages_collection'");
} else {
    print("Collection 'sms_messages_collection' already exists");
}

// Создаем коллекцию updates_collection
if (!db.getCollectionNames().includes("updates_collection")) {
    db.createCollection("updates_collection");
    db.updates_collection.createIndex({ "message.chat.id": 1 }, { unique: true });
    db.updates_collection.createIndex({ "message.chat.username": "text" });
    print("Created collection 'updates_collection'");
} else {
    print("Collection 'updates_collection' already exists");
}