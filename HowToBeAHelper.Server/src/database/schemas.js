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

const userSchema = new mongoose.Schema({
    uuid: String,
    name: String,
    password: String,
    characters: [Character.schema]
});
const User = mongoose.model('User', userSchema);

module.exports = {
    Skill,
    Character,
    User
};