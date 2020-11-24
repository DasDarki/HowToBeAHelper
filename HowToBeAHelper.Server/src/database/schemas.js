const mongoose = require('mongoose');

const skillSchema = new mongoose.Schema({
    category: Number,
    name: String,
    value: Number
});
const Skill = mongoose.model('Skill', skillSchema);

const characterSchema = new mongoose.Schema({
    uuid: String,
    name: String,
    age: Number,
    xp: Number,
    gender: String,
    health: Number,
    stature: String,
    religion: String,
    job: String,
    martialStatus: String,
    inventory: String,
    notes: String,
    pointsLeft: Number,
    createdAt: String,
    actSkills: [Skill.schema],
    knowledgeSkills: [Skill.schema],
    socialSkills: [Skill.schema]
});
const Character = mongoose.model('Character', characterSchema);

const sessionUserSchema = new mongoose.Schema({
    user: String,
    userName: String,
    session: String,
    sessionName: String,
    isPlayer: Boolean,
    character: Character.schema
});
const SessionUser = mongoose.model('SessionUser', sessionUserSchema);

const sessionSchema = new mongoose.Schema({
    hostName: String,
    uuid: String,
    name: String,
    password: String,
    players: [{type: mongoose.Schema.Types.ObjectId, ref: 'SessionUser'}]
});
const Session = mongoose.model('Session', sessionSchema);

const userSchema = new mongoose.Schema({
    uuid: String,
    name: String,
    email: String,
    password: String,
    isVerified: Boolean,
    resetId: String,
    characters: [Character.schema],
    sessions: [{type: mongoose.Schema.Types.ObjectId, ref: 'SessionUser'}]
});
const User = mongoose.model('User', userSchema);

module.exports = {
    Skill,
    Character,
    User,
    SessionUser,
    Session
};