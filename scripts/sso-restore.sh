#!/bin/bash

cd /dumps && pg_restore -U keycloak -d keycloakdb -1 origin.dump;

docker run -v C:\\Projetos\\Threeo\\ineditta\\novo_sistema_ineditta\\database\\auth:/dumps --rm postgres:14.6-alpine sh -c 'PGPASSWORD=keycloak &&  pg_restore -h host.docker.internal -U keycloak -p 55432 -d keycloakdb /dumps/origin.dump'