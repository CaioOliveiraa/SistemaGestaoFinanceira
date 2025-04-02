import { Dialect } from 'sequelize';
import { Sequelize } from 'sequelize-typescript';
import dotenv from 'dotenv';
import path from 'path';
import { User } from '../models/user.model';

dotenv.config();

const sequelize = new Sequelize({
    dialect: 'postgres' as Dialect,
    host: process.env.DB_HOST,
    port: Number(process.env.DB_PORT),
    username: process.env.DB_USER,
    password: process.env.DB_PASS,
    database: process.env.DB_NAME,
    models: [User],
    logging: false,
});

export default sequelize;
