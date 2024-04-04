#!/bin/bash

if [ -n "$URL" ]; then
    find /var/www/html -type f -exec sed -i "s|http://localhost:8000/|$URL|g" {} +
else
    echo "URL is unset or empty"
fi

if [ -n "$FRONTEND_CONFIG" ]; then
    for file in /var/www/html/assets/configs/config.*.json; do
        echo "$FRONTEND_CONFIG" > "$file"
    done
else
    echo "FRONTEND_CONFIG is unset or empty"
fi

if [ -n "$CONFIG_JSON" ]; then
    echo "$CONFIG_JSON" > /var/www/html/includes/config/config.json
else
    echo "CONFIG_JSON is unset or empty"
fi

if [ -n "$KEYCLOAK_CONFIG" ]; then
    echo "$KEYCLOAK_CONFIG" > /var/www/html/assets/configs/keycloak.json
    echo "$KEYCLOAK_CONFIG" > /var/www/html/keycloak.json
else
    echo "KEYCLOAK_CONFIG is unset or empty"
fi

mkdir -p /srv/ineditta/documentos/documentos_sindicais/
chmod -R 777 /srv/ineditta/documentos/documentos_sindicais/

# Run Apache in the foreground
exec apache2-foreground

exit 0
