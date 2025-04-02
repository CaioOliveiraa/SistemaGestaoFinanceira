import 'reflect-metadata';
import express from 'express';
import dotenv from 'dotenv';
import sequelize from './database/config/database';

dotenv.config();

const app = express();
const port = process.env.PORT || 3001;

app.use(express.json());

app.get('/', (req, res) => {
    res.send('API rodando');
});

(async () => {
    try {
        console.log('🔁 Tentando conectar ao banco...');
        await sequelize.sync({ alter: true });
        console.log('📦 Banco conectado com sucesso!');

        app.listen(port, () => {
            console.log(`🚀 API rodando na porta ${port}`);
        });
    } catch (error) {
        console.error('❌ Erro ao conectar ao banco de dados:', error);
    }
})();
