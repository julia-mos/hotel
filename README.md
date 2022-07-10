# Hotel

## Installation

Set up apps

```bash
docker-compose up --build -d
```

Export database connection env

```bash
export DB_CONNECTION='server=127.0.0.1;port=3308;userid=root;pwd=root;database=hotel;allowuservariables=true'
```

Migrate

```bash
./migrate.sh -n NameOfMigration
```
