# Use a Node.js base image
FROM node:latest

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the application code
COPY . .

RUN npm run build

# Expose the port Vite is listening on
EXPOSE 8081

# Command to run the Vite development server
CMD ["npm", "run", "preview"]