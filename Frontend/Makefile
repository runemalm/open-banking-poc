##########################################################################
# This is the project's Makefile.
##########################################################################

##########################################################################
# VARIABLES
##########################################################################

HOME := $(shell echo ~)
PWD := $(shell pwd)
SRC := $(PWD)/src
DIST := $(PWD)/dist
PUBLIC := $(PWD)/public
DEMO := $(PWD)/demo

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

.PHONY: check-env
check-env: ## Check that the env.make file exists
	@if [ ! -f env.make ]; then \
		echo "Error: env.make file not found! Create it by copying the env.make.sample and update it properly."; \
		exit 1; \
	fi

##########################################################################
# DEVELOPMENT
##########################################################################

.PHONY: start-app
start-app: check-env ## Start the development server
	npm start

.PHONY: watch-sdk
watch-sdk: check-env ## Watch for changes and rebuild the SDK
	npm run sdk-bundle-watch

##########################################################################
# BUILD
##########################################################################

.PHONY: bundle-sdk
bundle-sdk: check-env ## Build the browser-ready SDK
	npm run sdk-bundle

.PHONY: build-app
build-app: check-env ## Build the React app for production
	npm run build

.PHONY: build-all
build-all: bundle-sdk build-app ## Build both the SDK and the React app

##########################################################################
# TESTING
##########################################################################

.PHONY: test
test: check-env ## Run tests
	npm test

##########################################################################
# CLEANUP
##########################################################################

.PHONY: clean-dist
clean-dist: ## Clean the dist directory
	@echo "Cleaning dist directory..."
	rm -rf $(DIST)

.PHONY: clean-public-sdk
clean-public-sdk: ## Remove the SDK file from the public directory
	@echo "Removing SDK from public directory..."
	rm -f $(PUBLIC)/open-banking-client-sdk.js

.PHONY: clean-all
clean-all: clean-dist clean-public-sdk ## Clean all generated files
	@echo "Cleaning completed."

##########################################################################
# DEPLOYMENT
##########################################################################

.PHONY: deploy
deploy: build-all ## Deploy the application (placeholder)
	@echo "Add your deployment commands here."

##########################################################################
# DEMO
##########################################################################

.PHONY: demo-start
demo-start: ## Start the demo server
	@cd $(DEMO) && python3 -m http.server 8000
