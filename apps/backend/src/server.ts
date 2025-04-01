import express from 'express';
import dotenv from 'dotenv';

dotenv.config();

const app = express();
const port = process.env.PORT || 3001;

app.use(express.json());

app.get('/', (req, res) => {
  res.send('API rodando');
});

app.listen(port, () => {
  console.log(`API rodando na porta ${port}`);
});
