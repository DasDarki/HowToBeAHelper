const mongoose = require('mongoose');
const schemas = require('./schemas.js');
const {v4: uuidv4} = require('uuid');

function Database() {

    this.start = function (callback) {
        mongoose.connect(`mongodb://${process.env.MONGO_HOST}:27017/` + process.env.MONGO_DABA, {
            useNewUrlParser: true, useUnifiedTopology: true,
            user: process.env.MONGO_USER,
            pass: process.env.MONGO_PASS
        }).then(() => {
            console.log("Connection to Mongo established!");
            callback();
        }).catch(err => {
            console.error(err);
        });
    };

    this.closeSession = function (sessionId, callback) {
        schemas.Session.findOne({uuid: sessionId}).populate("players").exec(function (err, session) {
            if (err) {
                console.error(err);
                callback(false);
                return;
            }
            for (let i = 0; i < session.players.length; i++) {
                schemas.SessionUser.deleteOne({_id: session.players[i]._id}, function (err) {
                    console.error(err);
                });
            }
            schemas.Session.deleteOne({_id: session._id}, function (err) {
                if (err) {
                    console.error(err);
                    callback(false);
                    return;
                }
                callback(true);
            })
        });
    };

    this.kickPlayer = function (sessionId, playerId, callback) {
        schemas.Session.findOne({uuid: sessionId}).populate("players").exec(function (err, session) {
            if (err) {
                console.error(err);
                callback(false, null);
                return;
            }
            let sessionUser = null;
            for (let i = 0; i < session.players.length; i++) {
                let player = session.players[i];
                if (player.user == playerId) {
                    sessionUser = player;
                    break;
                }
            }
            if (sessionUser == null) {
                callback(false, null);
                return;
            }
            let id = sessionUser._id;
            let userName = sessionUser.userName;
            schemas.SessionUser.deleteOne({_id: id}, function (err) {
                if (err) {
                    console.error(err);
                    callback(false, null);
                    return;
                }
                callback(true, userName);
            });
        });
    };

    this.updateSessionSkills = function (sessionId, charId, json, hostSync) {
        schemas.Session.findOne({uuid: sessionId}).populate("players").exec(function (err, session) {
            if (err) {
                console.error(err);
                return;
            }
            let sessionUser = null;
            for (let i = 0; i < session.players.length; i++) {
                let player = session.players[i];
                if (!player.character) continue;
                if (player.character.uuid == charId) {
                    sessionUser = player;
                    break;
                }
            }
            if (sessionUser == null) return;
            let newSkills = JSON.parse(json);
            for (let i = 0; i < newSkills.length; i++) {
                let newSkill = newSkills[i];
                let skill = sessionUser.character[newSkill.type][newSkill.idx];
                skill.name = newSkill.name;
                skill.value = newSkill.val;
            }
            sessionUser.save(function (err, _) {
                if (err) {
                    console.error(err);
                }
            });
            hostSync(session.hostName, sessionUser.userName);
        });
    };

    this.updateSessionCharacter = function (sessionId, username, charId, key, val, hostSync) {
        schemas.Session.findOne({uuid: sessionId}).populate("players").exec(function (err, session) {
            if (err) {
                console.error(err);
                return;
            }
            let sessionUser = null;
            for (let i = 0; i < session.players.length; i++) {
                let player = session.players[i];
                if (!player.character) continue;
                if (player.character.uuid == charId) {
                    sessionUser = player;
                    break;
                }
            }
            if (sessionUser == null) return;
            sessionUser.character[key] = val;
            sessionUser.save(function (err, _) {
                if (err) {
                    console.error(err);
                }
            });
            hostSync(session.hostName, sessionUser.userName);
        });
    };

    this.createUser = function (name, password, callback) {
        schemas.User.find({name: {$regex: new RegExp(name, "i")}}, function (err, result) {
            if (err) {
                console.error(err);
                callback(2); //internal err
                return;
            }
            if (result && result.length > 0) {
                callback(1); //username existing
                return;
            }
            let user = new schemas.User({name: name, password: password, characters: [], uuid: uuidv4()});
            user.save(function (err, _) {
                let status = 0; //success
                if (err) {
                    status = 2; //internal err
                    console.error(err);
                }
                callback(status);
            });
        });
    };

    this.joinSession = function (username, sessionId, sessionPassword, charId, fn, syncCallback) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                fn("");
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                if (!user.sessions)
                    user.sessions = [];
                let character = null;
                for (let i = 0; i < user.characters.length; i++) {
                    if (user.characters[i].uuid == charId) {
                        character = user.characters[i];
                        break;
                    }
                }
                if (!character) {
                    fn("");
                    return;
                }
                schemas.Session.find({
                    uuid: sessionId,
                    password: sessionPassword
                }, null, {limit: 1}, function (err, sessionResult) {
                    if (err) {
                        console.error(err);
                        fn("");
                        return;
                    }
                    let session = sessionResult[0];
                    if (!session) {
                        fn("");
                        return;
                    }
                    let sessionUserData = new schemas.SessionUser({
                        user: user.uuid,
                        userName: username,
                        session: session.uuid,
                        sessionName: session.name,
                        isPlayer: true,
                        character: character
                    });
                    sessionUserData.save(function (err, sessionUser) {
                        if (err) {
                            console.error(err);
                            fn("");
                            return;
                        }
                        session.players.push(sessionUser._id);
                        session.save(function (err, _) {
                            if (err) {
                                console.error(err);
                                fn("");
                                return;
                            }
                            user.sessions.push(sessionUser._id);
                            user.save(function (err, _) {
                                if (err) {
                                    console.error(err);
                                    fn("");
                                    return;
                                }
                                let json = JSON.stringify(sessionUser).split('"').join('\\"');
                                syncCallback(session.hostName, session.uuid, json)
                                fn(json);
                            });
                        });
                    });
                });
            } else {
                fn("");
            }
        });
    };

    this.createSession = function (username, name, password, fn) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                fn("");
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                if (!user.sessions)
                    user.sessions = [];
                let sessionData = new schemas.Session({
                    uuid: uuidv4(),
                    name: name,
                    password: password,
                    hostName: user.name
                });
                sessionData.save(function (err, session) {
                    if (err) {
                        console.error(err);
                        fn("");
                        return;
                    }
                    let sessionUserData = new schemas.SessionUser({
                        user: user.uuid,
                        userName: username,
                        session: session.uuid,
                        sessionName: name,
                        isPlayer: false
                    });
                    sessionUserData.save(function (err, sessionUser) {
                        session.players.push(sessionUser._id);
                        session.save(function (err, _) {
                            if (err) {
                                console.error(err);
                                fn("");
                                return;
                            }
                            session.populate('players').execPopulate().then(() => {
                                user.sessions.push(sessionUser._id);
                                user.save(function (err, _) {
                                    if (err) {
                                        console.error(err);
                                        fn("");
                                        return;
                                    }
                                    user.populate("sessions").execPopulate().then(() => {
                                        fn(JSON.stringify([sessionUser, session]).split('"').join('\\"'));
                                    }).catch(err => {
                                        console.error(err);
                                        fn("");
                                    });
                                });
                            }).catch(err => {
                                console.error(err);
                                fn("");
                            });
                        });
                    });
                });
            } else {
                fn("");
            }
        });
    };

    this.refreshCharacters = function (username, fn) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                fn("");
                return;
            }
            if (result && result.length >= 1) {
                fn(JSON.stringify(result[0].characters).split('"').join('\\"'));
            } else {
                fn("");
            }
        });
    };

    this.updateSkills = function (username, charId, json) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                let character = null;
                for (let i = 0; i < user.characters.length; i++) {
                    let o = user.characters[i];
                    if (o.uuid == charId) {
                        character = o;
                        break;
                    }
                }
                let newSkills = JSON.parse(json);
                for (let i = 0; i < newSkills.length; i++) {
                    let newSkill = newSkills[i];
                    let skill = character[newSkill.type][newSkill.idx];
                    skill.name = newSkill.name;
                    skill.value = newSkill.val;
                }
                user.save(function (err, _) {
                    if (err) {
                        console.error(err);
                    }
                });
            }
        });
    };

    this.updateCharacter = function (username, charId, key, val) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                let character = null;
                for (let i = 0; i < user.characters.length; i++) {
                    let o = user.characters[i];
                    if (o.uuid == charId) {
                        character = o;
                        break;
                    }
                }
                character[key] = val;
                user.save(function (err, _) {
                    if (err) {
                        console.error(err);
                    }
                });
            }
        });
    };

    this.deleteCharacter = function (username, charId, callback) {
        schemas.User.find({
            name: {$regex: new RegExp(username, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                callback(false);
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                let charIdx = null;
                for (let i = 0; i < user.characters.length; i++) {
                    if (user.characters[i].uuid == charId) {
                        charIdx = i;
                        break;
                    }
                }
                user.characters.splice(charIdx, 1);
                user.save(function (err, _) {
                    if (err) {
                        console.error(err);
                        callback(false);
                    } else {
                        callback(true);
                    }
                });
            } else {
                callback(false);
            }
        });
    };

    this.saveCharacter = function (name, character, callback) {
        schemas.User.find({
            name: {$regex: new RegExp(name, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                callback(false);
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                user.characters.push(new schemas.Character(character));
                user.save(function (err, _) {
                    if (err) {
                        console.error(err);
                        callback(false);
                    } else {
                        callback(true);
                    }
                });
            } else {
                callback(false);
            }
        });
    };

    this.requestSessionData = function (sessionId, fn) {
        schemas.Session.find({uuid: sessionId}, null, {limit: 1}, function (err, res) {
            if (err) {
                console.error(err);
                fn("");
            } else {
                let obj = res[0];
                if (obj != null) {
                    fn(JSON.stringify(obj).split('"').join('\\"'));
                } else {
                    fn("");
                }
            }
        });
    };

    this.loginUser = async function (name, password, callback) {
        let result = await schemas.User.find({
            name: {$regex: new RegExp(name, "i")},
            password: password
        }, null, {limit: 1}).populate('sessions').exec();
        if (result && result.length >= 1) {
            let sessions = result[0].sessions;
            let sessionObjs = {};
            new Promise(resolve => {
                let waiters = 0;

                for (let i = 0; i < sessions.length; i++) {
                    let session = sessions[i];
                    if (!session.isPlayer) {
                        waiters++;
                        schemas.Session.find({uuid: session.session}, null, {limit: 1}).populate('players').exec(function (err, res) {
                            if (err) {
                                console.error(err);
                            } else {
                                let obj = res[0];
                                if (obj != null) {
                                    sessionObjs[i] = obj;
                                }
                            }
                            waiters--;
                        });
                    }
                }

                let sleeper = () => {
                    return new Promise(resolve => {
                        setTimeout(resolve, 100);
                    });
                };
                let finisher = () => {
                    if (waiters <= 0) {
                        resolve(sessions);
                    } else {
                        sleeper().then(_ => {
                            finisher();
                        });
                    }
                }
                finisher();
            }).then(_ => {
                callback(JSON.stringify([result[0].characters, sessions, sessionObjs]).split('"').join('\\"'));
            });
        } else {
            callback("");
        }
    };
}

module.exports = new Database();