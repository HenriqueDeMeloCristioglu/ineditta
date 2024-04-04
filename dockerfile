FROM php:8.0-apache as base

WORKDIR /var/www/html

RUN apt-get clean
RUN rm -rf /var/lib/apt/lists/*

# Install additional PHP extensions if needed
RUN apt-get update

RUN apt-get update && apt-get install -y \
    curl \
    unzip 

RUN docker-php-ext-install mysqli

RUN pecl install xdebug
RUN docker-php-ext-enable xdebug

# Enable Apache modules if needed
RUN a2enmod rewrite

COPY ./conf /usr/local/etc/php/
COPY ./conf/xdebug /usr/local/etc/php/conf.d/
COPY ./startup.sh /startup.sh

RUN chmod -R 777 /var/log && chmod +x /startup.sh

CMD ["/startup.sh"]

FROM node:16 as build
WORKDIR /app
COPY ./sistema/package*.json .

# Install the frontend dependencies
RUN npm install

COPY ./sistema/webpack.config.js .
# COPY ./sistema/keycloak.json .

# Copy the frontend source code to the container
COPY ./sistema/src ./src


# Build the frontend assets using Webpack
RUN npm run build


FROM base AS final
WORKDIR /var/www/html
COPY --from=build /app/dist .