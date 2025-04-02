FROM node:18

# Diretório de trabalho no container
WORKDIR /app

# Copia apenas os arquivos de dependência e instala
COPY package*.json ./
RUN npm install

# Copia todo o restante do código
COPY . .

# Expõe a porta usada no Express
EXPOSE 3001

# Comando de execução
CMD ["npm", "run", "dev"]
