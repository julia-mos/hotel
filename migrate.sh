#!/bin/bash

while getopts n: flag
do
    case "${flag}" in
        n) name=${OPTARG};;
    esac
done

if [ ${#name} -lt 5 ]; then echo "Podaj dluzsza nazwe migracji" ; exit
fi

( 
cd Migrations; 
dotnet ef migrations add ${name} --startup-project ../AuthService/ || echo "Blad przy tworzeniu migracji";  
dotnet ef database update --connection ${DB_CONNECTION} --startup-project ../AuthService/ || echo "Blad przy aktualizacji bazy"
) && echo "Migracja zakonczona";


exit;