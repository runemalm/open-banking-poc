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

##########################################################################
# GOOGLE CLOUD PLATFORM
##########################################################################

PROJECT_ID := open-banking-poc-447414
REGION := europe-north1
REPO_NAME := open-banking-poc
IMAGE_NAME := $(APP)-backend
IMAGE_TAG := latest
CONTAINER_IMAGE := $(REGION)-docker.pkg.dev/$(PROJECT_ID)/$(REPO_NAME)/$(IMAGE_NAME):$(IMAGE_TAG)
SERVICE_NAME := $(APP)-backend

.PHONY: gcp-auth-login
gcp-auth-login: ## Authenticate Google Cloud CLI
	@echo "Authenticating Google Cloud CLI..."
	@gcloud auth login
	@echo "Authenticating with Google Cloud CLI completed successfully."

.PHONY: gcp-auth-registry
gcp-auth-registry: ## Authenticate Docker with Google Cloud Registry
	@echo "Authenticating Docker with Google Cloud Registry..."
	@gcloud auth configure-docker $(REGION)-docker.pkg.dev

.PHONY: gcp-set-account
gcp-set-account: ## Set the active Google Cloud account
	@read -p "Enter the email address of the account to set as active: " ACCOUNT && \
	echo "Setting active account to $$ACCOUNT..." && \
	gcloud config set account $$ACCOUNT && \
	echo "Active account successfully set to $$ACCOUNT."

.PHONY: gcp-list-configs
gcp-list-configs: ## List all Google Cloud CLI configurations
	@echo "Listing all Google Cloud CLI configurations..."
	@gcloud config configurations list

.PHONY: gcp-create-config
gcp-create-config: ## Create a new Google Cloud CLI configuration for your project
	@read -p "Enter the name of the new configuration: " CONFIG_NAME && \
	read -p "Enter the project ID to associate with this configuration: " PROJECT_ID && \
	echo "Creating new configuration '$$CONFIG_NAME'..." && \
	gcloud config configurations create $$CONFIG_NAME && \
	gcloud config configurations activate $$CONFIG_NAME && \
	gcloud config set project $$PROJECT_ID && \
	echo "Configuration '$$CONFIG_NAME' created and associated with project '$$PROJECT_ID'."

.PHONY: gcp-set-config
gcp-set-config: ## Set a specific Google Cloud CLI configuration
	@read -p "Enter the name of the configuration to set: " CONFIG_NAME && \
	echo "Setting active configuration to $$CONFIG_NAME..." && \
	gcloud config configurations activate $$CONFIG_NAME && \
	echo "Active configuration successfully set to $$CONFIG_NAME."

.PHONY: gcp-auth-credentials
gcp-auth-credentials: ## Set up Application Default Credentials for your account
	@echo "Setting up Application Default Credentials (ADC)..."
	@gcloud auth application-default login
	@echo "Application Default Credentials successfully set up."

.PHONY: gcp-list-projects
gcp-list-projects: ## List all Google Cloud projects
	@echo "Listing all Google Cloud projects..."
	@gcloud projects list

.PHONY: gcp-set-project
gcp-set-project: ## Set the active Google Cloud project
	@echo "Setting active project to $(PROJECT_ID)..."
	@gcloud config set project $(PROJECT_ID)

.PHONY: gcp-build
gcp-build: ## Build the Docker image
	@echo "Building Docker image..."
	@docker build -t $(CONTAINER_IMAGE) .

.PHONY: gcp-test-locally
gcp-test-locally: ## Test the Docker image locally with ASPNETCORE_ENVIRONMENT set to Production
	@echo "Running Docker image locally with ASPNETCORE_ENVIRONMENT=Production..."
	@docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:8080 $(CONTAINER_IMAGE)
	@echo "Docker image is running on http://localhost:8080 with ASPNETCORE_ENVIRONMENT=Production"

.PHONY: gcp-push
gcp-push: ## Push the Docker image to Google Artifact Registry
	@echo "Pushing Docker image to Google Artifact Registry..."
	@docker push $(CONTAINER_IMAGE)

.PHONY: gcp-deploy
gcp-deploy: ## Deploy the backend to Google Cloud Run
	@echo "Deploying to Google Cloud Run..."
	@gcloud run deploy $(SERVICE_NAME) \
		--image=$(CONTAINER_IMAGE) \
		--platform=managed \
		--region=$(REGION) \
		--allow-unauthenticated \
		--memory=2Gi
	@echo "Deployment successful!"

.PHONY: gcp-metrics
gcp-metrics: ## Fetch status and conditions of the Cloud Run service
	@echo "Fetching metrics for Cloud Run service: $(SERVICE_NAME)..."
	@gcloud run services describe $(SERVICE_NAME) \
		--platform=managed \
		--region=$(REGION) \
		--format="value(status.conditions)"

.PHONY: gcp-logs
gcp-logs: ## Stream real-time logs for the Cloud Run service
	@echo "Streaming logs for Cloud Run service: $(SERVICE_NAME)..."
	@gcloud beta run services logs tail $(SERVICE_NAME) --region $(REGION)

.PHONY: gcp-get-url
gcp-get-url: ## Get the static URL of the deployed Cloud Run backend
	@echo "Retrieving the Cloud Run service URL for $(SERVICE_NAME)..."
	@gcloud run services describe $(SERVICE_NAME) \
		--platform=managed \
		--region=$(REGION) \
		--format="value(status.url)"

.PHONY: gcp-autoscale
gcp-autoscale: ## Update autoscaling rules for the Cloud Run service
	@echo "Updating autoscaling rules for $(SERVICE_NAME)..."
	@gcloud run services update $(SERVICE_NAME) \
		--platform=managed \
		--region=$(REGION) \
		--min-instances=1 \
		--max-instances=10
	@echo "Autoscaling rules updated successfully."

.PHONY: gcp-clean
gcp-clean: ## Remove local Docker images
	@echo "Cleaning up local Docker images..."
	@docker rmi $(CONTAINER_IMAGE) || true
