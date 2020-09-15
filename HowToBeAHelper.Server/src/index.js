require('dotenv').config();
const isDev = process.env.IS_DEV === "true";

const app = require('express');
const http = require('http').createServer(app);
const io = require('socket.io')(http);

const database = require('./database/index.js');

database.start(() => {
    io.on('connection', (socket) => {
        socket.on("user:register", function (name, password) {
            database.createUser(name, password, status => {
                socket.emit("user:register:result", status);
            });
        });
        socket.on("character:save", function (username, character, fn) {
            database.saveCharacter(username, character, fn);
        });
        socket.on("user:login", function (name, password, fn) {
            database.loginUser(name, password, fn);
        });
        console.log("User Connected!");
    });

    const port = process.env.PORT || 1339;
    http.listen(port, function () {
        console.log("Master Server running on *:" + port);
    });
});