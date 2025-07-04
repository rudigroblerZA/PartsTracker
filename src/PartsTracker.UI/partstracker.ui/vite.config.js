import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [plugin()],
    port: 8081,
    server: {
        port: 8081,
    },
    preview: {
        port: 8081,
    }
})