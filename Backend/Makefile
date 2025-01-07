##########################################################################
# This is the project's Makefile.
##########################################################################

##########################################################################
# VARIABLES
##########################################################################

APP := sessions
HOME := $(shell echo ~)
PWD := $(shell pwd)
SRC := $(PWD)/src

POSTGRES_CONTAINER_NAME := $(APP)-postgres
POSTGRES_IMAGE := postgres:17.2
POSTGRES_PORT := 5432
POSTGRES_VOLUME := $(APP)-postgres-data
POSTGRES_DB := $(APP)-local

# Load env file
include env.make
export $(shell sed 's/=.*//' env.make)

##########################################################################
# MENU
##########################################################################

.PHONY: help
help: ## Show this help menu
	@awk 'BEGIN {FS = ":.*?## "} /^[0-9a-zA-Z_-]+:.*?## / {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

##########################################################################
# SETUP
##########################################################################

# Ensure the env.make file exists
.PHONY: check-env
check-env: ## Check that the env.make file exists
	@if [ ! -f env.make ]; then \
		echo "Error: env.make file not found! Create it by copying the env.make.sample and update it properly."; \
		exit 1; \
	fi

##########################################################################
# POSTGRES
##########################################################################

# Run PostgreSQL container
.PHONY: postgres-up
postgres-up: ## Start the PostgreSQL container
	@echo "Starting PostgreSQL container..."
	@docker run -d \
		--name $(POSTGRES_CONTAINER_NAME) \
		--rm \
		-e POSTGRES_USER=$(POSTGRES_SUPERUSER) \
		-e POSTGRES_PASSWORD=$(POSTGRES_SUPERUSER_PASS) \
		-e POSTGRES_DB=$(POSTGRES_DB) \
		-p $(POSTGRES_PORT):5432 \
		-v $(POSTGRES_VOLUME):/var/lib/postgresql/data \
		$(POSTGRES_IMAGE)
	@echo "PostgreSQL container started on port $(POSTGRES_PORT)."

# Stop PostgreSQL container
.PHONY: postgres-down
postgres-down: ## Stop and remove the PostgreSQL container
	@echo "Stopping PostgreSQL container..."
	@docker stop $(POSTGRES_CONTAINER_NAME) || true
	@docker rm $(POSTGRES_CONTAINER_NAME) || true
	@echo "PostgreSQL container stopped and removed."

.PHONY: postgres-shell
postgres-shell: ## Open a shell to postgres
	@echo "Opening shell to PostgreSQL container..."
	@docker exec -e PGPASSWORD=$(POSTGRES_SUPERUSER_PASS) -it $(POSTGRES_CONTAINER_NAME) \
	psql -U $(POSTGRES_SUPERUSER) -d $(POSTGRES_DB)

# Display logs for PostgreSQL container
.PHONY: postgres-logs
postgres-logs: ## Display logs for the PostgreSQL container
	@docker logs -f $(POSTGRES_CONTAINER_NAME)

# List running containers
.PHONY: postgres-status
postgres-status: ## Show the status of the PostgreSQL container
	@docker ps | grep $(POSTGRES_CONTAINER_NAME) || echo "PostgreSQL container is not running."

.PHONY: postgres-volume-rm
postgres-volume-rm: postgres-down ## Remove the PostgreSQL Docker volume
	@echo "Removing PostgreSQL data volume..."
	@docker volume rm $(POSTGRES_VOLUME) || true

.PHONY: postgres-drop-db
postgres-drop-db: ## Drop a specific database by name
	@read -p "Enter database name to drop: " DB_NAME; \
	if [ -z "$$DB_NAME" ]; then \
		echo "Error: No database name provided. Exiting."; \
		exit 1; \
	fi; \
	echo "Dropping database '$$DB_NAME'..."; \
	docker exec -e PGPASSWORD=$(POSTGRES_SUPERUSER_PASS) \
		-it $(POSTGRES_CONTAINER_NAME) \
		psql -U $(POSTGRES_SUPERUSER) -d postgres -c "DROP DATABASE IF EXISTS \"$$DB_NAME\";"; \
	echo "Database '$$DB_NAME' dropped successfully."

##########################################################################
# BUILD
##########################################################################

.PHONY: clean
clean: ## Clean the solution
	find . $(SRC_DIR) -iname "bin" | xargs rm -rf
	find . $(SRC_DIR) -iname "obj" | xargs rm -rf

##########################################################################
# EF
##########################################################################

.PHONY: ef-migration-add
ef-migration-add: ## Create a new EF Core migration
	@read -p "Enter migration name: " name && \
	dotnet ef migrations add $$name \
	--project $(SRC)/Sessions \
	--startup-project $(SRC)/Sessions \
	--output-dir $(SRC)/Sessions/Infrastructure/Repositories/EF/Migrations

.PHONY: ef-database-update
ef-database-update: ## Apply EF Core migrations to the database
	@echo "Applying EF Core migrations..."
	@dotnet ef database update \
	--project $(SRC)/Sessions \
	--startup-project $(SRC)/Sessions \
	--verbose
	@echo "Database updated successfully."
