
FROM node:20-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps


COPY . .


RUN npm run build --prod


FROM nginx:1.25-alpine

RUN rm -rf /usr/share/nginx/html/*


COPY --from=builder /app/dist/ELearn/browser /usr/share/nginx/html



COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
