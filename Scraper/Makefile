##########################################################################
# This is the project's Makefile.
##########################################################################

##########################################################################
# VARIABLES
##########################################################################

APP := scraper
HOME := $(shell echo ~)
PWD := $(shell pwd)
SRC := $(PWD)/src
PROXY_VM := $(APP)-proxy
PROXY_IP := 4.225.173.22

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
# AZURE PROXY
##########################################################################

.PHONY: create-resource-group
create-resource-group: ## Create the resource group if it doesn't already exist
	@if ! az group show --name $(AZURE_RESOURCE_GROUP) --subscription $(AZURE_SUBSCRIPTION) > /dev/null 2>&1; then \
		echo "Resource group $(AZURE_RESOURCE_GROUP) does not exist. Creating..."; \
		az group create --name $(AZURE_RESOURCE_GROUP) --location $(AZURE_LOCATION) --subscription $(AZURE_SUBSCRIPTION); \
	else \
		echo "Resource group $(AZURE_RESOURCE_GROUP) already exists."; \
	fi

.PHONY: create-vm
create-vm: check-env ## Create an Azure VM for the proxy server
	az vm create \
		--resource-group $(AZURE_RESOURCE_GROUP) \
		--name $(PROXY_VM) \
		--image Ubuntu2404 \
		--admin-username $(AZURE_VM_ADMIN) \
		--authentication-type password \
		--admin-password $(AZURE_VM_PASSWORD) \
		--public-ip-address-allocation static

.PHONY: get-proxy-ip
get-proxy-ip: ## Get the public IP of the proxy server
	@az vm list-ip-addresses \
		--resource-group $(AZURE_RESOURCE_GROUP) \
		--name $(PROXY_VM) \
		--query "[0].virtualMachine.network.publicIpAddresses[0].ipAddress" \
		--output tsv

.PHONY: install-squid
install-squid: ## Install Squid on the proxy server
	@echo "Installing Squid on the proxy server...";
	@ssh $(AZURE_VM_ADMIN)@$(PROXY_IP) "sudo apt update && sudo apt install -y squid && echo 'Squid installed successfully.'"

.PHONY: ssh-vm
ssh-vm: ## Drop into vm ssh shell
	@echo "Dropping into vm ssh shell...";
	@ssh $(AZURE_VM_ADMIN)@$(PROXY_IP)

.PHONY: configure-squid
configure-squid: ## Configure Squid on the proxy server
	@echo "Configuring Squid on the proxy server...";
	@ssh $(AZURE_VM_ADMIN)@$(PROXY_IP) \
	"echo 'acl allowed_ips src 0.0.0.0/0' | sudo tee -a /etc/squid/squid.conf && \
	echo 'http_access allow allowed_ips' | sudo tee -a /etc/squid/squid.conf && \
	sudo systemctl restart squid"
	@echo "Squid proxy server configured successfully."

.PHONY: allow-proxy-port
allow-proxy-port: ## Allow proxy traffic in Azure VM firewall
	az network nsg rule create \
		--resource-group $(AZURE_RESOURCE_GROUP) \
		--nsg-name $(PROXY_VM)NSG \
		--name AllowSquid \
		--priority 1100 \
		--direction Inbound \
		--access Allow \
		--protocol Tcp \
		--source-port-ranges '*' \
		--destination-port-ranges 3128 \
		--source-address-prefixes '*' \
		--destination-address-prefixes '*'

.PHONY: test-proxy
test-proxy: ## Test the Squid proxy server
	curl -x http://$(PROXY_IP):3128 -L http://www.google.com

.PHONY: destroy-vm
destroy-vm: ## Destroy the proxy VM
	az vm delete \
		--resource-group $(AZURE_RESOURCE_GROUP) \
		--name $(PROXY_VM) \
		--yes
	az network public-ip delete \
		--resource-group $(AZURE_RESOURCE_GROUP) \
		--name $(PROXY_VM)PublicIP
	@echo "Proxy VM and associated resources destroyed."

.PHONY: destroy-resource-group
destroy-resource-group: ## Destroy the resource group and all resources in it
	@if [ -z "$(AZURE_RESOURCE_GROUP)" ]; then \
		echo "Error: AZURE_RESOURCE_GROUP variable is not set. Please specify the resource group name."; \
		exit 1; \
	fi
	@echo "Deleting resource group $(AZURE_RESOURCE_GROUP) and all associated resources...";
	@az group delete --name $(AZURE_RESOURCE_GROUP) --subscription $(AZURE_SUBSCRIPTION) --yes --no-wait
	@echo "Resource group deletion initiated. It may take a few minutes to complete."

##########################################################################
# PROXY OTHERS
##########################################################################

.PHONY: proxy1-ipinfo
proxy1-ipinfo: ## Request IP information from ipinfo.io through proxy 1
	@echo "Requesting IP information from ipinfo.io..."
	@if [ -z "$(PROXY1)" ]; then \
		echo "Error: PROXY variable is not set. Please provide a proxy configuration (e.g., http://user:pass@proxy:port)."; \
		exit 1; \
	fi
	@curl -x $(PROXY1) -L https://ipinfo.io

.PHONY: proxy1-headers
proxy1-headers: ## Output the headers sent by the proxy
	@if [ -z "$(PROXY1)" ]; then \
		echo "Error: PROXY variable is not set. Please provide a proxy configuration (e.g., http://user:pass@proxy:port)."; \
		exit 1; \
	fi
	@echo "Fetching headers through the proxy $(PROXY1)..."
	@curl -x $(PROXY1) -L https://httpbin.org/headers -v


##########################################################################
# FULL SETUP
##########################################################################

.PHONY: setup-proxy
setup-proxy: create-vm get-proxy-ip install-squid configure-squid allow-proxy-port ## Full proxy setup workflow

